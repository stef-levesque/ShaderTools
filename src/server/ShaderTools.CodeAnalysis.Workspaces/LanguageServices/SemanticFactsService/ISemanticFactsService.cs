﻿using System.Threading;
using ShaderTools.CodeAnalysis.Compilation;
using ShaderTools.CodeAnalysis.Host;
using ShaderTools.CodeAnalysis.Symbols;
using ShaderTools.CodeAnalysis.Syntax;

namespace ShaderTools.CodeAnalysis.LanguageServices
{
    internal interface ISemanticFactsService : ILanguageService
    {
        ISymbol GetDeclaredSymbol(SemanticModelBase semanticModel, ISyntaxToken token, CancellationToken cancellationToken);
    }
}
