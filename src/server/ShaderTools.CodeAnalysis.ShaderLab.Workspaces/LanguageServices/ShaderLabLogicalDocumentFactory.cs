using System.Collections.Generic;
using System.Text.RegularExpressions;
using ShaderTools.CodeAnalysis.Host;
using ShaderTools.CodeAnalysis.Host.Mef;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.CodeAnalysis.ShaderLab.LanguageServices
{
    [ExportLanguageService(typeof(ILogicalDocumentFactory), LanguageNames.ShaderLab)]
    internal sealed class ShaderLabLogicalDocumentFactory : ILogicalDocumentFactory
    {
        private static readonly Regex CgProgramRegex = new Regex(@"CGPROGRAM([\w\W]+?)ENDCG", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex HlslProgramRegex = new Regex(@"HLSLPROGRAM([\w\W]+?)ENDHLSL", RegexOptions.Compiled | RegexOptions.Multiline);

        public IEnumerable<LogicalDocument> GetLogicalDocuments(Document document)
        {
            // TODO: Is this too slow?
            var text = document.SourceText.ToString();

            var embeddedBlocks = new List<TextSpan>();

            void scanBlocks(Regex regex)
            {
                var programMatches = regex.Matches(text);
                foreach (Match match in programMatches)
                {
                    embeddedBlocks.Add(new TextSpan(match.Groups[1].Index, match.Groups[1].Length));
                }
            }

            scanBlocks(CgProgramRegex);
            scanBlocks(HlslProgramRegex);

            embeddedBlocks.Sort((x, y) => Comparer<int>.Default.Compare(x.Start, y.Start));

            // ShaderLab
            yield return new LogicalDocument(
                document.SourceText,
                document.Language,
                document,
                new TextSpan(0, document.SourceText.Length));

            // HLSL
            foreach (var block in embeddedBlocks)
            {
                var subText = document.SourceText.GetSubText(block);

                yield return new LogicalDocument(
                    subText,
                    LanguageNames.Hlsl,
                    document,
                    block);
            }
        }
    }
}
