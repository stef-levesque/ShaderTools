using System.ComponentModel.Composition;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;

namespace ShaderTools.VisualStudio.LanguageClients
{
    [Export(typeof(ILanguageClient))]
    [ContentType("hlsl")]
    internal sealed class HlslLanguageClient : LanguageClientBase
    {
        protected override string LanguageName => "HLSL";
    }
}
