using ShaderTools.CodeAnalysis.Hlsl.Binding.Signatures;
using ShaderTools.CodeAnalysis.Hlsl.Symbols;

namespace ShaderTools.CodeAnalysis.Hlsl.Binding.BoundNodes
{
    internal sealed class BoundElementAccessExpression : BoundExpression
    {
        public override TypeSymbol Type { get; }

        public BoundExpression Expression { get; }
        public BoundExpression Index { get; }

        public IndexerSymbol Symbol => Result.Selected?.Signature.Symbol;

        public OverloadResolutionResult<IndexerSymbolSignature> Result { get; }

        public BoundElementAccessExpression(BoundExpression expression, BoundExpression index, OverloadResolutionResult<IndexerSymbolSignature> result)
            : base(BoundNodeKind.ElementAccessExpression)
        {
            Expression = expression;
            Index = index;
            Result = result;
            Type = Symbol?.ValueType ?? TypeFacts.Unknown;
        }
    }
}