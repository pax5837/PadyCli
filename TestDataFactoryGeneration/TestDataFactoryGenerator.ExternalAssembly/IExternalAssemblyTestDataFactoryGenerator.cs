using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TypeSelectionWrapper;

public interface IExternalAssemblyTestDataFactoryGenerator
{
    Task<IImmutableList<string>> GenerateTestDataFactory(
        GenerationParameters generationParameters,
        CancellationToken cancellationToken);
}