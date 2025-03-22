using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal interface IProtoCodeGenerator
{
    IImmutableList<Type> GetNestedTypes(Type type);

    string GenerateInstantiationForWellKnownProtobufType(
        Type type,
        HashSet<string> dependencies);

    IImmutableList<string> GenerateInstantiationCodeForProtobufType(
        Type type,
        HashSet<string> dependencies,
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator);

}