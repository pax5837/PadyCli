using System.Collections.Immutable;
using System.Reflection;

namespace TestDataFactoryGenerator.Generation;

internal interface ITypeWithConstructorsGenerator
{
    IImmutableList<string> CreateGenerationCode(
        Type t,
        HashSet<string> dependencies);

    IImmutableList<string> CreateGenerationCodeForOneConstructor(
        Type type,
        ConstructorInfo constructor,
        HashSet<string> dependencies);
}