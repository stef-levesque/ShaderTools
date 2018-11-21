using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.Text;
using ShaderTools.Testing.Workspaces;
using Xunit;

namespace ShaderTools.CodeAnalysis.ShaderLab.Features.Tests
{
    public class DocumentTests
    {
        [Theory]
        [InlineData("Camera-DepthNormalTexture.shader", 11)]
        [InlineData("ShaderWithIncludeBlock.shader", 3)]
        public async Task CanCreateLogicalDocuments(string shader, int logicalDocumentsCount)
        {
            var workspace = new TestWorkspace();

            var testFile = Path.GetFullPath(Path.Combine("TestAssets", shader));

            var document = workspace.OpenDocument(
                DocumentId.CreateNewId(),
                SourceText.From(File.ReadAllText(testFile), testFile),
                LanguageNames.ShaderLab);

            Assert.Equal(logicalDocumentsCount, document.LogicalDocuments.Length);

            foreach (var logicalDocument in document.LogicalDocuments)
            {
                var syntaxTree = await logicalDocument.GetSyntaxTreeAsync(CancellationToken.None);

                var syntaxTreeDiagnostics = syntaxTree.GetDiagnostics();
                Assert.Empty(syntaxTreeDiagnostics);

                if (logicalDocument.SupportsSemanticModel)
                {
                    var semanticModel = await logicalDocument.GetSemanticModelAsync(CancellationToken.None);

                    var semanticModelDiagnostics = semanticModel.GetDiagnostics();
                    Assert.Empty(semanticModelDiagnostics.Where(x => x.Severity == CodeAnalysis.Diagnostics.DiagnosticSeverity.Error));
                }
            }
        }
    }
}
