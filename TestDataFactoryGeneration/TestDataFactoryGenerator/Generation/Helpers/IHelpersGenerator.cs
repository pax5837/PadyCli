using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Helpers;

internal interface IHelpersGenerator
{
    IImmutableList<string> GenerateCollectionHelperCode();

    IImmutableList<string> GenerateNonTdfSpecificHelperCode(bool includeHelperClasses);
}