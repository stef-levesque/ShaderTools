using System.ComponentModel.Composition;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;

namespace ShaderTools.VisualStudio
{
    internal static class ContentTypeDefinitions
    {
        public const string Hlsl = "hlsl";

        [Export]
        [Name(Hlsl)]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static ContentTypeDefinition HlslContentTypeDefinition;

        [Export, ContentType(Hlsl), FileExtension(".hlsl")]
        internal static FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionHlsl;

        [Export, ContentType(Hlsl), FileExtension(".hlsli")]
        internal static FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionHlsli;

        [Export, ContentType(Hlsl), FileExtension(".fx")]
        internal static FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionFx;

        [Export, ContentType(Hlsl), FileExtension(".fxh")]
        internal static FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionFxh;

        [Export, ContentType(Hlsl), FileExtension(".vsh")]
        internal static FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionVsh;

        [Export, ContentType(Hlsl), FileExtension(".psh")]
        internal static FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionPsh;

        [Export, ContentType(Hlsl), FileExtension(".cginc")]
        internal static FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionCginc;

        [Export, ContentType(Hlsl), FileExtension(".compute")]
        internal static FileExtensionToContentTypeDefinition HlslFileExtensionDefinitionCompute;

        public const string ShaderLab = "shaderlab";

        [Export]
        [Name(ShaderLab)]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static ContentTypeDefinition ShaderLabContentTypeDefinition;

        [Export, ContentType(ShaderLab), FileExtension(".shader")]
        internal static FileExtensionToContentTypeDefinition ShaderLabFileExtensionDefinition;
    }
}
