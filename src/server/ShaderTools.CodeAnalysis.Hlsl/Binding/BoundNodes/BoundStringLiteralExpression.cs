﻿using System.Collections.Immutable;
using ShaderTools.CodeAnalysis.Hlsl.Symbols;

namespace ShaderTools.CodeAnalysis.Hlsl.Binding.BoundNodes
{
    internal sealed class BoundStringLiteralExpression : BoundExpression
    {
        public BoundStringLiteralExpression(ImmutableArray<string> values)
            : base(BoundNodeKind.StringLiteralExpression)
        {
            Type = IntrinsicTypes.String;
            Values = values;
        }

        public override TypeSymbol Type { get; }
        public ImmutableArray<string> Values { get; }
    }
}