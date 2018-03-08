﻿using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.Host;

namespace ShaderTools.CodeAnalysis.Structure
{
    internal interface IBlockStructureProvider : ILanguageService
    {
        Task<ImmutableArray<BlockSpan>> ProvideBlockStructureAsync(Document document, CancellationToken cancellationToken);
    }
}
