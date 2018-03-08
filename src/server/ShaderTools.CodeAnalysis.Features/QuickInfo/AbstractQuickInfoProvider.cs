﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.Shared.Extensions;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.QuickInfo
{
    internal abstract class AbstractQuickInfoProvider : IQuickInfoProvider
    {
        public async Task<QuickInfoItem> GetItemAsync(
            Document document,
            int position,
            CancellationToken cancellationToken)
        {
            var tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
            var sourceLocation = tree.MapRootFilePosition(position);
            var token = await tree.GetTouchingTokenAsync(sourceLocation, cancellationToken, findInsideTrivia: true).ConfigureAwait(false);

            if (token == null)
            {
                return null;
            }

            var state = await GetQuickInfoItemAsync(document, tree, token, sourceLocation, cancellationToken).ConfigureAwait(false);
            if (state != null)
            {
                return state;
            }

            if (ShouldCheckPreviousToken(token))
            {
                var previousToken = token.GetPreviousToken();

                if ((state = await GetQuickInfoItemAsync(document, tree, previousToken, sourceLocation, cancellationToken).ConfigureAwait(false)) != null)
                {
                    return state;
                }
            }

            return null;
        }

        protected virtual bool ShouldCheckPreviousToken(ISyntaxToken token)
        {
            return true;
        }

        private async Task<QuickInfoItem> GetQuickInfoItemAsync(
            Document document,
            SyntaxTreeBase syntaxTree,
            ISyntaxToken token,
            SourceLocation sourceLocation,
            CancellationToken cancellationToken)
        {
            if (token != default(ISyntaxToken) &&
                token.SourceRange.Contains(sourceLocation))
            {
                var deferredContent = await BuildContentAsync(document, token, cancellationToken).ConfigureAwait(false);
                if (deferredContent != null)
                {
                    var rootSpan = syntaxTree.GetSourceFileSpan(token.SourceRange).Span;
                    return new QuickInfoItem(rootSpan, deferredContent);
                }
            }

            return null;
        }

        protected abstract Task<QuickInfoContent> BuildContentAsync(Document document, ISyntaxToken token, CancellationToken cancellationToken);
    }
}
