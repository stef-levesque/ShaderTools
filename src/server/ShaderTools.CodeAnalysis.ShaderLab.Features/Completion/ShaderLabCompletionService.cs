using System.Collections.Immutable;
using System.Composition;
using ShaderTools.CodeAnalysis.Completion;
using ShaderTools.CodeAnalysis.Host;
using ShaderTools.CodeAnalysis.Host.Mef;

namespace ShaderTools.CodeAnalysis.ShaderLab.Completion
{
    [ExportLanguageServiceFactory(typeof(CompletionService), LanguageNames.ShaderLab), Shared]
    internal class ShaderLabCompletionServiceFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices languageServices)
        {
            return new ShaderLabCompletionService(languageServices.WorkspaceServices.Workspace);
        }
    }

    internal class ShaderLabCompletionService : CommonCompletionService
    {
        private readonly ImmutableArray<CompletionProvider> _defaultCompletionProviders =
            ImmutableArray<CompletionProvider>.Empty;

        private readonly Workspace _workspace;

        public ShaderLabCompletionService(
            Workspace workspace, ImmutableArray<CompletionProvider>? exclusiveProviders = null)
            : base(workspace, exclusiveProviders)
        {
            _workspace = workspace;
        }

        public override string Language => LanguageNames.ShaderLab;
    }
}