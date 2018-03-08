using System.Collections.Immutable;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Symbols;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Hlsl.Symbols
{
    public sealed class ConstantBufferSymbol : ContainerSymbol, INamedTypeSymbol
    {
        public ConstantBufferSyntax Syntax { get; }

        public override SyntaxTreeBase SourceTree { get; }
        public override ImmutableArray<SourceRange> Locations { get; }
        public override ImmutableArray<SyntaxNodeBase> DeclaringSyntaxNodes { get; }

        internal ConstantBufferSymbol(ConstantBufferSyntax syntax, Symbol parent)
            : base(SymbolKind.ConstantBuffer, syntax.Name.Text, string.Empty, parent)
        {
            Syntax = syntax;

            SourceTree = syntax.SyntaxTree;
            Locations = ImmutableArray.Create(syntax.Name.SourceRange);
            DeclaringSyntaxNodes = ImmutableArray.Create((SyntaxNodeBase) syntax);
        }
    }
}