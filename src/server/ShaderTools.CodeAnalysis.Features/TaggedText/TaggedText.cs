﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ShaderTools.CodeAnalysis.Classification;
using ShaderTools.CodeAnalysis.Collections;
using ShaderTools.CodeAnalysis.Symbols.Markup;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis
{
    /// <summary>
    /// A piece of text with a descriptive tag.
    /// </summary>
    public struct TaggedText
    {
        /// <summary>
        /// A descriptive tag from <see cref="TextTags"/>.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// The actual text to be displayed.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Creates a new instance of <see cref="TaggedText"/>
        /// </summary>
        /// <param name="tag">A descriptive tag from <see cref="TextTags"/>.</param>
        /// <param name="text">The actual text to be displayed.</param>
        public TaggedText(string tag, string text)
        {
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public override string ToString()
        {
            return Text;
        }
    }

    internal static class TaggedTextExtensions
    {
        public static ImmutableArray<TaggedText> ToTaggedText(this IEnumerable<SymbolMarkupToken> displayParts)
        {
            if (displayParts == null)
            {
                return ImmutableArray<TaggedText>.Empty;
            }

            return displayParts.Select(d =>
                new TaggedText(SymbolMarkupKindTags.GetTag(d.Kind), d.Text)).ToImmutableArray();
        }

        public static string JoinText(this ImmutableArray<TaggedText> values)
        {

            return values.IsDefault
                ? null
                : Join(values);
        }

        private static string Join(ImmutableArray<TaggedText> values)
        {
            var pooled = PooledStringBuilder.GetInstance();
            var builder = pooled.Builder;
            foreach (var val in values)
            {
                builder.Append(val.Text);
            }

            return pooled.ToStringAndFree();
        }

        public static string ToClassificationTypeName(this string taggedTextTag, ITaggedTextMappingService mappingService)
        {
            return mappingService.GetClassificationTypeName(taggedTextTag);
        }

        public static IEnumerable<ClassifiedSpan> ToClassifiedSpans(
            this IEnumerable<TaggedText> parts, 
            ITaggedTextMappingService mappingService)
        {
            var index = 0;
            foreach (var part in parts)
            {
                var text = part.ToString();
                var classificationTypeName = part.Tag.ToClassificationTypeName(mappingService);

                yield return new ClassifiedSpan(new TextSpan(index, text.Length), classificationTypeName);
                index += text.Length;
            }
        }

        private const string LeftToRightMarkerPrefix = "\u200e";

        public static string ToVisibleDisplayString(this TaggedText part, bool includeLeftToRightMarker)
        {
            var text = part.ToString();

            if (includeLeftToRightMarker)
            {
                var tag = part.Tag;
                if (tag == TextTags.Punctuation ||
                    tag == TextTags.Space ||
                    tag == TextTags.LineBreak)
                {
                    text = LeftToRightMarkerPrefix + text;
                }
            }

            return text;
        }

        public static string ToVisibleDisplayString(this IEnumerable<TaggedText> parts, bool includeLeftToRightMarker)
        {
            return string.Join(string.Empty, parts.Select(
                p => p.ToVisibleDisplayString(includeLeftToRightMarker)));
        }

        public static string GetFullText(this IEnumerable<TaggedText> parts)
        {
            return string.Join(string.Empty, parts.Select(p => p.ToString()));
        }

        public static void AddLineBreak(this IList<TaggedText> parts, string text = "\r\n")
        {
            parts.Add(new TaggedText(TextTags.LineBreak, text));
        }

        public static void AddSpace(this IList<TaggedText> parts, string text = " ")
        {
            parts.Add(new TaggedText(TextTags.Space, text));
        }

        public static void AddText(this IList<TaggedText> parts, string text)
        {
            parts.Add(new TaggedText(TextTags.Text, text));
        }

        public static void AddPunctuation(this IList<TaggedText> parts, string text)
        {
            parts.Add(new TaggedText(TextTags.Punctuation, text));
        }
    }
}