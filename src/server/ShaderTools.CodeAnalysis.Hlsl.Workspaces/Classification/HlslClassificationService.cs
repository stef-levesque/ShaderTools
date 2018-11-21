// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using ShaderTools.CodeAnalysis.Classification;
using ShaderTools.CodeAnalysis.Compilation;
using ShaderTools.CodeAnalysis.Hlsl.Compilation;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Host.Mef;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Hlsl.Classification
{
    [ExportLanguageService(typeof(IClassificationService), LanguageNames.Hlsl), Shared]
    internal class ShaderLabClassificationService : AbstractClassificationService
    {
        public override void AddSemanticClassifications(SemanticModelBase semanticModel, TextSpan textSpan, Workspace workspace, List<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            var semanticTaggerVisitor = new SemanticTaggerVisitor((SemanticModel) semanticModel, result, cancellationToken);
            semanticTaggerVisitor.VisitCompilationUnit((CompilationUnitSyntax) semanticModel.SyntaxTree.Root);
        }
    }
}
