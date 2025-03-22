using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal interface IEitherCodeGenerator
{

    string GetEitherGeneratorName(Type type);

    string GenerateEitherParameterInstantiation(Type type);

    IImmutableList<string> CreateGenerationCodeForEither(Type type, HashSet<string> dependencies, IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator);
}