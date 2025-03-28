using System.Collections.Immutable;
using System.Reflection;

namespace TestDataFactoryGenerator.Generation;

internal interface ITypeWithConstructorsGenerator
{
    IImmutableList<string> CreateGenerationMethod(
        Type type,
        HashSet<string> dependencies);

    IImmutableList<string> CreateGenerationCodeForOneConstructor(
        Type type,
        ConstructorInfo constructor,
        HashSet<string> dependencies);
}