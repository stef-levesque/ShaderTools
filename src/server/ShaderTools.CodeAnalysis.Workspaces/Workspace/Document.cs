using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.Host;
using ShaderTools.CodeAnalysis.Options;
using ShaderTools.CodeAnalysis.Properties;
using ShaderTools.CodeAnalysis.Text;
using ShaderTools.Utilities.Collections;
using ShaderTools.Utilities.ErrorReporting;
using ShaderTools.Utilities.Threading;

namespace ShaderTools.CodeAnalysis
{
    /// <summary>
    /// Contains the details and contents of an open document.
    /// </summary>
    public sealed class Document
    {
        private readonly HostLanguageServices _languageServices;
        
        /// <summary>
        /// Gets a unique string that identifies this file.  At this time,
        /// this property returns a normalized version of the value stored
        /// in the FilePath property.
        /// </summary>
        public DocumentId Id { get; }

        public SourceText SourceText { get; }

        /// <summary>
        /// Gets the path at which this file resides.
        /// </summary>
        public string FilePath { get; }

        public string Name => (FilePath != null) ? Path.GetFileName(FilePath) : "[NoName]";

        public string Language => _languageServices.Language;

        public HostLanguageServices LanguageServices => _languageServices;

        public Workspace Workspace => _languageServices.WorkspaceServices.Workspace;

        public ImmutableArray<LogicalDocument> LogicalDocuments { get; }

        internal Document(
            HostLanguageServices languageServices, 
            DocumentId documentId, SourceText sourceText, string filePath,
            Document parent, SourceRange? rangeInParent, TextSpan? textSpanInParent)
        {
            _languageServices = languageServices;

            Id = documentId;
            SourceText = sourceText;
            FilePath = filePath;

            var logicalDocumentFactory = languageServices.GetRequiredService<ILogicalDocumentFactory>();
            LogicalDocuments = logicalDocumentFactory.GetLogicalDocuments(this).ToImmutableArray();
        }

        public Document WithId(DocumentId documentId)
        {
            return new Document(_languageServices, documentId, SourceText, FilePath, null, null, null);
        }

        /// <summary>
        /// Creates a new instance of this document updated to have the text specified.
        /// </summary>
        public Document WithText(SourceText newText)
        {
            return new Document(_languageServices, Id, newText, FilePath, null, null, null);
        }

        public Document WithFilePath(string filePath)
        {
            return new Document(_languageServices, Id, SourceText, filePath, null, null, null);
        }

        /// <summary>
        /// Get the text changes between this document and a prior version of the same document.
        /// The changes, when applied to the text of the old document, will produce the text of the current document.
        /// </summary>
        public async Task<IEnumerable<TextChange>> GetTextChangesAsync(Document oldDocument, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                //using (Logger.LogBlock(FunctionId.Workspace_Document_GetTextChanges, this.Name, cancellationToken))
                {
                    if (oldDocument == this)
                    {
                        // no changes
                        return SpecializedCollections.EmptyEnumerable<TextChange>();
                    }

                    if (this.Id != oldDocument.Id)
                    {
                        throw new ArgumentException(WorkspacesResources.The_specified_document_is_not_a_version_of_this_document);
                    }

                    // first try to see if text already knows its changes
                    IList<TextChange> textChanges = null;
                    var text = this.SourceText;
                    var oldText = oldDocument.SourceText;

                    if (text == oldText)
                    {
                        return SpecializedCollections.EmptyEnumerable<TextChange>();
                    }

                    var container = text.Container;
                    if (container != null)
                    {
                        textChanges = text.GetTextChanges(oldText).ToList();

                        // if changes are significant (not the whole document being replaced) then use these changes
                        if (textChanges.Count > 1 || (textChanges.Count == 1 && textChanges[0].Span != new TextSpan(0, oldText.Length)))
                        {
                            return textChanges;
                        }
                    }

                    return text.GetTextChanges(oldText).ToList();
                }
            }
            catch (Exception e) when (FatalError.ReportUnlessCanceled(e))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private AsyncLazy<DocumentOptionSet> _cachedOptions;

        /// <summary>
        /// Returns the options that should be applied to this document. This consists of global options from <see cref="Solution.Options"/>,
        /// merged with any settings the user has specified at the document levels.
        /// </summary>
        /// <remarks>
        /// This method is async because this may require reading other files. In files that are already open, this is expected to be cheap and complete synchronously.
        /// </remarks>
        public Task<DocumentOptionSet> GetOptionsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_cachedOptions == null)
            {
                var newAsyncLazy = new AsyncLazy<DocumentOptionSet>(async c =>
                {
                    var optionsService = Workspace.Services.GetRequiredService<IOptionService>();
                    var optionSet = await optionsService.GetUpdatedOptionSetForDocumentAsync(this, Workspace.Options, c).ConfigureAwait(false);
                    return new DocumentOptionSet(optionSet, Language);
                }, cacheResult: true);

                Interlocked.CompareExchange(ref _cachedOptions, newAsyncLazy, comparand: null);
            }

            return _cachedOptions.GetValueAsync(cancellationToken);
        }

        public LogicalDocument GetLogicalDocument(TextSpan textSpan)
        {
            foreach (var logicalDocument in LogicalDocuments.Reverse())
            {
                if (logicalDocument.SpanInParentRootFile.Contains(textSpan))
                {
                    return logicalDocument;
                }
            }

            throw new InvalidOperationException();
        }
    }
}
