﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ShaderTools.CodeAnalysis.Symbols;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Hlsl.Symbols
{
    public class FunctionSymbol : InvocableSymbol
    {
        public override SyntaxTreeBase SourceTree { get; } = null;
        public override ImmutableArray<SourceRange> Locations => ImmutableArray<SourceRange>.Empty;
        public override ImmutableArray<SyntaxNodeBase> DeclaringSyntaxNodes => ImmutableArray<SyntaxNodeBase>.Empty;

        internal FunctionSymbol(string name, string documentation, Symbol parent, TypeSymbol returnType, Func<InvocableSymbol, IEnumerable<ParameterSymbol>> lazyParameters = null, bool isNumericConstructor = false)
            : base(SymbolKind.Function, name, documentation, parent, returnType, lazyParameters)
        {
            IsNumericConstructor = isNumericConstructor;
        }

        public bool IsNumericConstructor { get; }
    }
}