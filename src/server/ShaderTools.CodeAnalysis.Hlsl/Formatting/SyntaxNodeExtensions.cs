﻿using System.Collections.Generic;
using System.Linq;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;

namespace ShaderTools.CodeAnalysis.Hlsl.Formatting
{
    internal static class SyntaxNodeExtensions
    {
        public static SyntaxNode FindCommonAncestor(this SyntaxToken token, SyntaxToken otherToken)
        {
            var otherTokenAncestors = otherToken.Ancestors().ToList();
            return token.Ancestors().Cast<SyntaxNode>().FirstOrDefault(ancestor => otherTokenAncestors.Contains(ancestor));
        }

        public static IEnumerable<LocatedNode> GetRootLocatedNodes(this SyntaxNode node)
        {
            if (node.IsToken)
            {
                var token = (SyntaxToken) node;
                foreach (var locatedNode in GetRootLocatedNodes(token))
                    yield return locatedNode;
            }
            else if (node is SyntaxTrivia)
            {
                var trivia = (SyntaxTrivia) node;
                if (trivia.FileSpan.File.IsRootFile)
                    yield return trivia;
            }
            else
            {
                foreach (var childNode in node.ChildNodes)
                    foreach (var locatedNode in GetRootLocatedNodes((SyntaxNode) childNode))
                        yield return locatedNode;
            }
        }

        private static IEnumerable<LocatedNode> GetRootLocatedNodes(this SyntaxToken token)
        {
            if (token.MacroReference == null)
                foreach (var trivia in token.LeadingTrivia)
                    foreach (var locatedNode in GetRootLocatedNodes(trivia))
                        yield return locatedNode;


            if (token.MacroReference != null)
            {
                if (token.IsFirstTokenInMacroExpansion)
                    foreach (var originalNode in token.MacroReference.OriginalNodes)
                        foreach (var locatedNode in GetRootLocatedNodes(originalNode))
                            yield return locatedNode;
            }
            else
            {
                if (token.FileSpan.File.IsRootFile)
                    yield return token;
            }

            if (token.MacroReference == null)
                foreach (var trivia in token.TrailingTrivia)
                    foreach (var locatedNode in GetRootLocatedNodes(trivia))
                        yield return locatedNode;
        }
    }
}