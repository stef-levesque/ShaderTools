using System.Collections.Immutable;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Symbols;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Hlsl.Symbols
{
    public sealed class NamespaceSymbol : ContainerSymbol
    {
        public NamespaceSyntax Syntax { get; }

        public override SyntaxTreeBase SourceTree => Syntax.SyntaxTree;
        public override ImmutableArray<SourceRange> Locations { get; }
        public override ImmutableArray<SyntaxNodeBase> DeclaringSyntaxNodes { get; }

        internal NamespaceSymbol(NamespaceSyntax syntax, Symbol parent)
            : base(SymbolKind.Namespace, syntax.Name.Text, string.Empty, parent)
        {
            Syntax = syntax;

            Locations = ImmutableArray.Create(syntax.Name.SourceRange);
            DeclaringSyntaxNodes = ImmutableArray.Create((SyntaxNodeBase) syntax);
        }
    }
}