﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using ShaderTools.CodeAnalysis.Completion;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Options;
using ShaderTools.CodeAnalysis.Symbols;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.Hlsl.Completion.Providers
{
    internal static class CompletionUtilities
    {
        internal static TextSpan GetCompletionItemSpan(SourceText text, int position)
        {
            return CommonCompletionUtilities.GetWordSpan(text, position, IsCompletionItemStartCharacter, IsWordCharacter);
        }

        public static bool IsWordStartCharacter(char ch)
        {
            return SyntaxFacts.IsIdentifierStartCharacter(ch);
        }

        public static bool IsWordCharacter(char ch)
        {
            return SyntaxFacts.IsIdentifierStartCharacter(ch) || SyntaxFacts.IsIdentifierPartCharacter(ch);
        }

        public static bool IsCompletionItemStartCharacter(char ch)
        {
            return ch == '@' || IsWordCharacter(ch);
        }

        internal static bool IsTriggerCharacter(SourceText text, int characterPosition, OptionSet options)
        {
            var ch = text[characterPosition];
            if (ch == '.')
            {
                return true;
            }

            // Trigger for directive
            if (ch == '#')
            {
                return true;
            }

            // Trigger on pointer member access
            if (ch == '>' && characterPosition >= 1 && text[characterPosition - 1] == '-')
            {
                return true;
            }

            // Trigger on alias name
            if (ch == ':' && characterPosition >= 1 && text[characterPosition - 1] == ':')
            {
                return true;
            }

            if (options.GetOption(CompletionOptions.TriggerOnTypingLetters, LanguageNames.Hlsl) && IsStartingNewWord(text, characterPosition))
            {
                return true;
            }

            return false;
        }

        internal static bool IsTriggerAfterSpaceOrStartOfWordCharacter(SourceText text, int characterPosition, OptionSet options)
        {
            // Bring up on space or at the start of a word.
            var ch = text[characterPosition];
            return SpaceTypedNotBeforeWord(ch, text, characterPosition) ||
                   (IsStartingNewWord(text, characterPosition) && options.GetOption(CompletionOptions.TriggerOnTypingLetters, LanguageNames.Hlsl));
        }

        private static bool SpaceTypedNotBeforeWord(char ch, SourceText text, int characterPosition)
        {
            return ch == ' ' && (characterPosition == text.Length - 1 || !IsWordStartCharacter(text[characterPosition + 1]));
        }

        public static bool IsStartingNewWord(SourceText text, int characterPosition)
        {
            return CommonCompletionUtilities.IsStartingNewWord(
                text, characterPosition, IsWordStartCharacter, IsWordCharacter);
        }

        //public static (string displayText, string insertionText) GetDisplayAndInsertionText(ISymbol symbol, SyntaxContext context)
        //{
        //    var insertionText = GetInsertionText(symbol, context);
        //    var displayText = symbol.GetArity() == 0
        //        ? insertionText
        //        : string.Format("{0}<>", insertionText);

        //    return (displayText, insertionText);
        //}

        //public static string GetInsertionText(ISymbol symbol, SyntaxContext context)
        //{
        //    if (CommonCompletionUtilities.TryRemoveAttributeSuffix(symbol, context, out var name))
        //    {
        //        // Cannot escape Attribute name with the suffix removed. Only use the name with
        //        // the suffix removed if it does not need to be escaped.
        //        if (name.Equals(name.EscapeIdentifier()))
        //        {
        //            return name;
        //        }
        //    }

        //    return symbol.Name.EscapeIdentifier(isQueryContext: context.IsInQuery);
        //}
    }
}
