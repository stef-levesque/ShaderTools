using System.Collections.Generic;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Host
{
    internal interface ILogicalDocumentFactory : ILanguageService
    {
        IEnumerable<LogicalDocument> GetLogicalDocuments(Document document);
    }
}
