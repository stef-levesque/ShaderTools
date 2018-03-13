namespace ShaderTools.CodeAnalysis.QuickInfo
{
    internal interface IQuickInfoProviderCoordinatorFactory
    {
        IQuickInfoProviderCoordinator CreateCoordinator(LogicalDocument document);
    }
}