using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ShaderTools.CodeAnalysis.NavigateTo;

namespace ShaderTools.LanguageServer.Handlers
{
    internal sealed class WorkspaceSymbolsHandler : IWorkspaceSymbolsHandler
    {
        private readonly LanguageServerWorkspace _workspace;

        public WorkspaceSymbolsHandler(LanguageServerWorkspace workspace)
        {
            _workspace = workspace;
        }

        public async Task<WorkspaceSymbolInformationContainer> Handle(WorkspaceSymbolParams request, CancellationToken token)
        {
            var searchService = _workspace.Services.GetService<INavigateToSearchService>();

            var symbols = ImmutableArray.CreateBuilder<WorkspaceSymbolInformation>();

            foreach (var document in _workspace.CurrentDocuments.Documents)
            {
                foreach (var logicalDocument in document.LogicalDocuments)
                {
                    await Helpers.FindSymbolsInDocument(searchService, logicalDocument, request.Query, token, symbols);
                }
            }

            return new WorkspaceSymbolInformationContainer(symbols);
        }

        public void SetCapability(WorkspaceSymbolCapability capability) { }
    }
}
