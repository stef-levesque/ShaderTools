// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using ShaderTools.CodeAnalysis.Compilation;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Classification
{
    internal abstract partial class AbstractClassificationService : IClassificationService
    {
        protected AbstractClassificationService()
        {
        }

        public abstract void AddSemanticClassifications(SemanticModelBase semanticModel, TextSpan textSpan, Workspace workspace, List<ClassifiedSpan> result, CancellationToken cancellationToken);
    }
}
