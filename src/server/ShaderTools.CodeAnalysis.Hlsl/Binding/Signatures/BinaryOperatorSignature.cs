﻿using ShaderTools.CodeAnalysis.Hlsl.Binding.BoundNodes;
using ShaderTools.CodeAnalysis.Hlsl.Symbols;

namespace ShaderTools.CodeAnalysis.Hlsl.Binding.Signatures
{
    internal sealed class BinaryOperatorSignature : Signature
    {
        private readonly TypeSymbol _leftParameterType;
        private readonly TypeSymbol _rightParameterType;

        public BinaryOperatorSignature(BinaryOperatorKind kind, TypeSymbol returnType, TypeSymbol leftParameterType, TypeSymbol rightParameterType)
        {
            Kind = kind;
            ReturnType = returnType;
            _leftParameterType = leftParameterType;
            _rightParameterType = rightParameterType;
        }

        public BinaryOperatorSignature(BinaryOperatorKind kind, TypeSymbol type)
            : this(kind, type, type)
        {
        }

        public BinaryOperatorSignature(BinaryOperatorKind kind, TypeSymbol returnType, TypeSymbol parameterType)
            : this(kind, returnType, parameterType, parameterType)
        {
        }

        public override TypeSymbol ReturnType { get; }

        public override ParameterDirection GetParameterDirection(int index) => ParameterDirection.In;
        public override bool ParameterHasDefaultValue(int index) => false;

        public override TypeSymbol GetParameterType(int index)
        {
            return index == 0 ? _leftParameterType : _rightParameterType;
        }

        public override int ParameterCount => 2;
        public override bool HasVariadicParameter { get; } = false;

        public BinaryOperatorKind Kind { get; }

        public override string ToString()
        {
            return $"{Kind}({_leftParameterType.ToDisplayName()}, {_rightParameterType.ToDisplayName()}) AS {ReturnType.ToDisplayName()}";
        }
    }
}