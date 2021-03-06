﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.Options;
using ShaderTools.CodeAnalysis.Text;
using ShaderTools.CodeAnalysis.Shared.Extensions;
using ShaderTools.CodeAnalysis.Symbols.Markup;

namespace ShaderTools.CodeAnalysis.Completion
{
    internal abstract class CommonCompletionProvider : CompletionProvider
    {
        public override bool ShouldTriggerCompletion(SourceText text, int position, CompletionTrigger trigger, OptionSet options)
        {
            switch (trigger.Kind)
            {
                case CompletionTriggerKind.Insertion:
                    var insertedCharacterPosition = position - 1;
                    return this.IsInsertionTrigger(text, insertedCharacterPosition, options);

                default:
                    return false;
            }
        }

        internal virtual bool IsInsertionTrigger(SourceText text, int insertedCharacterPosition, OptionSet options)
        {
            return false;
        }

        public sealed override async Task<CompletionDescription> GetDescriptionAsync(
            LogicalDocument document, CompletionItem item, CancellationToken cancellationToken)
        {
            // Get the actual description provided by whatever subclass we are.
            // Then, if we would commit text that could be expanded as a snippet, 
            // put that information in the description so that the user knows.
            var description = await this.GetDescriptionWorkerAsync(document, item, cancellationToken).ConfigureAwait(false);
            var parts = description.TaggedParts;

            return description.WithTaggedParts(parts);
        }

        protected virtual Task<CompletionDescription> GetDescriptionWorkerAsync(
            LogicalDocument document, CompletionItem item, CancellationToken cancellationToken)
        {
            return CommonCompletionItem.HasDescription(item)
                ? Task.FromResult(CommonCompletionItem.GetDescription(item))
                : Task.FromResult(CompletionDescription.Empty);
        }


        public override async Task<CompletionChange> GetChangeAsync(LogicalDocument document, CompletionItem item, char? commitKey = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var change = (await GetTextChangeAsync(document, item, commitKey, cancellationToken).ConfigureAwait(false))
                         ?? new TextChange(item.Span, item.DisplayText);
            return CompletionChange.Create(change);
        }

        public virtual Task<TextChange?> GetTextChangeAsync(LogicalDocument document, CompletionItem selectedItem, char? ch, CancellationToken cancellationToken)
        {
            return GetTextChangeAsync(selectedItem, ch, cancellationToken);
        }

        protected virtual Task<TextChange?> GetTextChangeAsync(CompletionItem selectedItem, char? ch, CancellationToken cancellationToken)
        {
            return Task.FromResult<TextChange?>(null);
        }

        private static CompletionItemRules s_suggestionItemRules = CompletionItemRules.Create(enterKeyRule: EnterKeyRule.Never);

        protected CompletionItem CreateSuggestionModeItem(string displayText, string description)
        {
            return CommonCompletionItem.Create(
                displayText: displayText ?? string.Empty,
                description: description != null ? description.ToSymbolMarkupTokens() : default(ImmutableArray<SymbolMarkupToken>),
                rules: s_suggestionItemRules);
        }
    }
}