namespace ShaderTools.CodeAnalysis.Hlsl.Binding.BoundNodes
{
    internal sealed class BoundStateArrayInitializer : BoundInitializer
    {
        public BoundStateArrayInitializer()
            : base(BoundNodeKind.StateArrayInitializer)
        {
        }
    }
}