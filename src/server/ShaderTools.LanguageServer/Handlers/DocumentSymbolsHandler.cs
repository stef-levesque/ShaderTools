using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ShaderTools.CodeAnalysis.NavigateTo;

namespace ShaderTools.LanguageServer.Handlers
{
    internal sealed class DocumentSymbolsHandler : IDocumentSymbolHandler
    {
        private readonly LanguageServerWorkspace _workspace;
        private readonly TextDocumentRegistrationOptions _registrationOptions;

        public DocumentSymbolsHandler(LanguageServerWorkspace workspace, TextDocumentRegistrationOptions registrationOptions)
        {
            _workspace = workspace;
            _registrationOptions = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _registrationOptions;

        public async Task<DocumentSymbolInformationContainer> Handle(DocumentSymbolParams request, CancellationToken token)
        {
            var document = _workspace.GetDocument(request.TextDocument.Uri);

            var searchService = _workspace.Services.GetService<INavigateToSearchService>();

            var symbols = ImmutableArray.CreateBuilder<DocumentSymbolInformation>();

            foreach (var logicalDocument in document.LogicalDocuments)
            {
                await Helpers.FindSymbolsInDocument(searchService, logicalDocument, string.Empty, token, symbols);
            }

            return new DocumentSymbolInformationContainer(symbols);
        }

        public void SetCapability(DocumentSymbolCapability capability) { }
    }
}
