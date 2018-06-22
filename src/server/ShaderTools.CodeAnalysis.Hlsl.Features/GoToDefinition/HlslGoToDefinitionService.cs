using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.FindUsages;
using ShaderTools.CodeAnalysis.GoToDefinition;
using ShaderTools.CodeAnalysis.Hlsl.LanguageServices;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Hlsl.Text;
using ShaderTools.CodeAnalysis.Host.Mef;
using ShaderTools.CodeAnalysis.Shared.Extensions;
using ShaderTools.CodeAnalysis.Syntax;

namespace ShaderTools.CodeAnalysis.Hlsl.GoToDefinition
{
    [ExportLanguageService(typeof(IGoToDefinitionService), LanguageNames.Hlsl)]
    internal sealed class HlslGoToDefinitionService : AbstractGoToDefinitionService
    {
        protected override async Task<ImmutableArray<DefinitionItem>> GetSyntacticDefinitionsAsync(LogicalDocument document, int position, CancellationToken cancellationToken)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
            var sourceLocation = syntaxTree.MapRootFilePosition(position);
            var syntaxFacts = document.GetLanguageService<ISyntaxFactsService>();
            var syntaxToken = (SyntaxToken)await syntaxTree.GetTouchingTokenAsync(sourceLocation, x => true, cancellationToken, findInsideTrivia: true).ConfigureAwait(false);

            if (syntaxToken == null)
            {
                return ImmutableArray<DefinitionItem>.Empty;
            }

            if (syntaxToken.MacroReference != null)
            {
                return GetMacroDefinitionItem(document, sourceLocation, syntaxToken);
            }

            if (syntaxToken.Parent.IsKind(SyntaxKind.IncludeDirectiveTrivia))
            {
                return GetIncludeDefinitionItem(document, syntaxToken);
            }

            return ImmutableArray<DefinitionItem>.Empty;
        }

        private static ImmutableArray<DefinitionItem> GetMacroDefinitionItem(LogicalDocument document, CodeAnalysis.Text.SourceLocation sourceLocation, SyntaxToken syntaxToken)
        {
            var nameToken = syntaxToken.MacroReference.NameToken;

            if (!nameToken.SourceRange.ContainsOrTouches(sourceLocation))
            {
                return ImmutableArray<DefinitionItem>.Empty;
            }

            if (!nameToken.FileSpan.IsInRootFile)
            {
                return ImmutableArray<DefinitionItem>.Empty;
            }

            var definitionItem = DefinitionItem.Create(
                ImmutableArray<string>.Empty,
                ImmutableArray<TaggedText>.Empty,
                new DocumentSpan(document, syntaxToken.MacroReference.DefineDirective.MacroName.FileSpan),
                ImmutableArray<TaggedText>.Empty);

            return ImmutableArray.Create(definitionItem);
        }

        private static ImmutableArray<DefinitionItem> GetIncludeDefinitionItem(LogicalDocument document, SyntaxToken syntaxToken)
        {
            var includeDirective = (IncludeDirectiveTriviaSyntax) syntaxToken.Parent;

            var syntaxTree = document.GetSyntaxTreeSynchronously(CancellationToken.None);

            var workspace = document.Workspace;

            var includeFileSystem = workspace.Services.GetRequiredService<IWorkspaceIncludeFileSystem>();

            var parseOptions = (HlslParseOptions) syntaxTree.Options;
            var includeFileResolver = new IncludeFileResolver(includeFileSystem, parseOptions);

            var currentFile = ((SyntaxTree) syntaxTree).File;

            var include = includeFileResolver.OpenInclude(includeDirective.TrimmedFilename, currentFile);
            if (include == null)
            {
                return ImmutableArray<DefinitionItem>.Empty;
            }

            var fileSpan = new CodeAnalysis.Text.SourceFileSpan(include, new CodeAnalysis.Text.TextSpan(0, 0));

            var definitionItem = DefinitionItem.Create(
                ImmutableArray<string>.Empty,
                ImmutableArray<TaggedText>.Empty,
                new DocumentSpan(document, fileSpan),
                ImmutableArray<TaggedText>.Empty);

            return ImmutableArray.Create(definitionItem);
        }
    }
}
