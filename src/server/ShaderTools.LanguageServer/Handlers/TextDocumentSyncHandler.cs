using System;
using System.Linq;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace ShaderTools.LanguageServer.Handlers
{
    internal sealed class TextDocumentSyncHandler : ITextDocumentSyncHandler
    {
        private readonly LanguageServerWorkspace _workspace;
        private readonly TextDocumentRegistrationOptions _registrationOptions;

        public TextDocumentSyncOptions Options { get; } = new TextDocumentSyncOptions
        {
            Change = TextDocumentSyncKind.Incremental,
            OpenClose = true,
            WillSave = false,
            WillSaveWaitUntil = false
        };

        public TextDocumentSyncHandler(LanguageServerWorkspace workspace, TextDocumentRegistrationOptions registrationOptions)
        {
            _workspace = workspace;
            _registrationOptions = registrationOptions;
        }

        public TextDocumentChangeRegistrationOptions GetRegistrationOptions() => new TextDocumentChangeRegistrationOptions
        {
            DocumentSelector = _registrationOptions.DocumentSelector,
            SyncKind = TextDocumentSyncKind.Incremental
        };

        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            return new TextDocumentAttributes(uri, string.Empty);
        }

        public Task Handle(DidChangeTextDocumentParams notification)
        {
            var document = _workspace.GetDocument(notification.TextDocument.Uri);

            if (document == null)
            {
                return Task.CompletedTask;
            }

            _workspace.UpdateDocument(
                document,
                notification.ContentChanges.Select(x =>
                    Helpers.ToTextChange(
                        document,
                        x.Range,
                        x.Text)));

            return Task.CompletedTask;
        }

        public Task Handle(DidOpenTextDocumentParams notification)
        {
            _workspace.OpenDocument(
                notification.TextDocument.Uri,
                notification.TextDocument.Text,
                notification.TextDocument.LanguageId);

            return Task.CompletedTask;
        }

        public Task Handle(DidCloseTextDocumentParams notification)
        {
            var document = _workspace.GetDocument(notification.TextDocument.Uri);

            if (document != null)
            {
                _workspace.CloseDocument(document.Id);
            }

            return Task.CompletedTask;
        }

        public Task Handle(DidSaveTextDocumentParams notification) => Task.CompletedTask;

        public void SetCapability(SynchronizationCapability capability) { }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions() => new TextDocumentSaveRegistrationOptions();
    }
}
