using System.Collections.Generic;
using System.Collections.Immutable;

namespace ShaderTools.CodeAnalysis.Hlsl.Symbols
{
    internal static class IntrinsicSemantics
    {
        public static readonly ImmutableArray<SemanticSymbol> AllSemantics;

        static IntrinsicSemantics()
        {
            var allSemantics = new List<SemanticSymbol>();

            // D3D10
            allSemantics.Add(new SemanticSymbol("SV_ClipDistance", "Clip distance data. SV_ClipDistance values are each assumed to be a float32 signed distance to a plane. Primitive setup only invokes rasterization on pixels for which the interpolated plane distance(s) are >= 0. Multiple clip planes can be implemented simultaneously, by declaring multiple component(s) of one or more vertex elements as the SV_ClipDistance. The combined clip and cull distance values are at most D3D#_CLIP_OR_CULL_DISTANCE_COUNT components in at most D3D#_CLIP_OR_CULL_DISTANCE_ELEMENT_COUNT registers.", true, SemanticUsages.AllShaders & ~SemanticUsages.VertexShaderInput, IntrinsicTypes.Float));
            allSemantics.Add(new SemanticSymbol("SV_CullDistance", "Cull distance data. When component(s) of vertex Element(s) are given this label, these values are each assumed to be a float32 signed distance to a plane. Primitives will be completely discarded if the plane distance(s) for all of the vertices in the primitive are < 0. Multiple cull planes can be used simultaneously, by declaring multiple component(s) of one or more vertex elements as the SV_CullDistance. The combined clip and cull distance values are at most D3D#_CLIP_OR_CULL_DISTANCE_COUNT components in at most D3D#_CLIP_OR_CULL_DISTANCE_ELEMENT_COUNT registers.", true, SemanticUsages.AllShaders & ~SemanticUsages.VertexShaderInput, IntrinsicTypes.Float));
            allSemantics.Add(new SemanticSymbol("SV_Coverage", "A mask that can be specified on input, output, or both of a pixel shader.\n\nFor SV_Coverage on a pixel shader, OUTPUT is supported on ps_4_1 or higher.\n\nFor SV_Coverage on a pixel shader, INPUT requires ps_5_0 or higher.", false, SemanticUsages.PixelShaderInput | SemanticUsages.PixelShaderOutput, IntrinsicTypes.Uint));
            allSemantics.Add(new SemanticSymbol("SV_Depth", "Depth buffer data.", false, SemanticUsages.AllShaders, IntrinsicTypes.Float));
            allSemantics.Add(new SemanticSymbol("SV_DepthGreaterEqual", "Tests whether the value is greater than or equal to the depth data value.", true, SemanticUsages.PixelShaderOutput));
            allSemantics.Add(new SemanticSymbol("SV_DepthLessEqual", "Tests whether the value is less than or equal to the depth data value.", true, SemanticUsages.PixelShaderOutput));
            allSemantics.Add(new SemanticSymbol("SV_DispatchThreadID", "Defines the global thread offset within the Dispatch call, per dimension of the group. (read only)", false, SemanticUsages.ComputeShaderInput, IntrinsicTypes.Uint3));
            allSemantics.Add(new SemanticSymbol("SV_DomainLocation", "Defines the location on the hull of the current domain point being evaluated. (read only)", false, SemanticUsages.DomainShaderInput, IntrinsicTypes.Float2, IntrinsicTypes.Float3));
            allSemantics.Add(new SemanticSymbol("SV_GroupID", "Defines the group offset within a Dispatch call, per dimension of the dispatch call. (read only)", false, SemanticUsages.ComputeShaderInput, IntrinsicTypes.Uint3));
            allSemantics.Add(new SemanticSymbol("SV_GroupIndex", "Provides a flattened index for a given thread within a given group. (read only)", false, SemanticUsages.ComputeShaderInput, IntrinsicTypes.Uint));
            allSemantics.Add(new SemanticSymbol("SV_GroupThreadID", "Defines the thread offset within the group, per dimension of the group. (read only)", false, SemanticUsages.ComputeShaderInput, IntrinsicTypes.Uint3));
            allSemantics.Add(new SemanticSymbol("SV_GSInstanceID", "Defines the instance of the geometry shader. The instance is needed as a geometry shader can be invoked up to 32 times on the same geometry primitive.", false, SemanticUsages.GeometryShaderInput, IntrinsicTypes.Uint));
            allSemantics.Add(new SemanticSymbol("SV_InnerCoverage", "Represents underestimated conservative rasterization information (i.e. whether a pixel is guaranteed-to-be-fully covered).", false, SemanticUsages.PixelShaderInput | SemanticUsages.PixelShaderOutput));
            allSemantics.Add(new SemanticSymbol("SV_InsideTessFactor", "Defines the tessellation amount within a patch surface.", false, SemanticUsages.HullShaderOutput | SemanticUsages.DomainShaderInput, IntrinsicTypes.Float, IntrinsicTypes.Float2));
            allSemantics.Add(new SemanticSymbol("SV_InstanceID", "Per-instance identifier automatically generated by the runtime.", false, SemanticUsages.VertexShaderInput));
            allSemantics.Add(new SemanticSymbol("SV_IsFrontFace", "Specifies whether a triangle is front facing. For lines and points, IsFrontFace has the value true. The exception is lines drawn out of triangles (wireframe mode), which sets IsFrontFace the same way as rasterizing the triangle in solid mode.", false, SemanticUsages.GeometryShaderOutput | SemanticUsages.PixelShaderInput, IntrinsicTypes.Bool));
            allSemantics.Add(new SemanticSymbol("SV_OutputControlPointID", "Defines the index of the control point ID being operated on by an invocation of the main entry point of the hull shader.", false, SemanticUsages.HullShaderInput, IntrinsicTypes.Uint));
            allSemantics.Add(new SemanticSymbol("SV_Position", "When SV_Position is declared for input to a shader, it can have one of two interpolation modes specified: linearNoPerspective or linearNoPerspectiveCentroid, where the latter causes centroid-snapped xyzw values to be provided when multisample antialiasing. When used in a shader, SV_Position describes the pixel location. Available in all shaders to get the pixel center with a 0.5 offset.", false, SemanticUsages.VertexShaderOutput | SemanticUsages.GeometryShaderInput | SemanticUsages.GeometryShaderOutput | SemanticUsages.PixelShaderInput, IntrinsicTypes.Float4));
            allSemantics.Add(new SemanticSymbol("SV_PrimitiveID", "Per-primitive identifier automatically generated by the runtime.", false, SemanticUsages.GeometryShaderOutput | SemanticUsages.PixelShaderOutput | SemanticUsages.GeometryShaderInput | SemanticUsages.PixelShaderInput | SemanticUsages.HullShaderInput | SemanticUsages.DomainShaderInput, IntrinsicTypes.Uint));
            allSemantics.Add(new SemanticSymbol("SV_RenderTargetArrayIndex", "Render-target array index. Applied to geometry shader output and indicates the render target array slice that the primitive will be drawn to by the pixel shader. SV_RenderTargetArrayIndex is only valid if the render target is an array resource. This semantic applies only to primitives, if a primitive has more than one vertex the value from the leading vertex will be used.\nThis value also indicates which array slice of a depthstencilview is used for read / write purposes.", false, SemanticUsages.GeometryShaderOutput | SemanticUsages.PixelShaderInput | SemanticUsages.PixelShaderOutput, IntrinsicTypes.Uint));
            allSemantics.Add(new SemanticSymbol("SV_SampleIndex", "Sample frequency index data.", false, SemanticUsages.PixelShaderInput | SemanticUsages.PixelShaderOutput, IntrinsicTypes.Uint));
            allSemantics.Add(new SemanticSymbol("SV_StencilRef", "Represents the current pixel shader stencil reference value.", false, SemanticUsages.PixelShaderOutput, IntrinsicTypes.Uint));
            allSemantics.Add(new SemanticSymbol("SV_Target", "The output value that will be stored in a render target. The index indicates which of the 8 possibly bound render targets to write to.", true, SemanticUsages.PixelShaderOutput, IntrinsicTypes.Float));
            allSemantics.Add(new SemanticSymbol("SV_TessFactor", "Defines the tessellation amount on each edge of a patch. Available for writing in the hull shader and reading in the domain shader.", false, SemanticUsages.HullShaderOutput | SemanticUsages.DomainShaderInput, IntrinsicTypes.Float2, IntrinsicTypes.Float3, IntrinsicTypes.Float4));
            allSemantics.Add(new SemanticSymbol("SV_VertexID", "Per-vertex identifier automatically generated by the runtime.", false, SemanticUsages.VertexShaderInput, IntrinsicTypes.Uint));
            allSemantics.Add(new SemanticSymbol("SV_ViewportArrayIndex", "Viewport array index. Applied to geometry shader output and indicates which viewport to use for the primitive currently being written out. The primitive will be transformed and clipped against the viewport specified by the index before it is passed to the rasterizer. This semantic applies only to primitives, if a primitive has more than one vertex the value from the leading vertex will be used.", false, SemanticUsages.GeometryShaderOutput | SemanticUsages.PixelShaderInput | SemanticUsages.PixelShaderOutput, IntrinsicTypes.Uint));

            // D3D9
            allSemantics.Add(new SemanticSymbol("BINORMAL", "Binormal.", true, SemanticUsages.VertexShaderInput));
            allSemantics.Add(new SemanticSymbol("BLENDINDICES", "Blend indices.", true, SemanticUsages.VertexShaderInput));
            allSemantics.Add(new SemanticSymbol("BLENDWEIGHT", "Blend weights.", true, SemanticUsages.VertexShaderInput));
            allSemantics.Add(new SemanticSymbol("COLOR", "Diffuse or specular color.", true, SemanticUsages.VertexShaderInput | SemanticUsages.VertexShaderOutput | SemanticUsages.PixelShaderInput | SemanticUsages.PixelShaderOutput));
            allSemantics.Add(new SemanticSymbol("NORMAL", "Normal vector.", true, SemanticUsages.VertexShaderInput));
            allSemantics.Add(new SemanticSymbol("POSITION", "Used as input: Vertex position in object space.\nUsed as output: Position of a vertex in homogenous space. Compute position in screen-space by dividing (x,y,z) by w. Every (D3D9) vertex shader must write out a parameter with this semantic.", true, SemanticUsages.VertexShaderInput));
            allSemantics.Add(new SemanticSymbol("POSITIONT", "Transformed vertex position.", false, SemanticUsages.VertexShaderInput));
            allSemantics.Add(new SemanticSymbol("PSIZE", "Point size.", true, SemanticUsages.VertexShaderInput));
            allSemantics.Add(new SemanticSymbol("TANGENT", "Tangent.", true, SemanticUsages.VertexShaderInput));
            allSemantics.Add(new SemanticSymbol("TEXCOORD", "Texture coordinates", true, SemanticUsages.VertexShaderInput | SemanticUsages.VertexShaderOutput | SemanticUsages.PixelShaderInput));
            allSemantics.Add(new SemanticSymbol("FOG", "Vertex fog.", false, SemanticUsages.VertexShaderOutput));
            allSemantics.Add(new SemanticSymbol("PSIZE", "Point size.", true, SemanticUsages.VertexShaderOutput | SemanticUsages.PixelShaderInput));
            allSemantics.Add(new SemanticSymbol("TESSFACTOR", "Tessellation factor.", true, SemanticUsages.VertexShaderOutput));
            allSemantics.Add(new SemanticSymbol("VFACE", "Floating-point scalar that indicates a back-facing primitive. A negative value faces backwards, while a positive value faces the camera.\nNote  This semantic is available in Direct3D 9 Shader Model 3.0. For Direct3D 10 and later, use SV_IsFrontFace instead.", false, SemanticUsages.PixelShaderInput));
            allSemantics.Add(new SemanticSymbol("VPOS", "The pixel location (x,y) in screen space.", false, SemanticUsages.PixelShaderInput));
            allSemantics.Add(new SemanticSymbol("DEPTH", "Depth.", true, SemanticUsages.PixelShaderOutput));

            AllSemantics = allSemantics.ToImmutableArray();
        }
    }
}