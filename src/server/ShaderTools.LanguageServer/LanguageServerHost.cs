using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Serilog;
using ShaderTools.CodeAnalysis;
using ShaderTools.CodeAnalysis.Diagnostics;
using ShaderTools.CodeAnalysis.Host.Mef;
using ShaderTools.LanguageServer.Handlers;

namespace ShaderTools.LanguageServer
{
    internal sealed class LanguageServerHost : IDisposable
    {
        private readonly OmniSharp.Extensions.LanguageServer.Server.LanguageServer _server;

        private readonly MefHostServices _exportProvider;
        private LanguageServerWorkspace _workspace;
        private DiagnosticNotifier _diagnosticNotifier;

        private readonly LoggerFactory _loggerFactory;
        private readonly Serilog.Core.Logger _logger;

        public LanguageServerHost(
            Stream input,
            Stream output,
            string logFilePath,
            LogLevel minLogLevel)
        {
            _exportProvider = CreateHostServices();

            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(logFilePath)
                .CreateLogger();

            _loggerFactory = new LoggerFactory(
                ImmutableArray<ILoggerProvider>.Empty, 
                new LoggerFilterOptions { MinLevel = minLogLevel });

            _loggerFactory.AddSerilog(_logger);

            _server = new OmniSharp.Extensions.LanguageServer.Server.LanguageServer(input, output, _loggerFactory);
            _server.OnInitialize(Initialize);
        }

        private static MefHostServices CreateHostServices()
        {
            var assemblies = MefHostServices.DefaultAssemblies
                .Union(new[] { typeof(LanguageServerHost).GetTypeInfo().Assembly });

            return MefHostServices.Create(assemblies);
        }

        private Task Initialize(InitializeParams request)
        {
            _workspace = new LanguageServerWorkspace(_exportProvider, request.RootPath);

            var diagnosticService = _workspace.Services.GetService<IDiagnosticService>();
            _diagnosticNotifier = new DiagnosticNotifier(_server, diagnosticService);

            var documentSelector = new DocumentSelector(
                LanguageNames.AllLanguages
                    .Select(x => new DocumentFilter
                    {
                        Language = x
                    }));

            var registrationOptions = new TextDocumentRegistrationOptions
            {
                DocumentSelector = documentSelector
            };

            _server.AddHandler(new TextDocumentSyncHandler(_workspace, registrationOptions));

            _server.AddHandler(new CompletionHandler(_workspace, registrationOptions));
            _server.AddHandler(new DefinitionHandler(_workspace, registrationOptions));
            _server.AddHandler(new WorkspaceSymbolsHandler(_workspace));
            _server.AddHandler(new DocumentHighlightHandler(_workspace, registrationOptions));
            _server.AddHandler(new DocumentSymbolsHandler(_workspace, registrationOptions));
            _server.AddHandler(new HoverHandler(_workspace, registrationOptions));
            _server.AddHandler(new SignatureHelpHandler(_workspace, registrationOptions));

            return Task.CompletedTask;
        }

        public Task Initialize() => _server.Initialize();

        public Task WaitForExit => _server.WaitForExit;

        public void Dispose()
        {
            _diagnosticNotifier?.Dispose();

            _server.Dispose();

            _logger.Dispose();
            _loggerFactory.Dispose();
        }
    }
}
