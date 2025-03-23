using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TypeSelectionWrapper;

public interface IExternalAssemblyTestDataFactoryGenerator
{
    Task<IImmutableList<string>> GenerateTestDataFactoryAsync(
        GenerationParameters generationParameters,
        CancellationToken cancellationToken);
}