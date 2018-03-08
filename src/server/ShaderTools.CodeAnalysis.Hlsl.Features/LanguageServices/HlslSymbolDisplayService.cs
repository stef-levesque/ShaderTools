﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.Compilation;
using ShaderTools.CodeAnalysis.Host.Mef;
using ShaderTools.CodeAnalysis.LanguageServices;
using ShaderTools.CodeAnalysis.Symbols;

namespace ShaderTools.CodeAnalysis.Hlsl.LanguageServices
{
    [ExportLanguageService(typeof(ISymbolDisplayService), LanguageNames.Hlsl)]
    internal sealed class HlslSymbolDisplayService : ISymbolDisplayService
    {
        public Task<IDictionary<SymbolDescriptionGroups, ImmutableArray<TaggedText>>> ToDescriptionGroupsAsync(Workspace workspace, SemanticModelBase semanticModel, int position, ImmutableArray<ISymbol> symbols, CancellationToken cancellationToken)
        {
            return Task.FromResult((IDictionary<SymbolDescriptionGroups, ImmutableArray<TaggedText>>) new Dictionary<SymbolDescriptionGroups, ImmutableArray<TaggedText>>
            {
                { SymbolDescriptionGroups.MainDescription, symbols[0].ToMarkup().Tokens.ToTaggedText() }
            });
        }
    }
}
