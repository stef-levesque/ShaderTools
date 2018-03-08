﻿using System.Collections.Generic;
using System.Collections.Immutable;
using ShaderTools.CodeAnalysis.Hlsl.Symbols.Markup;
using ShaderTools.CodeAnalysis.Symbols;
using ShaderTools.CodeAnalysis.Symbols.Markup;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Hlsl.Symbols
{
    public abstract class Symbol : ISymbol
    {
        public SymbolKind Kind { get; }
        public string Name { get; }
        public string Documentation { get; }
        public Symbol Parent { get; }

        ISymbol ISymbol.Parent => Parent;

        internal Symbol(SymbolKind kind, string name, string documentation, Symbol parent)
        {
            Kind = kind;
            Name = name;
            Documentation = documentation;
            Parent = parent;
        }

        public abstract SyntaxTreeBase SourceTree { get; }
        public abstract ImmutableArray<SourceRange> Locations { get; }
        public abstract ImmutableArray<SyntaxNodeBase> DeclaringSyntaxNodes { get; }

        public sealed override string ToString()
        {
            return ToMarkup().ToString();
        }

        public SymbolMarkup ToMarkup(SymbolDisplayFormat format = SymbolDisplayFormat.QuickInfo)
        {
            var nodes = new List<SymbolMarkupToken>();
            nodes.AppendSymbol(this, format);
            return new SymbolMarkup(nodes);
        }

        protected bool EqualsImpl(Symbol other)
        {
            return Kind == other.Kind
                && string.Equals(Name, other.Name)
                && (Parent == null) == (other.Parent == null)
                && (Parent == null || Parent.EqualsImpl(other.Parent));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return EqualsImpl((Symbol) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Kind;
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Parent?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}