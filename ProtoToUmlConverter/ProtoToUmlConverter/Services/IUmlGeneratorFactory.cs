namespace ProtoToUmlConverter.Services;

public interface IUmlGeneratorFactory
{
    IUmlGenerator GetUmlGenerator(DiagramProvider diagramProvider);
}