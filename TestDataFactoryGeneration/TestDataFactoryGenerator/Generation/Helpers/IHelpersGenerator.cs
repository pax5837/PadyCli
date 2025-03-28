using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Helpers;

internal interface IHelpersGenerator
{
    IImmutableList<string> GenerateHelpersCode(bool includeHelperClasses);
}