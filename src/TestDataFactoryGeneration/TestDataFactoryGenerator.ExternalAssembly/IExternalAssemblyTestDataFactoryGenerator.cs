using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TypeSelectionWrapper;

public interface IExternalAssemblyTestDataFactoryGenerator
{
    Task<IImmutableList<string>> GenerateTestDataFactoryAsync(
        GenerationParameters generationParameters,
        CancellationToken cancellationToken);

    public record GenerationParameters(
        string TestDataFactoryName,
        string NameSpace,
        IImmutableSet<string> TypeNames,
        bool OutputToConsole,
        string? ExecutionDirectory = null);
}