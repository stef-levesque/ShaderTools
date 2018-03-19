using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Task = System.Threading.Tasks.Task;

namespace ShaderTools.VisualStudio
{
    /// <summary>
    /// This package class only exists for the [ProvideLanguageExtension] attributes,
    /// which themselves are only necessary because HLSL and ShaderLab are treated a bit differently by Visual Studio.
    /// There is some limited built-in support for these languages that piggybacks on the C++ editor.
    /// Using modern ContentType-based extensibility alone isn't enough - we also need to override
    /// the built-in support, and providing a IVsLanguageInfo seems to be the way to do it.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid("e52029c2-bf90-4c5c-ad38-98cb294e1e9b")]

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
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        private abstract class LanguageInfo : IVsLanguageInfo
        {
            protected abstract string LanguageName { get; }

            public int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr)
            {
                ppCodeWinMgr = null;
                return VSConstants.S_OK;
            }

            public int GetColorizer(IVsTextLines pBuffer, out IVsColorizer ppColorizer)
            {
                ppColorizer = null;

                return VSConstants.E_NOTIMPL;
            }

            public int GetFileExtensions(out string pbstrExtensions)
            {
                pbstrExtensions = null;

                return VSConstants.E_NOTIMPL;
            }

            public int GetLanguageName(out string bstrName)
            {
                bstrName = LanguageName;

                return VSConstants.S_OK;
            }
        }

        [Guid("889F59DE-BF8E-48C7-8649-60EC288A4E89")]
        private sealed class HlslLanguageInfo : LanguageInfo
        {
            protected override string LanguageName { get; } = ContentTypeDefinitions.Hlsl;
        }

        [Guid("0E6876C7-11C4-497D-8B0F-BFE496A21FC2")]
        private sealed class ShaderLabLanguageInfo : LanguageInfo
        {
            protected override string LanguageName { get; } = ContentTypeDefinitions.ShaderLab;
        }
    }
}
