namespace ShaderTools.CodeAnalysis.Hlsl.Syntax
{
    public abstract class SyntaxVisitor
    {
        public virtual void Visit(SyntaxNode node)
        {
            node?.Accept(this);
        }

        protected virtual void DefaultVisit(SyntaxNode node)
        {

        }

        public virtual void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitNamespace(NamespaceSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitSamplerStateInitializer(SamplerStateInitializerSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitSyntaxTrivia(SyntaxTrivia node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitSkippedTokensSyntaxTrivia(SkippedTokensTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitTemplateArgumentList(TemplateArgumentListSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitArgumentList(ArgumentListSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitParameterList(ParameterListSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitMacroArgumentList(MacroArgumentListSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitAttribute(AttributeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitAnnotations(AnnotationsSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitVariableDeclarationStatement(VariableDeclarationStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitTypeDeclarationStatement(TypeDeclarationStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitTypedefStatement(TypedefStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitTypeAlias(TypeAliasSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitIdentifierName(IdentifierNameSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitQualifiedName(QualifiedNameSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitIdentifierDeclarationName(IdentifierDeclarationNameSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitQualifiedDeclarationName(QualifiedDeclarationNameSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitStructType(StructTypeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitInterfaceType(InterfaceTypeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitConstantBuffer(ConstantBufferSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitTechnique(TechniqueSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitPass(PassSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitFunctionDeclaration(FunctionDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitFunctionDefinition(FunctionDefinitionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitParameter(ParameterSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitBlock(BlockSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitEmptyStatement(EmptyStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitReturnStatement(ReturnStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDiscardStatement(DiscardStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitBreakStatement(BreakStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitContinueStatement(ContinueStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitIfStatement(IfStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitForStatement(ForStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitWhileStatement(WhileStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDoStatement(DoStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitSwitchSection(SwitchSectionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitCompoundExpression(CompoundExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitPrefixCastExpression(CastExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitStringLiteralExpression(StringLiteralExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitFieldAccess(FieldAccessExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitFunctionInvocationExpression(FunctionInvocationExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitMethodInvocationExpression(MethodInvocationExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitCompileExpression(CompileExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitArrayInitializerExpression(ArrayInitializerExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitRegisterLocation(RegisterLocation node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitLogicalRegisterSpace(LogicalRegisterSpace node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitPackOffsetLocation(PackOffsetLocation node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitPackOffsetComponentPart(PackOffsetComponentPart node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitScalarType(ScalarTypeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitMatrixType(MatrixTypeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitGenericMatrixType(GenericMatrixTypeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitPredefinedObjectType(PredefinedObjectTypeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitVectorType(VectorTypeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitGenericVectorType(GenericVectorTypeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitStateInitializer(StateInitializerSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitStateArrayInitializer(StateArrayInitializerSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitStateProperty(StatePropertySyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitSyntaxToken(SyntaxToken node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitSemantic(SemanticSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitNumericConstructorInvocation(NumericConstructorInvocationExpressionSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitBaseList(BaseListSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitElseClause(ElseClauseSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitMacroArgument(MacroArgumentSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitIfDefDirectiveTrivia(IfDefDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitIfNDefDirectiveTrivia(IfNDefDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitObjectLikeDefineDirectiveTrivia(ObjectLikeDefineDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitFunctionLikeDefineDirectiveTrivia(FunctionLikeDefineDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitFunctionLikeDefineDirectiveParameterList(FunctionLikeDefineDirectiveParameterListSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitIncludeDirectiveTrivia(IncludeDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitPragmaDirectiveTrivia(PragmaDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitObjectLikeMacroReference(ObjectLikeMacroReference node)
        {
            
        }

        public virtual void VisitFunctionLikeMacroReference(FunctionLikeMacroReference node)
        {

        }
    }

    public abstract class SyntaxVisitor<T>
    {
        public virtual T Visit(SyntaxNode node)
        {
            if (node != null)
                return node.Accept(this);
            return default(T);
        }

        protected virtual T DefaultVisit(SyntaxNode node)
        {
            return default(T);
        }

        public virtual T VisitCompilationUnit(CompilationUnitSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitNamespace(NamespaceSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSamplerStateInitializer(SamplerStateInitializerSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSyntaxTrivia(SyntaxTrivia node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSkippedTokensSyntaxTrivia(SkippedTokensTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTemplateArgumentList(TemplateArgumentListSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitArgumentList(ArgumentListSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitParameterList(ParameterListSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitMacroArgumentList(MacroArgumentListSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAttribute(AttributeSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAnnotations(AnnotationsSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitVariableDeclarationStatement(VariableDeclarationStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTypeDeclarationStatement(TypeDeclarationStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTypedefStatement(TypedefStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTypeAlias(TypeAliasSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIdentifierName(IdentifierNameSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQualifiedName(QualifiedNameSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIdentifierDeclarationName(IdentifierDeclarationNameSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitQualifiedDeclarationName(QualifiedDeclarationNameSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitStructType(StructTypeSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitInterfaceType(InterfaceTypeSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConstantBuffer(ConstantBufferSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitTechnique(TechniqueSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPass(PassSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitFunctionDeclaration(FunctionDeclarationSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitFunctionDefinition(FunctionDefinitionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitParameter(ParameterSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitBlock(BlockSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitEmptyStatement(EmptyStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitReturnStatement(ReturnStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDiscardStatement(DiscardStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitBreakStatement(BreakStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitContinueStatement(ContinueStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIfStatement(IfStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitForStatement(ForStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitWhileStatement(WhileStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDoStatement(DoStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSwitchStatement(SwitchStatementSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSwitchSection(SwitchSectionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitCompoundExpression(CompoundExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPrefixCastExpression(CastExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitStringLiteralExpression(StringLiteralExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitFieldAccess(FieldAccessExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitMethodInvocationExpression(MethodInvocationExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitFunctionInvocationExpression(FunctionInvocationExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitCompileExpression(CompileExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitArrayInitializerExpression(ArrayInitializerExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitRegisterLocation(RegisterLocation node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitLogicalRegisterSpace(LogicalRegisterSpace node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPackOffsetLocation(PackOffsetLocation node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPackOffsetComponentPart(PackOffsetComponentPart node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitScalarType(ScalarTypeSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitMatrixType(MatrixTypeSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitGenericMatrixType(GenericMatrixTypeSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPredefinedObjectType(PredefinedObjectTypeSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitVectorType(VectorTypeSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitGenericVectorType(GenericVectorTypeSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitStateInitializer(StateInitializerSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitStateArrayInitializer(StateArrayInitializerSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitStateProperty(StatePropertySyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSyntaxToken(SyntaxToken node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitSemantic(SemanticSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitNumericConstructorInvocation(NumericConstructorInvocationExpressionSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitBaseList(BaseListSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitElseClause(ElseClauseSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitMacroArgument(MacroArgumentSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIfDefDirectiveTrivia(IfDefDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIfNDefDirectiveTrivia(IfNDefDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitObjectLikeDefineDirectiveTrivia(ObjectLikeDefineDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitFunctionLikeDefineDirectiveTrivia(FunctionLikeDefineDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitFunctionLikeDefineDirectiveParameterList(FunctionLikeDefineDirectiveParameterListSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitIncludeDirectiveTrivia(IncludeDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitPragmaDirectiveTrivia(PragmaDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            return DefaultVisit(node);
        }

        public virtual T VisitObjectLikeMacroReference(ObjectLikeMacroReference node)
        {
            return default(T);
        }

        public virtual T VisitFunctionLikeMacroReference(FunctionLikeMacroReference node)
        {
            return default(T);
        }
    }
}