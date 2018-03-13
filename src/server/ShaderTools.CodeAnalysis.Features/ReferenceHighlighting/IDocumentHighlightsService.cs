using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.Host;

namespace ShaderTools.CodeAnalysis.ReferenceHighlighting
{
    internal interface IDocumentHighlightsService : IWorkspaceService
    {
        Task<ImmutableArray<DocumentHighlights>> GetDocumentHighlightsAsync(
            LogicalDocument document, int position, IImmutableSet<LogicalDocument> documentsToSearch, CancellationToken cancellationToken);
    }
}
