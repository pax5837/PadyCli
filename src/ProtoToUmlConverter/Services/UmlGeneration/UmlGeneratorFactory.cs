namespace ProtoToUmlConverter.Services.UmlGeneration;

internal class UmlGeneratorFactory : IUmlGeneratorFactory
{
    public IUmlGenerator GetUmlGenerator(DiagramProvider diagramProvider)
    {
        return diagramProvider switch
        {
            DiagramProvider.Mermaid => new MermaidGenerator(),
            DiagramProvider.PlantUml => new PlantUmlGenerator(),
            _ => throw new ArgumentException($"Can not handle {diagramProvider}"),
        };
    }
}