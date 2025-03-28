using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace TestDataFactoryGenerator.Generation.FactoryGeneration;

internal class TypeLister : ITypeLister
{
    private readonly IProtoCodeGenerator _protoCodeGenerator;
    private readonly IUserDefinedGenericsCodeGenerator _userDefinedGenericsCodeGenerator;
    private readonly IProtoInformationService _protoInformationService;
    private readonly IEitherInformationService _eitherInformationService;
    private readonly IAbstractClassInformationService _abstractClassInformationService;
    private readonly ILogger<TypeLister> _logger;

    public TypeLister(IProtoCodeGenerator protoCodeGenerator,
        IUserDefinedGenericsCodeGenerator userDefinedGenericsCodeGenerator,
        IProtoInformationService protoInformationService,
        IEitherInformationService eitherInformationService,
        IAbstractClassInformationService abstractClassInformationService,
        ILogger<TypeLister> logger)
    {
        _protoCodeGenerator = protoCodeGenerator;
        _userDefinedGenericsCodeGenerator = userDefinedGenericsCodeGenerator;
        _protoInformationService = protoInformationService;
        _eitherInformationService = eitherInformationService;
        _abstractClassInformationService = abstractClassInformationService;
        _logger = logger;
    }


    public IImmutableSet<Type> GetAllTypes(IImmutableSet<Type> inputTypes)
    {
        var types = new HashSet<Type>();
        foreach (var inputType in inputTypes)
        {
            PopulateType(inputType, types);
        }

        var allTypes = types
            .Where(IsNotInSystemNamespace)
            .ToImmutableHashSet();

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            foreach (var type in allTypes.OrderBy(t => t.FullName))
            {
                _logger.LogTrace($"Type: {type.FullName}");
            }
        }

        return allTypes;
    }

    private static bool IsNotInSystemNamespace(Type type)
    {
        return
            type.Namespace != null &&
            !type.Namespace.StartsWith("System", StringComparison.OrdinalIgnoreCase);
    }

    private void PopulateType(Type type, HashSet<Type> allTypes)
    {
        if (_protoInformationService.IsWellKnownProtobufType(type))
        {
            return;
        }

        allTypes.Add(type);

        if (_protoInformationService.IsProto(type))
        {
            PopulateNestedTypesForProtobufType(type, allTypes);
        }
        else if (_protoInformationService.IsProtoRepeatedField(type))
        {
            PopulateNestedTypesForProtobufRepeatedType(type, allTypes);
        }

        else if (_abstractClassInformationService.IsAbstractClassUsedAsOneOf(type, IsNotInSystemNamespace))
        {
            PopulateDerivedTypes(type, allTypes);
        }
        else
        {
            PopulateNestedTypesForStandardTypes(type, allTypes);
        }
    }

    private void PopulateDerivedTypes(Type type, HashSet<Type> allTypes)
    {
        var derivedTypes = _abstractClassInformationService.GetDerivedTypes(type, IsNotInSystemNamespace);
        foreach (var derivedType in derivedTypes)
        {
            PopulateType(derivedType, allTypes);
        }
    }

    private void PopulateNestedTypesForProtobufType(
        Type type,
        HashSet<Type> types)
    {
        var propertyTypes = _protoCodeGenerator.GetNestedTypes(type);
        foreach (var propertyType in propertyTypes)
        {
            if (_protoInformationService.IsProtoRepeatedField(propertyType))
            {
                var genericType = propertyType.GenericTypeArguments.Single();
                if (!types.Contains(genericType))
                {
                    PopulateType(genericType, types);
                }

                return;
            }

            if (!types.Contains(propertyType))
            {
                PopulateType(propertyType, types);
            }
        }
    }

    private void PopulateNestedTypesForProtobufRepeatedType(
        Type t,
        HashSet<Type> types)
    {
        var genericType = t.GenericTypeArguments.Single();
        if (!types.Contains(genericType))
        {
            PopulateType(genericType, types);
        }
    }

    private void PopulateNestedTypesForStandardTypes(
        Type t,
        HashSet<Type> types)
    {
        var constructors = t.GetConstructors();
        foreach (var constructor in constructors)
        {
            PopulateTypesInConstructor(types, constructor);
        }
    }

    private void PopulateTypesInConstructor(
        HashSet<Type> types,
        ConstructorInfo constructor)
    {
        foreach (var parameter in constructor.GetParameters())
        {
            var parameterType = parameter.ParameterType;
            if (parameterType.IsGenericType)
            {
                PopulateTypesForGenericType(types, parameterType);
            }
            else
            {
                if (!types.Contains(parameterType))
                {
                    PopulateType(parameterType, types);
                }
            }
        }
    }

    private void PopulateTypesForGenericType(
        HashSet<Type> types,
        Type parameterType)
    {
        if (_eitherInformationService.IsEither(parameterType))
        {
            types.Add(parameterType);
        }

        if (_userDefinedGenericsCodeGenerator.IsAUserDefinedGenericType(parameterType))
        {
            types.Add(parameterType);
        }

        var genericTypeArguments = parameterType.GenericTypeArguments;
        foreach (var genericType in genericTypeArguments)
        {
            if (!types.Contains(genericType))
            {
                types.Add(genericType);
                PopulateType(genericType, types);
            }
        }
    }
}