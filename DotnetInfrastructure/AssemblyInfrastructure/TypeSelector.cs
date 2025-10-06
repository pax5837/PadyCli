using System.Collections.Immutable;
using System.Reflection;
using DotnetInfrastructure.Contracts;
using Infrastructure;
using Infrastructure.Either;
using Microsoft.Extensions.Logging;

namespace DotnetInfrastructure;

internal class TypeSelector : ITypeSelector
{
    private readonly AssemblyInfo _assemblyInfo;
    private readonly ILogger<TypeSelector> _logger;

    public TypeSelector(
        AssemblyInfo assemblyInfo,
        ILogger<TypeSelector> logger)
    {
        _assemblyInfo = assemblyInfo;
        _logger = logger;
    }

    public Either<Type, ExitRequested> SelectType(string typeIdentifier, Assembly assembly)
    {
        var types = GetTypes(assembly).ToImmutableList();

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogTrace("Found {TypeCount} types:\n{AllTypes}",
                types.Count,
                string.Join("\n", types.Select(t => $"  - {t.Name}")));
        }

        var candidateTypes = types
            .Where(t => t.FullName != null && t.FullName.EndsWith(typeIdentifier, StringComparison.OrdinalIgnoreCase))
            .ToImmutableList();

        if (!candidateTypes.Any())
        {
            throw new InvalidOperationException(
                $"\n\nNo type found matching '{typeIdentifier}'."
                + "\nThis error could be cause by a type located in a dll from a nuget package, you could:"
                + "\n- use <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies> in your csproj"
                + "\n- run a `dotnet publish` or `dotnet publish --self-contained true`, and pick the dll in the publish directory."
                + "\n\n\n");
        }

        if (candidateTypes.Count > 1)
        {
            var userInputResult = UserInputResult.InvalidInput;
            int index = -1;
            while (userInputResult != UserInputResult.ValidInput && userInputResult != UserInputResult.Exit)
            {
                (index, userInputResult) = GetUserInput(candidateTypes);
            }

            if (userInputResult == UserInputResult.Exit)
            {
                return new ExitRequested();
            }

            return candidateTypes[index - 1];
        }

        return candidateTypes.Single();
    }

    private Type[] GetTypes(Assembly assembly)
    {
        try
        {
            List<Type> allTypes = new List<Type>();
            allTypes.AddRange(assembly.GetTypes());

            var referencedAssemblies = assembly.GetReferencedAssemblies()
                .Where(ra => ra.Name is not null && ra.Name.StartsWith("MT.LXC.RA.MetaData.ReadWrite"))
                .ToImmutableList();
            foreach (var referencedAssembly in referencedAssemblies)
            {
                var pathToDll = Path.Combine(_assemblyInfo.PathToBinFolder, $"{referencedAssembly.Name}.dll");
                if (File.Exists(pathToDll))
                {
                    try
                    {
                        var assy = _assemblyInfo.AssemblyLoadContext.LoadFromAssemblyPath(pathToDll);
                        allTypes.AddRange(assy.GetExportedTypes());
                    }
                    catch (Exception)
                    {
                        _logger.LogWarning("Could not load {DLL}", pathToDll);
                    }
                }
            }

            var array = allTypes.Distinct().ToArray();
            return array;
        }
        catch (ReflectionTypeLoadException e)
        {
            _logger.LogWarning(
                "Failed to get all types normally, as some files could not be loaded. Loading only some types");
            return e.Types.OfType<Type>().ToArray();
        }
    }

    private (int SelectedIndex, UserInputResult UserInputResult) GetUserInput(ImmutableList<Type> candidateTypes)
    {
        Console.WriteLine("\n\nMultiple possible matches found, select which one to use:");
        foreach (var candidateType in candidateTypes.Select((x, i) => (x, i + 1)))
        {
            Console.WriteLine($" [{candidateType.Item2}] {candidateType.x.FullName}");
        }

        Console.WriteLine(" [x] to exit");

        var reply = Console.ReadLine();
        if (reply.Equals("x", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Exit");
            return (-1, UserInputResult.Exit);
        }

        if (int.TryParse(reply, out var index))
        {
            if (index < 1 || index > candidateTypes.Count())
            {
                _logger.LogInformation("Invalid input");
                return (-1, UserInputResult.InvalidInput);
            }

            _logger.LogInformation($"Picked {candidateTypes[index - 1].FullName}");
            return (index, UserInputResult.ValidInput);
        }

        _logger.LogInformation("Invalid input");
        return (-1, UserInputResult.InvalidInput);
    }

    private enum UserInputResult
    {
        ValidInput,
        InvalidInput,
        Exit,
    }
}