using System.Runtime.InteropServices;
using System.Threading;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Hlsl.Text;
using ShaderTools.CodeAnalysis.Host;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Hlsl.LanguageServices
{
    internal sealed class HlslSyntaxTreeFactoryService : ISyntaxTreeFactoryService
    {
        private readonly Workspace _workspace;
        private readonly IIncludeFileSystem _fileSystem;

        public HlslSyntaxTreeFactoryService(Workspace workspace, IIncludeFileSystem fileSystem)
        {
            _workspace = workspace;
            _fileSystem = fileSystem;
        }

        public SyntaxTreeBase ParseSyntaxTree(SourceText text, CancellationToken cancellationToken)
        {
            var configFile = _workspace.LoadConfigFile(text);

            var options = new HlslParseOptions();
            options.PreprocessorDefines.Add("__INTELLISENSE__", "1");

            foreach (var kvp in configFile.HlslPreprocessorDefinitions)
                options.PreprocessorDefines.Add(kvp.Key, kvp.Value);

            options.AdditionalIncludeDirectories.AddRange(configFile.HlslAdditionalIncludeDirectories);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                options.AdditionalIncludeDirectories.Add(@"C:\Program Files\Unity\Editor\Data\CGIncludes");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                options.AdditionalIncludeDirectories.Add(@"/Applications/Unity/Unity.app/Contents/CGIncludes");
            }

            return SyntaxFactory.ParseSyntaxTree(
                text,
                options,
                _fileSystem,
                cancellationToken);
        }
    }
}
