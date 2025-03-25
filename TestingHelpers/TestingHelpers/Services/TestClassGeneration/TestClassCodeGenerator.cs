using System.Collections.Immutable;
using System.Reflection;

namespace TestingHelpers.Services.TestClassGeneration;

internal class TestClassCodeGenerator : ITestClassCodeGenerator
{
    private readonly TestClassGeneratorConfig _config;

    private static readonly IImmutableSet<string> SutMethodNamesToIgnore =
        ["GetType", "ToString", "Equals", "GetHashCode"];

    public TestClassCodeGenerator(TestClassGeneratorConfig config)
    {
        _config = config;
    }

    public IImmutableList<string> GenerateTestClass(Type type)
    {
        var lines = new List<string>();
        lines.Add("using Microsoft.VisualStudio.TestTools.UnitTesting;");
        lines.Add("using Moq;");
        lines.Add(string.Empty);
        lines.Add($"using {type.Namespace};");
        lines.Add(string.Empty);
        lines.Add($"namespace {type.Namespace};");
        lines.Add(string.Empty);
        lines.Add("[TestClass]");
        lines.Add($"public class {type.Name}Tests");
        lines.Add("{");
        lines.AddRange(GenerateTestMethods(type));
        lines.AddRange(GenerateTestSetup(type));
        lines.Add("}");

        return lines.ToImmutableList();
    }

    private IImmutableList<string> GenerateTestMethods(Type type)
    {
        var lines = new List<string>();

        var publicMethods = type.GetMethods()
            .Where(m => m.IsPublic && !SutMethodNamesToIgnore.Contains(m.Name))
            .ToImmutableList();
        foreach (var method in publicMethods)
        {
            lines.AddRange(GenerateTestMethod(method));
        }

        return lines.ToImmutableList();
    }

    private IImmutableList<string> GenerateTestMethod(MethodInfo methodInfo)
    {
        var lines = new List<string>();

        var isAsync = methodInfo.ReturnType.Name == "Task`1";

        var methodQualifyer = isAsync
            ? "public async Task"
            : "public void";

        lines.Add($"{_config.Indent}[TestMethod]");
        lines.Add($"{_config.Indent}{methodQualifyer} {methodInfo.Name}()");
        lines.Add($"{_config.Indent}{{");
        lines.Add($"{_config.Indent}{_config.Indent}// Arrange");
        lines.Add($"{_config.Indent}{_config.Indent}var setup = new TestSetup();");
        lines.Add(string.Empty);
        lines.Add($"{_config.Indent}{_config.Indent}// Act");

        var awaitText = isAsync ? "await " : string.Empty;
        lines.Add($"{_config.Indent}{_config.Indent}var result = {awaitText}setup.Sut." + methodInfo.Name + "();");
        lines.Add(string.Empty);
        lines.Add($"{_config.Indent}{_config.Indent}// Assert");
        lines.Add($"{_config.Indent}}}");
        lines.Add(string.Empty);

        return lines.ToImmutableList();
    }

    private IImmutableList<string> GenerateTestSetup(Type type)
    {
        var constructor = type.GetConstructors().Single();
        var constructorParameters = constructor.GetParameters();

        var lines = new List<string>();

        lines.Add("private sealed class TestSetup");
        lines.Add("{");
        lines.Add($"{_config.Indent}public {type.Name} Sut {{ get; }}");
        lines.Add(string.Empty);
        lines.AddRange(getMockFieldLines(constructorParameters));
        lines.Add(string.Empty);
        lines.Add($"{_config.Indent}public TestSetup()");
        lines.Add($"{_config.Indent}{{");
        lines.AddRange(getClassInstantiation(type, constructorParameters));
        lines.Add($"{_config.Indent}}}");
        lines.Add("}");

        return lines
            .Select(line => string.IsNullOrEmpty(line) ? line : $"{_config.Indent}{line}")
            .ToImmutableList();
    }

    private IImmutableList<string> getMockFieldLines(ParameterInfo[] constructorParameters)
    {
        return constructorParameters
            .Select(p => $"{_config.Indent}private readonly Mock<{p.ParameterType.Name}> _{p.Name}Mock = new();")
            .ToImmutableList();
    }

    private IImmutableList<string> getClassInstantiation(Type type, ParameterInfo[] constructorParameters)
    {
        var lines = new List<string>();
        if (constructorParameters.Length == 0)
        {
            lines.Add($"{_config.Indent}{_config.Indent}Sut = new {type.Name}();");
            return lines.ToImmutableList();
        }

        lines.Add($"{_config.Indent}{_config.Indent}Sut = new {type.Name}(");
        for (int i = 0; i < constructorParameters.Length; i++)
        {
            var eol = i == constructorParameters.Length - 1 ? ");" : ",";
            lines.Add($"{_config.Indent}{_config.Indent}{_config.Indent}_{constructorParameters[i].Name}Mock.Object{eol}");
        }

        return lines.ToImmutableList();
    }
}