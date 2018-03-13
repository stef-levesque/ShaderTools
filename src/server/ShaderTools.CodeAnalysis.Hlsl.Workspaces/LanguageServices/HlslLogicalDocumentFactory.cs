using System.Collections.Generic;
using ShaderTools.CodeAnalysis.Host;
using ShaderTools.CodeAnalysis.Host.Mef;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Hlsl.LanguageServices
{
    [ExportLanguageService(typeof(ILogicalDocumentFactory), LanguageNames.Hlsl)]
    internal sealed class HlslLogicalDocumentFactory : ILogicalDocumentFactory
    {
        public IEnumerable<LogicalDocument> GetLogicalDocuments(Document document)
        {
            yield return new LogicalDocument(
                document.SourceText,
                document.Language,
                document,
                new TextSpan(0, document.SourceText.Length));
        }
    }
}
