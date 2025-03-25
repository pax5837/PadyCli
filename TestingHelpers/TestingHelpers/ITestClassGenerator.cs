using System.Collections.Immutable;

namespace TestingHelpers;

public interface ITestClassGenerator
{
    public Task<IImmutableList<string>> GenerateTestClassAsync(
        string targetClassName,
        bool compileOnTheFly,
        CancellationToken cancellationToken);
}