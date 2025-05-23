using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.AbstractClasses;

internal class AbstractOneOfClassGenerationCreationService : IAbstractOneOfClassGenerationCreationService
{
    private readonly IAbstractClassInformationService _infoService;
    private readonly TdfGeneratorConfiguration _config;

    public AbstractOneOfClassGenerationCreationService(
        IAbstractClassInformationService infoService,
        TdfGeneratorConfiguration config)
    {
        _infoService = infoService;
        _config = config;
    }

    public IImmutableList<string> CreateGenerationCode(Type type)
    {
        var lines = new Lines(_config);

        var derivedTypes = _infoService.GetDerivedTypes(type, _ => true).ToImmutableList();

        lines
            .Add(1, $"public {type.Name} {Definitions.GenerationMethodPrefix}{type.Name}()")
            .Add(1, "{")
            .Add(2, $"return {_config.LeadingUnderscore()}random.Next(0, {derivedTypes.Count}) switch {{");

        for (int i = 0; i < derivedTypes.Count; i++)
        {
            lines.Add(3, $"{i} => Generate{derivedTypes[i].Name}(),");
        }

        lines
            .Add(3, $"_ => throw new InvalidOperationException(\"Unexpected constructor number\"),")
            .Add(2, "};")
            .Add(1, "}")
            .AddEmptyLine();

        return lines.ToImmutableList();
    }
}