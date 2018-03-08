﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ShaderTools.CodeAnalysis.Hlsl.Binding.Signatures;
using ShaderTools.CodeAnalysis.Hlsl.Symbols;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;

namespace ShaderTools.CodeAnalysis.Hlsl.Binding.BoundNodes
{
    internal sealed class BoundFunctionInvocationExpression : BoundExpression
    {
        public BoundFunctionInvocationExpression(FunctionInvocationExpressionSyntax syntax, ImmutableArray<BoundExpression> arguments, OverloadResolutionResult<FunctionSymbolSignature> result)
            : base(BoundNodeKind.FunctionInvocationExpression)
        {
            Syntax = syntax;
            Arguments = arguments;
            Result = result;
        }

        public override TypeSymbol Type => Symbol == null ? TypeFacts.Unknown : Symbol.ReturnType;

        public FunctionSymbol Symbol => Result.Selected?.Signature.Symbol ?? Result.Candidates.FirstOrDefault()?.Signature.Symbol;

        public FunctionInvocationExpressionSyntax Syntax { get; }
        public ImmutableArray<BoundExpression> Arguments { get; }
        public OverloadResolutionResult<FunctionSymbolSignature> Result { get; }

        public BoundFunctionInvocationExpression Update(IEnumerable<BoundExpression> arguments, OverloadResolutionResult<FunctionSymbolSignature> result)
        {
            var newArguments = arguments.ToImmutableArray();

            if (newArguments == Arguments && result == Result)
                return this;

            return new BoundFunctionInvocationExpression(Syntax, newArguments, result);
        }

        public override string ToString()
        {
            return $"{Symbol.Name}({string.Join(",", Arguments)})";
        }
    }
}