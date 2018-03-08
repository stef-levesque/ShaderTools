namespace ShaderTools.CodeAnalysis.Hlsl.Syntax
{
    public class FunctionInvocationExpressionSyntax : InvocationExpressionSyntax
    {
        public readonly NameSyntax Name;
        public readonly ArgumentListSyntax ArgumentList;

        public override ArgumentListSyntax ArgumentListSyntax => ArgumentList;

        public FunctionInvocationExpressionSyntax(NameSyntax name, ArgumentListSyntax argumentList)
            : base(SyntaxKind.FunctionInvocationExpression)
        {
            RegisterChildNode(out Name, name);
            RegisterChildNode(out ArgumentList, argumentList);
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            visitor.VisitFunctionInvocationExpression(this);
        }

        public override T Accept<T>(SyntaxVisitor<T> visitor)
        {
            return visitor.VisitFunctionInvocationExpression(this);
        }
    }
}