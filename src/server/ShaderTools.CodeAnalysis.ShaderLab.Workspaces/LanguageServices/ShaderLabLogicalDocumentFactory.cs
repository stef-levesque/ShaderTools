using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ShaderTools.CodeAnalysis.Host;
using ShaderTools.CodeAnalysis.Host.Mef;
using ShaderTools.CodeAnalysis.ShaderLab.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.ShaderLab.LanguageServices
{
    [ExportLanguageService(typeof(ILogicalDocumentFactory), LanguageNames.ShaderLab)]
    internal sealed class ShaderLabLogicalDocumentFactory : ILogicalDocumentFactory
    {
        private const RegexOptions ProgramRegexOptions = RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase;
        private static readonly Regex CgProgramRegex = new Regex(@"(CGPROGRAM|CGINCLUDE)([\w\W]+?)ENDCG", ProgramRegexOptions);
        private static readonly Regex HlslProgramRegex = new Regex(@"(HLSLPROGRAM|HLSLINCLUDE)([\w\W]+?)ENDHLSL", ProgramRegexOptions);

        public IEnumerable<LogicalDocument> GetLogicalDocuments(Document document)
        {
            // TODO: Is this too slow?
            var text = document.SourceText.ToString();

            var embeddedBlocks = new List<(TextSpan Span, SyntaxKind Kind)>();

            void scanBlocks(Regex regex, SyntaxKind programKind, SyntaxKind includeKind)
            {
                var programMatches = regex.Matches(text);
                foreach (Match match in programMatches)
                {
                    var isInclude = match.Groups[1].Value.EndsWith("INCLUDE", StringComparison.InvariantCultureIgnoreCase);
                    embeddedBlocks.Add((
                        new TextSpan(match.Groups[2].Index, match.Groups[2].Length),
                        isInclude ? includeKind : programKind));
                }
            }

            scanBlocks(CgProgramRegex, SyntaxKind.CgProgram, SyntaxKind.CgInclude);
            scanBlocks(HlslProgramRegex, SyntaxKind.HlslProgram, SyntaxKind.HlslInclude);

            embeddedBlocks.Sort((x, y) => Comparer<int>.Default.Compare(x.Span.Start, y.Span.Start));

            // ShaderLab
            yield return new LogicalDocument(
                document.SourceText,
                document.Language,
                document,
                new TextSpan(0, document.SourceText.Length));

            var cgIncludes = new List<SourceText>();
            var hlslIncludes = new List<SourceText>();

            const string implicitCgIncludes = @"
#include ""HLSLSupport.cginc""
#include ""UnityShaderVariables.cginc""
";

            string GetIncludeBlocks(List<SourceText> includeBlocks)
            {
                var result = new StringBuilder();
                foreach (var includeBlock in includeBlocks)
                {
                    result.Append(includeBlock.ToString());
                }
                return result.ToString();
            }

            // HLSL
            foreach (var (span, kind) in embeddedBlocks)
            {
                var subText = document.SourceText.GetSubText(span);

                // Prepend default includes, as well as any preceding *INCLUDE blocks.
                var implicitIncludes = string.Empty;
                switch (kind)
                {
                    case SyntaxKind.CgInclude:
                        implicitIncludes += implicitCgIncludes;
                        cgIncludes.Add(subText);
                        break;

                    case SyntaxKind.HlslInclude:
                        hlslIncludes.Add(subText);
                        break;

                    case SyntaxKind.CgProgram:
                        implicitIncludes += implicitCgIncludes;
                        implicitIncludes += GetIncludeBlocks(cgIncludes);
                        break;

                    case SyntaxKind.HlslProgram:
                        implicitIncludes += GetIncludeBlocks(hlslIncludes);
                        break;
                }

                var actualText = SourceText.From(
                    implicitIncludes + $"#line 1 \"{subText.FilePath}\"\n" + subText.ToString(), 
                    subText.FilePath);

                yield return new LogicalDocument(
                    actualText,
                    LanguageNames.Hlsl,
                    document,
                    span);
            }
        }
    }
}
