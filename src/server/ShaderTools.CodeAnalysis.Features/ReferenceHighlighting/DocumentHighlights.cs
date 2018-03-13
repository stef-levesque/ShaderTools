using System.Collections.Immutable;

namespace ShaderTools.CodeAnalysis.ReferenceHighlighting
{
    internal struct DocumentHighlights
    {
        public LogicalDocument Document { get; }
        public ImmutableArray<HighlightSpan> HighlightSpans { get; }

        public DocumentHighlights(LogicalDocument document, ImmutableArray<HighlightSpan> highlightSpans)
        {
            Document = document;
            HighlightSpans = highlightSpans;
        }
    }
}
