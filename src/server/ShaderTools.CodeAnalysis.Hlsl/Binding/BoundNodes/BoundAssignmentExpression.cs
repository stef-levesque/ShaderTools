using ShaderTools.CodeAnalysis.Hlsl.Symbols;

namespace ShaderTools.CodeAnalysis.Hlsl.Binding.BoundNodes
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public override TypeSymbol Type { get; }

        public BoundExpression Left { get; }
        public BinaryOperatorKind? OperatorKind { get; }
        public BoundExpression Right { get; }

        public BoundAssignmentExpression(BoundExpression left, BinaryOperatorKind? operatorKind, BoundExpression right)
            : base(BoundNodeKind.AssignmentExpression)
        {
            OperatorKind = operatorKind;
            Left = left;
            Right = right;
            Type = left.Type;
        }
    }
}