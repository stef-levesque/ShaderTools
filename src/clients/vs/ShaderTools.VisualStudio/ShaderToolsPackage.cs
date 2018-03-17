using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ShaderTools.VisualStudio
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(ShaderToolsPackage.PackageGuidString)]

    [ProvideLanguageExtension(typeof(HlslLanguageInfo), ".hlsl")]
    [ProvideLanguageExtension(typeof(HlslLanguageInfo), ".hlsli")]
    [ProvideLanguageExtension(typeof(HlslLanguageInfo), ".fx")]
    [ProvideLanguageExtension(typeof(HlslLanguageInfo), ".fxh")]
    [ProvideLanguageExtension(typeof(HlslLanguageInfo), ".vsh")]
    [ProvideLanguageExtension(typeof(HlslLanguageInfo), ".psh")]
    [ProvideLanguageExtension(typeof(HlslLanguageInfo), ".cginc")]
    [ProvideLanguageExtension(typeof(HlslLanguageInfo), ".compute")]

    [ProvideLanguageExtension(typeof(ShaderLabLanguageInfo), ".shader")]

    public sealed class ShaderToolsPackage : AsyncPackage
    {
        public const string PackageGuidString = "e52029c2-bf90-4c5c-ad38-98cb294e1e9b";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }
    }
}
