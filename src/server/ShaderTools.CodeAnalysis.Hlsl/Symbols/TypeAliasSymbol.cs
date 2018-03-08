using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Symbols;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Hlsl.Symbols
{
    public sealed class TypeAliasSymbol : TypeSymbol, IAliasSymbol
    {
        internal TypeAliasSymbol(TypeAliasSyntax syntax, TypeSymbol valueType)
            : base(SymbolKind.TypeAlias, syntax.Identifier.Text, string.Empty, null)
        {
            ValueType = valueType;
            Locations = ImmutableArray.Create(syntax.Identifier.SourceRange);
            SourceTree = syntax.SyntaxTree;
            DeclaringSyntaxNodes = ImmutableArray.Create((SyntaxNodeBase) syntax);
        }

        public TypeSymbol ValueType { get; }

        ITypeSymbol IAliasSymbol.Target => ValueType;

        public override ImmutableArray<SourceRange> Locations { get; }

        public override ImmutableArray<SyntaxNodeBase> DeclaringSyntaxNodes { get; }

        public override SyntaxTreeBase SourceTree { get; }

        public override IEnumerable<T> LookupMembers<T>(string name)
        {
            return ValueType.LookupMembers<T>(name);
        }
    }
}