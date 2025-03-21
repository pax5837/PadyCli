using System.Collections.Immutable;
using System.Reflection;

namespace TestingHelpers.Services.TestClassGeneration;

internal class TestClassCodeGenerator : ITestClassCodeGenerator
{
    private static readonly IImmutableSet<string> SutMethodNamesToIgnore =
        ["GetType", "ToString", "Equals", "GetHashCode"];

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

    private static IImmutableList<string> GenerateTestMethods(Type type)
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

    private static IImmutableList<string> GenerateTestMethod(MethodInfo methodInfo)
    {
        var lines = new List<string>();

        var isAsync = methodInfo.ReturnType.Name == "Task`1";

        var methodQualifyer = isAsync
            ? "public async Task"
            : "public void";

        lines.Add("\t[TestMethod]");
        lines.Add($"\t{methodQualifyer} {methodInfo.Name}()");
        lines.Add("\t{");
        lines.Add("\t\t// Arrange");
        lines.Add("\t\tvar setup = new TestSetup();");
        lines.Add(string.Empty);
        lines.Add("\t\t// Act");

        var awaitText = isAsync ? "await " : string.Empty;
        lines.Add($"\t\tvar result = {awaitText}setup.Sut." + methodInfo.Name + "();");
        lines.Add(string.Empty);
        lines.Add("\t\t// Assert");
        lines.Add("\t}");
        lines.Add(string.Empty);

        return lines.ToImmutableList();
    }

    private static IImmutableList<string> GenerateTestSetup(Type type)
    {
        var constructor = type.GetConstructors().Single();
        var constructorParameters = constructor.GetParameters();

        var lines = new List<string>();

        lines.Add("private sealed class TestSetup");
        lines.Add("{");
        lines.Add($"\tpublic {type.Name} Sut {{ get; }}");
        lines.Add(string.Empty);
        lines.AddRange(getMockFieldLines(constructorParameters));
        lines.Add(string.Empty);
        lines.Add($"\tpublic TestSetup()");
        lines.Add("\t{");
        lines.AddRange(getClassInstantiation(type, constructorParameters));
        lines.Add("\t}");
        lines.Add("}");

        return lines
            .Select(line => string.IsNullOrEmpty(line) ? line : $"\t{line}")
            .ToImmutableList();
    }

    private static IImmutableList<string> getMockFieldLines(ParameterInfo[] constructorParameters)
    {
        return constructorParameters
            .Select(p => $"\tprivate readonly Mock<{p.ParameterType.Name}> _{p.Name}Mock = new();")
            .ToImmutableList();
    }

    private static IImmutableList<string> getClassInstantiation(Type type, ParameterInfo[] constructorParameters)
    {
        var lines = new List<string>();
        if (constructorParameters.Length == 0)
        {
            lines.Add($"\t\tSut = new {type.Name}();");
            return lines.ToImmutableList();
        }

        lines.Add($"\t\tSut = new {type.Name}(");
        for (int i = 0; i < constructorParameters.Length; i++)
        {
            var eol = i == constructorParameters.Length - 1 ? ");" : ",";
            lines.Add($"\t\t\t_{constructorParameters[i].Name}Mock.Object{eol}");
        }

        return lines.ToImmutableList();
    }
}