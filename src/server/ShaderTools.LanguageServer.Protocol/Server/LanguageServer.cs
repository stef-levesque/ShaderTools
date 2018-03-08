//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis;
using ShaderTools.CodeAnalysis.Completion;
using ShaderTools.CodeAnalysis.GoToDefinition;
using ShaderTools.CodeAnalysis.Host.Mef;
using ShaderTools.CodeAnalysis.NavigateTo;
using ShaderTools.CodeAnalysis.QuickInfo;
using ShaderTools.CodeAnalysis.ReferenceHighlighting;
using ShaderTools.CodeAnalysis.Shared.Extensions;
using ShaderTools.CodeAnalysis.Text;
using ShaderTools.CodeAnalysis.Utilities;
using ShaderTools.LanguageServer.Protocol.LanguageServer;
using ShaderTools.LanguageServer.Protocol.MessageProtocol;
using ShaderTools.LanguageServer.Protocol.MessageProtocol.Channel;
using ShaderTools.LanguageServer.Protocol.Services.SignatureHelp;
using ShaderTools.LanguageServer.Protocol.Utilities;
using CompletionItem = ShaderTools.LanguageServer.Protocol.LanguageServer.CompletionItem;

namespace ShaderTools.LanguageServer.Protocol.Server
{
    public sealed class LanguageServer : ProtocolEndpoint
    {
        private static readonly string DiagnosticSourceName = "ShaderToolsEditorServices";

        private static readonly BidirectionalMap<string, string> VsCodeLanguageToLanguageNameMap;

        static LanguageServer()
        {
            VsCodeLanguageToLanguageNameMap = new BidirectionalMap<string, string>(new[]
            {
                new KeyValuePair<string, string>("hlsl", LanguageNames.Hlsl),
                new KeyValuePair<string, string>("shaderlab", LanguageNames.ShaderLab)
            });
        }

        private readonly IMefHostExportProvider _exportProvider;
        private readonly LanguageServerWorkspace _workspace;

        private string _workspacePath;

        private static CancellationTokenSource existingRequestCancellation;

        public LanguageServer(ChannelBase serverChannel)
            : base(serverChannel, MessageProtocolType.LanguageServer)
        {
            var hostServices = CreateHostServices();
            _exportProvider = hostServices;
            _workspace = new LanguageServerWorkspace(hostServices);
        }

        private static MefHostServices CreateHostServices()
        {
            var assemblies = MefHostServices.DefaultAssemblies
                .Union(new[] { typeof(LanguageServerWorkspace).GetTypeInfo().Assembly });

            return MefHostServices.Create(assemblies);
        }

        protected override Task OnStart()
        {
            // Register handlers for server lifetime messages
            this.SetRequestHandler(ShutdownRequest.Type, this.HandleShutdownRequest);
            this.SetEventHandler(ExitNotification.Type, this.HandleExitNotification);

            // Initialize the implementation class
            this.Initialize();

            return Task.FromResult(true);
        }

        protected override async Task OnStop()
        {
            await Shutdown();
        }

        /// <summary>
        /// Overridden by the subclass to provide initialization
        /// logic after the server channel is started.
        /// </summary>
        private void Initialize()
        {
            SetRequestHandler(InitializeRequest.Type, this.HandleInitializeRequest);

            SetEventHandler(DidOpenTextDocumentNotification.Type, this.HandleDidOpenTextDocumentNotification);
            SetEventHandler(DidCloseTextDocumentNotification.Type, this.HandleDidCloseTextDocumentNotification);
            SetEventHandler(DidChangeTextDocumentNotification.Type, this.HandleDidChangeTextDocumentNotification);

            SetRequestHandler(DocumentHighlightRequest.Type, this.HandleDocumentHighlightRequest);
            SetRequestHandler(SignatureHelpRequest.Type, this.HandleSignatureHelpRequest);
            SetRequestHandler(DefinitionRequest.Type, this.HandleDefinitionRequest);
            SetRequestHandler(HoverRequest.Type, this.HandleHoverRequest);
            SetRequestHandler(DocumentSymbolRequest.Type, this.HandleDocumentSymbolRequest);
            SetRequestHandler(WorkspaceSymbolRequest.Type, this.HandleWorkspaceSymbolRequest);
            SetRequestHandler(CompletionRequest.Type, this.HandleCompletionRequest);
        }

        /// <summary>
        /// Can be overridden by the subclass to provide shutdown
        /// logic before the server exits.  Subclasses do not need
        /// to invoke or return the value of the base implementation.
        /// </summary>
        private Task Shutdown()
        {
            Logger.Write(LogLevel.Normal, "Language service is shutting down...");

            return Task.FromResult(true);
        }

        private async Task HandleInitializeRequest(
            InitializeParams initializeParams,
            RequestContext<InitializeResult> requestContext)
        {
            // Grab the workspace path from the parameters
            _workspacePath = initializeParams.RootPath;

            await requestContext.SendResult(
                new InitializeResult
                {
                    Capabilities = new ServerCapabilities
                    {
                        TextDocumentSync = TextDocumentSyncKind.Incremental,
                        DefinitionProvider = true,
                        //ReferencesProvider = true,
                        DocumentHighlightProvider = true,
                        DocumentSymbolProvider = true,
                        WorkspaceSymbolProvider = true,
                        HoverProvider = true,
                        //CodeActionProvider = true,
                        CompletionProvider = new Protocol.LanguageServer.CompletionOptions
                        {
                            ResolveProvider = false,
                            TriggerCharacters = new [] { ".", ":" }
                        },
                        SignatureHelpProvider = new SignatureHelpOptions
                        {
                            TriggerCharacters = new[] { "(" }
                        }
                    }
                });
        }

        private Task HandleDidOpenTextDocumentNotification(
            DidOpenTextDocumentParams openParams,
            EventContext eventContext)
        {
            var openedDocument = _workspace.OpenDocument(
                DocumentId.CreateNewId(ResolveFilePath(openParams.TextDocument.Uri)),
                SourceText.From(openParams.TextDocument.Text, ResolveFilePath(openParams.TextDocument.Uri)),
                GetLanguageName(openParams.TextDocument.LanguageId));

            // TODO: Get all recently edited files in the workspace
            this.RunScriptDiagnostics(new Document[] { openedDocument });

            Logger.Write(LogLevel.Verbose, "Finished opening document.");

            return Task.FromResult(true);
        }

        private static string GetLanguageName(string languageId)
        {
            if (VsCodeLanguageToLanguageNameMap.TryGetValue(languageId, out var languageName))
            {
                return languageName;
            }

            throw new ArgumentOutOfRangeException(nameof(languageId), languageId, "Invalid languageId");
        }

        private async Task HandleDidCloseTextDocumentNotification(
            DidCloseTextDocumentParams closeParams,
            EventContext eventContext)
        {
            // Find and close the file in the current session
            var fileToClose = GetDocument(closeParams.TextDocument);

            if (fileToClose != null)
            {
                _workspace.CloseDocument(fileToClose.Id);
                await ClearMarkers(fileToClose, eventContext);
            }

            Logger.Write(LogLevel.Verbose, "Finished closing document.");
        }

        private Task HandleDidChangeTextDocumentNotification(
            DidChangeTextDocumentParams textChangeParams,
            EventContext eventContext)
        {
            var fileToChange = GetDocument(textChangeParams.TextDocument);

            if (fileToChange == null)
            {
                return Task.FromResult(true);
            }

            // A text change notification can batch multiple change requests
            var updatedDocument = _workspace.UpdateDocument(fileToChange,
                textChangeParams.ContentChanges.Select(x =>
                    GetFileChangeDetails(
                    fileToChange,
                    x.Range.Value,
                    x.Text)));

            // TODO: Get all recently edited files in the workspace
            this.RunScriptDiagnostics(new[] { updatedDocument } );

            return Task.FromResult(true);
        }

        private async Task HandleDocumentHighlightRequest(
            TextDocumentPositionParams textDocumentPositionParams,
            RequestContext<DocumentHighlight[]> requestContext)
        {
            var document = GetDocument(textDocumentPositionParams.TextDocument);
            var position = ConvertPosition(document, textDocumentPositionParams.Position);

            var documentHighlightsService = document.Workspace.Services.GetService<IDocumentHighlightsService>();
            
            var documentHighlightsList = await documentHighlightsService.GetDocumentHighlightsAsync(
                document, position,
                ImmutableHashSet<Document>.Empty,
                CancellationToken.None);

            var result = new List<DocumentHighlight>();

            foreach (var documentHighlights in documentHighlightsList)
            {
                if (documentHighlights.Document != document)
                {
                    continue;
                }

                foreach (var highlightSpan in documentHighlights.HighlightSpans)
                {
                    result.Add(new DocumentHighlight
                    {
                        Kind = highlightSpan.Kind == HighlightSpanKind.Definition
                            ? DocumentHighlightKind.Write
                            : DocumentHighlightKind.Read,
                        Range = ConvertTextSpanToRange(document.SourceText, highlightSpan.TextSpan)
                    });
                }
            }

            await requestContext.SendResult(result.ToArray());
        }

        private async Task HandleSignatureHelpRequest(
            TextDocumentPositionParams textDocumentPositionParams,
            RequestContext<SignatureHelp> requestContext)
        {
            var document = GetDocument(textDocumentPositionParams.TextDocument);
            var position = ConvertPosition(document, textDocumentPositionParams.Position);

            var signatureHelpHandler = document.Workspace.Services.GetService<SignatureHelpHandler>();

            var result = await signatureHelpHandler.GetResultAsync(document, position, CancellationToken.None);

            await requestContext.SendResult(result);
        }

        private async Task HandleDefinitionRequest(
            TextDocumentPositionParams textDocumentPositionParams,
            RequestContext<Location[]> requestContext)
        {
            var document = GetDocument(textDocumentPositionParams.TextDocument);
            var position = ConvertPosition(document, textDocumentPositionParams.Position);

            var goToDefinitionService = document.GetLanguageService<IGoToDefinitionService>();
            var definitions = await goToDefinitionService.FindDefinitionsAsync(document, position, CancellationToken.None);

            // TODO: Handle spans within embedded HLSL blocks; the TextSpan is currently relative to the start of the embedded block.

            var locations = definitions
                .Select(x =>
                {
                    var sourceSpan = x.SourceSpans[0].SourceSpan;
                    return new Location
                    {
                        Uri = GetFileUri(sourceSpan.File.FilePath),
                        Range = ConvertTextSpanToRange(sourceSpan.File.Text, sourceSpan.Span)
                    };
                })
                .ToArray();

            await requestContext.SendResult(locations);
        }

        private async Task HandleHoverRequest(
            TextDocumentPositionParams textDocumentPositionParams,
            RequestContext<Hover> requestContext)
        {
            var document = GetDocument(textDocumentPositionParams.TextDocument);
            var position = ConvertPosition(document, textDocumentPositionParams.Position);

            var providerCoordinatorFactory = GetGlobalService<IQuickInfoProviderCoordinatorFactory>();
            var providerCoordinator = providerCoordinatorFactory.CreateCoordinator(document);

            var (item, _) = await providerCoordinator.GetItemAsync(document, position, CancellationToken.None);

            var symbolInfo = new List<MarkedString>();
            Range? symbolRange = null;

            if (item != null)
            {
                switch (item.Content)
                {
                    case QuickInfoDisplayContent c:
                        symbolInfo.Add(new MarkedString
                        {
                            Language = VsCodeLanguageToLanguageNameMap.GetKeyOrDefault(document.Language),
                            Value = c.MainDescription.GetFullText()
                        });

                        if (!c.Documentation.IsEmpty)
                        {
                            symbolInfo.Add(new MarkedString
                            {
                                Value = c.Documentation.GetFullText()
                            });
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                symbolRange = ConvertTextSpanToRange(document.SourceText, item.TextSpan);
            }

            await requestContext.SendResult(
                new Hover
                {
                    Contents = symbolInfo.ToArray(),
                    Range = symbolRange
                });
        }

        private async Task HandleDocumentSymbolRequest(
            DocumentSymbolParams documentSymbolParams,
            RequestContext<SymbolInformation[]> requestContext)
        {
            var document = GetDocument(documentSymbolParams.TextDocument);

            var searchService = _workspace.Services.GetService<INavigateToSearchService>();

            var symbols = ImmutableArray.CreateBuilder<SymbolInformation>();

            await FindSymbolsInDocument(searchService, document, string.Empty, symbols);

            await requestContext.SendResult(symbols.ToArray());
        }

        private async Task HandleWorkspaceSymbolRequest(
            WorkspaceSymbolParams workspaceSymbolParams,
            RequestContext<SymbolInformation[]> requestContext)
        {
            var searchService = _workspace.Services.GetService<INavigateToSearchService>();

            var symbols = ImmutableArray.CreateBuilder<SymbolInformation>();

            foreach (var document in _workspace.CurrentDocuments.Documents)
            {
                await FindSymbolsInDocument(searchService, document, workspaceSymbolParams.Query, symbols);
            }

            await requestContext.SendResult(symbols.ToArray());
        }

        private async Task HandleCompletionRequest(
            TextDocumentPositionParams completionParams,
            RequestContext<CompletionItem[]> requestContext)
        {
            var document = GetDocument(completionParams.TextDocument);
            var position = ConvertPosition(document, completionParams.Position);

            var completionService = document.GetLanguageService<CompletionService>();

            var completionList = await completionService.GetCompletionsAsync(document, position);

            var completionItems = completionList.Items
                .Select(x => ConvertCompletionItem(document, completionList.Rules, x))
                .ToArray();

            await requestContext.SendResult(completionItems);
        }

        private static CompletionItem ConvertCompletionItem(Document document, CompletionRules completionRules, CodeAnalysis.Completion.CompletionItem item)
        {
            var documentation = CommonCompletionItem.HasDescription(item)
                ? CommonCompletionItem.GetDescription(item).Text
                : string.Empty;

            return new CompletionItem
            {
                Label = item.DisplayText,
                SortText = item.SortText,
                FilterText = item.FilterText,
                Kind = CompletionItemKind.Class,
                TextEdit = new TextEdit
                {
                    NewText = item.DisplayText,
                    Range = ConvertTextSpanToRange(document.SourceText, item.Span)
                },
                Documentation = documentation,
                CommitCharacters = completionRules.DefaultCommitCharacters.Select(x => x.ToString()).ToArray()
            };
        }

        private async Task FindSymbolsInDocument(
            INavigateToSearchService searchService, Document document,
            string searchPattern,
            ImmutableArray<SymbolInformation>.Builder resultsBuilder)
        {
            var foundSymbols = await searchService.SearchDocumentAsync(document, searchPattern, CancellationToken.None);

            resultsBuilder.AddRange(foundSymbols
               .Select(r => new SymbolInformation
                {
                    ContainerName = r.AdditionalInformation,
                    Kind = GetSymbolKind(r.Kind),
                    Location = new Location
                    {
                        Uri = GetFileUri(r.NavigableItem.SourceSpan.File.FilePath),
                        Range = ConvertTextSpanToRange(r.NavigableItem.SourceSpan.File.Text, r.NavigableItem.SourceSpan.Span)
                    },
                    Name = r.Name
                }));
        }

        private static SymbolKind GetSymbolKind(string symbolType)
        {
            switch (symbolType)
            {
                case NavigateToItemKind.Class:
                case NavigateToItemKind.Structure:
                    return SymbolKind.Class;

                case NavigateToItemKind.Module:
                    return SymbolKind.Namespace;

                case NavigateToItemKind.Interface:
                    return SymbolKind.Interface;

                case NavigateToItemKind.Field:
                    return SymbolKind.Field;

                case NavigateToItemKind.Method:
                    return SymbolKind.Method;

                default:
                    return SymbolKind.Variable;
            }
        }

        private static string GetFileUri(string filePath)
        {
            // If the file isn't untitled, return a URI-style path
            return
                !filePath.StartsWith("untitled")
                    ? new Uri("file://" + filePath).AbsoluteUri
                    : filePath;
        }

        private static Range ConvertTextSpanToRange(SourceText sourceText, TextSpan textSpan)
        {
            var linePositionSpan = sourceText.Lines.GetLinePositionSpan(textSpan);

            return new Range
            {
                Start = new Position
                {
                    Line = linePositionSpan.Start.Line,
                    Character = linePositionSpan.Start.Character
                },
                End = new Position
                {
                    Line = linePositionSpan.End.Line,
                    Character = linePositionSpan.End.Character
                }
            };
        }

        private T GetGlobalService<T>()
            where T : class
        {
            return _exportProvider.GetExports<T>().FirstOrDefault()?.Value;
        }

        private Document GetDocument(TextDocumentIdentifier documentIdentifier)
        {
            var filePath = ResolveFilePath(documentIdentifier.Uri);

            return _workspace.CurrentDocuments
                .GetDocumentsWithFilePath(filePath)
                .FirstOrDefault();
        }

        private int ConvertPosition(Document document, Position position)
        {
            return document.SourceText.Lines.GetPosition(new LinePosition(position.Line, position.Character));
        }

        private static TextChange GetFileChangeDetails(Document document, Range changeRange, string insertString)
        {
            var startPosition = document.SourceText.Lines.GetPosition(new LinePosition(changeRange.Start.Line, changeRange.Start.Character));
            var endPosition = document.SourceText.Lines.GetPosition(new LinePosition(changeRange.End.Line, changeRange.End.Character));

            return new TextChange(TextSpan.FromBounds(startPosition, endPosition), insertString);
        }

        private async Task ClearMarkers(Document scriptFile, EventContext eventContext)
        {
            // send empty diagnostic markers to clear any markers associated with the given file
            await PublishScriptDiagnostics(
                    scriptFile,
                    ImmutableArray<CodeAnalysis.Diagnostics.MappedDiagnostic>.Empty,
                    eventContext);
        }

        private static async Task PublishScriptDiagnostics(
            Document scriptFile,
            ImmutableArray<CodeAnalysis.Diagnostics.MappedDiagnostic> markers,
            EventContext eventContext)
        {
            await PublishScriptDiagnostics(
                scriptFile,
                markers,
                eventContext.SendEvent);
        }

        private Task RunScriptDiagnostics(
            Document[] filesToAnalyze)
        {
            return RunScriptDiagnostics(filesToAnalyze, this.SendEvent);
        }

        private Task RunScriptDiagnostics(
            Document[] filesToAnalyze,
            Func<NotificationType<PublishDiagnosticsNotification, object>, PublishDiagnosticsNotification, Task> eventSender)
        {
            // If there's an existing task, attempt to cancel it
            try
            {
                if (existingRequestCancellation != null)
                {
                    // Try to cancel the request
                    existingRequestCancellation.Cancel();

                    // If cancellation didn't throw an exception,
                    // clean up the existing token
                    existingRequestCancellation.Dispose();
                    existingRequestCancellation = null;
                }
            }
            catch (Exception e)
            {
                // TODO: Catch a more specific exception!
                Logger.Write(
                    LogLevel.Error,
                    string.Format(
                        "Exception while canceling analysis task:\n\n{0}",
                        e.ToString()));

                TaskCompletionSource<bool> cancelTask = new TaskCompletionSource<bool>();
                cancelTask.SetCanceled();
                return cancelTask.Task;
            }

            // Create a fresh cancellation token and then start the task.
            // We create this on a different TaskScheduler so that we
            // don't block the main message loop thread.
            // TODO: Is there a better way to do this?
            existingRequestCancellation = new CancellationTokenSource();
            Task.Factory.StartNew(
                () =>
                    DelayThenInvokeDiagnostics(
                        750,
                        filesToAnalyze,
                        eventSender,
                        existingRequestCancellation.Token),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);

            return Task.FromResult(true);
        }

        private static async Task DelayThenInvokeDiagnostics(
            int delayMilliseconds,
            Document[] filesToAnalyze,
            Func<NotificationType<PublishDiagnosticsNotification, object>, PublishDiagnosticsNotification, Task> eventSender,
            CancellationToken cancellationToken)
        {
            // First of all, wait for the desired delay period before
            // analyzing the provided list of files
            try
            {
                await Task.Delay(delayMilliseconds, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // If the task is cancelled, exit directly
                return;
            }

            // If we've made it past the delay period then we don't care
            // about the cancellation token anymore.  This could happen
            // when the user stops typing for long enough that the delay
            // period ends but then starts typing while analysis is going
            // on.  It makes sense to send back the results from the first
            // delay period while the second one is ticking away.

            // Get the requested files
            foreach (var scriptFile in filesToAnalyze)
            {
                Logger.Write(LogLevel.Verbose, "Analyzing script file: " + scriptFile.FilePath);

                var diagnosticService = scriptFile.Workspace.Services.GetService<CodeAnalysis.Diagnostics.IDiagnosticService>();

                var diagnostics = await diagnosticService.GetDiagnosticsAsync(scriptFile.Id, CancellationToken.None);

                Logger.Write(LogLevel.Verbose, "Analysis complete.");

                await PublishScriptDiagnostics(
                    scriptFile,
                    diagnostics,
                    eventSender);
            }
        }

        private static async Task PublishScriptDiagnostics(
            Document scriptFile,
            ImmutableArray<CodeAnalysis.Diagnostics.MappedDiagnostic> markers,
            Func<NotificationType<PublishDiagnosticsNotification, object>, PublishDiagnosticsNotification, Task> eventSender)
        {
            var diagnostics = new List<Diagnostic>();

            foreach (var marker in markers)
            {
                // Does the marker contain a correction?
                var markerDiagnostic = GetDiagnosticFromMarker(marker);

                diagnostics.Add(markerDiagnostic);
            }

            // Always send syntax and semantic errors.  We want to
            // make sure no out-of-date markers are being displayed.
            await eventSender(
                PublishDiagnosticsNotification.Type,
                new PublishDiagnosticsNotification
                {
                    Uri = GetFileUri(scriptFile.FilePath),
                    Diagnostics = diagnostics.ToArray()
                });
        }

        private static Diagnostic GetDiagnosticFromMarker(CodeAnalysis.Diagnostics.MappedDiagnostic diagnostic)
        {
            var sourceFileSpan = diagnostic.FileSpan;

            var linePositionSpan = ConvertTextSpanToRange(sourceFileSpan.File.Text, sourceFileSpan.Span);

            return new Diagnostic
            {
                Severity = MapDiagnosticSeverity(diagnostic.Diagnostic.Severity),
                Message = diagnostic.Diagnostic.Message,
                Code = diagnostic.Diagnostic.Descriptor.Id,
                Source = DiagnosticSourceName,
                Range = linePositionSpan
            };
        }

        private static DiagnosticSeverity MapDiagnosticSeverity(CodeAnalysis.Diagnostics.DiagnosticSeverity severity)
        {
            switch (severity)
            {
                case CodeAnalysis.Diagnostics.DiagnosticSeverity.Error:
                    return DiagnosticSeverity.Error;

                case CodeAnalysis.Diagnostics.DiagnosticSeverity.Warning:
                    return DiagnosticSeverity.Warning;

                default:
                    return DiagnosticSeverity.Error;
            }
        }

        private async Task HandleShutdownRequest(
            RequestContext<object> requestContext)
        {
            // Allow the implementor to shut down gracefully
            await this.Shutdown();

            await requestContext.SendResult(new object());
        }

        private async Task HandleExitNotification(
            object exitParams,
            EventContext eventContext)
        {
            // Stop the server channel
            await this.Stop();
        }

        private static bool IsPathInMemory(string filePath)
        {
            // When viewing PowerShell files in the Git diff viewer, VS Code
            // sends the contents of the file at HEAD with a URI that starts
            // with 'inmemory'.  Untitled files which have been marked of
            // type PowerShell have a path starting with 'untitled'.
            return
                filePath.StartsWith("inmemory") ||
                filePath.StartsWith("untitled") ||
                filePath.StartsWith("private") ||
                filePath.StartsWith("git");

            // TODO #342: Remove 'private' and 'git' and then add logic to
            // throw when any unsupported file URI scheme is encountered.
        }

        private string ResolveFilePath(string filePath)
        {
            if (!IsPathInMemory(filePath))
            {
                if (filePath.StartsWith(@"file://"))
                {
                    // Client sent the path in URI format, extract the local path
                    Uri fileUri = new Uri(Uri.UnescapeDataString(filePath));
                    filePath = fileUri.LocalPath;
                }

                if (!Path.IsPathRooted(filePath))
                {
                    filePath = Path.Combine(_workspacePath, filePath);
                }

                // Get the absolute file path
                filePath = Path.GetFullPath(filePath);
            }

            Logger.Write(LogLevel.Verbose, "Resolved path: " + filePath);

            return filePath;
        }
    }
}

