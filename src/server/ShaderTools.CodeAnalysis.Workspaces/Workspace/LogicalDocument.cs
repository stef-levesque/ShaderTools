using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.Compilation;
using ShaderTools.CodeAnalysis.Host;
using ShaderTools.CodeAnalysis.Options;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;
using ShaderTools.Utilities.Diagnostics;
using ShaderTools.Utilities.Threading;

namespace ShaderTools.CodeAnalysis
{
    public sealed class LogicalDocument
    {
        private readonly HostLanguageServices _languageServices;

        private readonly AsyncLazy<SyntaxTreeBase> _lazySyntaxTree;
        private readonly AsyncLazy<SemanticModelBase> _lazySemanticModel;

        public Document Parent { get; }

        /// <summary>
        /// Text span of this logical document in the parent document's root file.
        /// </summary>
        public TextSpan SpanInParentRootFile { get; }

        public SourceText SourceText { get; }

        public string Language => _languageServices.Language;

        public HostLanguageServices LanguageServices => _languageServices;

        public Workspace Workspace => _languageServices.WorkspaceServices.Workspace;

        public bool SupportsSemanticModel => LanguageServices.GetService<ICompilationFactoryService>() != null;

        internal LogicalDocument(
            SourceText sourceText,
            string language,
            Document parent, 
            TextSpan spanInParentRootFile)
        {
            Parent = parent;
            SpanInParentRootFile = spanInParentRootFile;
            SourceText = sourceText;

            _languageServices = parent.Workspace.Services.GetLanguageServices(language);

            _lazySyntaxTree = new AsyncLazy<SyntaxTreeBase>(ct => Task.Run(() =>
            {
                var syntaxTreeFactory = _languageServices.GetRequiredService<ISyntaxTreeFactoryService>();

                var syntaxTree = syntaxTreeFactory.ParseSyntaxTree(sourceText, ct);

                // make sure there is an association between this tree and this doc id before handing it out
                BindSyntaxTreeToId(syntaxTree, this);

                return syntaxTree;
            }, ct), true);

            _lazySemanticModel = new AsyncLazy<SemanticModelBase>(ct => Task.Run(async () =>
            {
                var syntaxTree = await GetSyntaxTreeAsync(ct).ConfigureAwait(false);

                var compilationFactory = _languageServices.GetRequiredService<ICompilationFactoryService>();

                return compilationFactory.CreateCompilation(syntaxTree).GetSemanticModelBase(ct);
            }, ct), true);
        }

        public Task<SyntaxTreeBase> GetSyntaxTreeAsync(CancellationToken cancellationToken)
        {
            return _lazySyntaxTree.GetValueAsync(cancellationToken);
        }

        internal async Task<SyntaxNodeBase> GetSyntaxRootAsync(CancellationToken cancellationToken)
        {
            var syntaxTree = await GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
            return syntaxTree.Root;
        }

        /// <summary>
        /// Only for features that absolutely must run synchronously (probably because they're
        /// on the UI thread).  Right now, the only feature this is for is Outlining as VS will
        /// block on that feature from the UI thread when a document is opened.
        /// </summary>
        internal SyntaxNodeBase GetSyntaxRootSynchronously(CancellationToken cancellationToken)
        {
            var tree = GetSyntaxTreeSynchronously(cancellationToken);
            return tree.Root;
        }

        internal SyntaxTreeBase GetSyntaxTreeSynchronously(CancellationToken cancellationToken)
        {
            return _lazySyntaxTree.GetValue(cancellationToken);
        }

        /// <summary>
        /// Get the current syntax tree for the document if the text is already loaded and the tree is already parsed.
        /// In almost all cases, you should call <see cref="GetSyntaxTreeAsync"/> to fetch the tree, which will parse the tree
        /// if it's not already parsed.
        /// </summary>
        public bool TryGetSyntaxTree(out SyntaxTreeBase syntaxTree)
        {
            return _lazySyntaxTree.TryGetValue(out syntaxTree);
        }

        public async Task<SemanticModelBase> GetSemanticModelAsync(CancellationToken cancellationToken)
        {
            if (!SupportsSemanticModel)
                return null;

            var options = await Parent.GetOptionsAsync(cancellationToken).ConfigureAwait(false);

            if (!options.GetOption(FeatureOnOffOptions.IntelliSense))
                return null;

            return await _lazySemanticModel.GetValueAsync(cancellationToken);
        }

        private static readonly ReaderWriterLockSlim s_syntaxTreeToIdMapLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private static readonly ConditionalWeakTable<SyntaxTreeBase, LogicalDocument> s_syntaxTreeToIdMap =
            new ConditionalWeakTable<SyntaxTreeBase, LogicalDocument>();

        private static void BindSyntaxTreeToId(SyntaxTreeBase tree, LogicalDocument id)
        {
            using (s_syntaxTreeToIdMapLock.DisposableWrite())
            {
                if (s_syntaxTreeToIdMap.TryGetValue(tree, out var existingId))
                {
                    Contract.ThrowIfFalse(existingId == id);
                }
                else
                {
                    s_syntaxTreeToIdMap.Add(tree, id);
                }
            }
        }

        internal static LogicalDocument GetDocumentIdForTree(SyntaxTreeBase tree)
        {
            using (s_syntaxTreeToIdMapLock.DisposableRead())
            {
                s_syntaxTreeToIdMap.TryGetValue(tree, out var id);
                return id;
            }
        }
    }
}
