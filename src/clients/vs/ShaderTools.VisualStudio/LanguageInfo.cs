using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace ShaderTools.VisualStudio
{
    // These LanguageInfo classes, and the [ProvideLanguageExtension] attributes on ShaderToolsPackage,
    // are only necessary because HLSL and ShaderLab are treated a bit differently by Visual Studio.
    // There is some limited built-in support for these languages that piggybacks on the C++ editor.
    // Using modern ContentType-based extensibility alone isn't enough - we also need to override
    // the built-in support, and providing a IVsLanguageInfo seems to be the way to do it.
    internal abstract class LanguageInfo : IVsLanguageInfo
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
            bstrName = ;

            return VSConstants.S_OK;
        }
    }

    [Guid("889F59DE-BF8E-48C7-8649-60EC288A4E89")]
    internal sealed class HlslLanguageInfo : LanguageInfo
    {
        protected override string LanguageName { get; } = ContentTypeDefinitions.Hlsl;
    }

    [Guid("0E6876C7-11C4-497D-8B0F-BFE496A21FC2")]
    internal sealed class ShaderLabLanguageInfo : LanguageInfo
    {
        protected override string LanguageName { get; } = ContentTypeDefinitions.ShaderLab;
    }
}
