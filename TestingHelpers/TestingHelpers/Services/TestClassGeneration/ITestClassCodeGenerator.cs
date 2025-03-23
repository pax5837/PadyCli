using System.Collections.Immutable;

namespace TestingHelpers.Services.TestClassGeneration;

internal interface ITestClassCodeGenerator
{
    IImmutableList<string> GenerateTestClass(Type type);
}