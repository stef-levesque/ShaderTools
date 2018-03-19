using System.ComponentModel.Composition;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;

namespace ShaderTools.VisualStudio
{
#pragma warning disable CS0649
    internal static class ContentTypeDefinitions
    {
        public const string Hlsl = "hlsl";

        [Export]
        [Name(Hlsl)]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        public static readonly ContentTypeDefinition HlslContentTypeDefinition;

        [Export, ContentType(Hlsl), FileExtension(".hlsl")]
        internal static readonly FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionHlsl;

        [Export, ContentType(Hlsl), FileExtension(".hlsli")]
        internal static readonly FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionHlsli;

        [Export, ContentType(Hlsl), FileExtension(".fx")]
        internal static readonly FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionFx;

        [Export, ContentType(Hlsl), FileExtension(".fxh")]
        internal static readonly FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionFxh;

        [Export, ContentType(Hlsl), FileExtension(".vsh")]
        internal static readonly FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionVsh;

        [Export, ContentType(Hlsl), FileExtension(".psh")]
        internal static readonly FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionPsh;

        [Export, ContentType(Hlsl), FileExtension(".cginc")]
        internal static readonly FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionCginc;

        [Export, ContentType(Hlsl), FileExtension(".compute")]
        internal static readonly FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionCompute;

        public const string ShaderLab = "shaderlab";

        [Export]
        [Name(ShaderLab)]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static readonly ContentTypeDefinition ShaderLabContentTypeDefinition;

        [Export, ContentType(ShaderLab), FileExtension(".shader")]
        internal static readonly FileExtensionToContentTypeDefinition ShaderLabFileExtensionDefinition;
    }
#pragma warning restore CS0649
}
