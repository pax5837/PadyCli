using System.Collections.Immutable;
using System.Reflection;
using DotnetInfrastructure.Contracts;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace DotnetInfrastructure;

internal class AssemblyLoader : IAssemblyLoader
{
    private readonly ILogger<AssemblyLoader> _logger;

    public AssemblyLoader(ILogger<AssemblyLoader> logger)
    {
        _logger = logger;
    }

    public async Task<GetAssemblyResponse> GetAssembly(string projectDirectoryPath, bool includeReferencedAssemblies, CancellationToken cancellationToken)
    {
        var pathToCsProjFile = FindCsprojFile(projectDirectoryPath);
        if (pathToCsProjFile is null)
        {
            throw new InvalidOperationException("No csproj file found.");
        }

        var projectDirectory = Directory.GetParent(pathToCsProjFile);
        if (projectDirectory is null)
        {
            throw new InvalidOperationException("No project directory found.");
        }

        var projectName = pathToCsProjFile.Split("\\").Last().Replace(".csproj", string.Empty);
        _logger.LogInformation("Found project file: {ProjectName}, in directory: {ProjectDirectory}",
            projectName,
            projectDirectory);

        var pathToDll = PickDllAndReturnPath(projectDirectory, projectName);
        var pathToDllFolder = Directory.GetParent(pathToDll).FullName;
        var assemblyLoadContext = new CustomAssemblyLoadContext(pathToDllFolder);
        var mainAssembly = assemblyLoadContext.LoadFromAssemblyPath(pathToDll);

        await Task.Delay(1);

        if (!includeReferencedAssemblies)
        {
            return new GetAssemblyResponse(mainAssembly, new List<Assembly>());
        }

        var referencedAssemblies = LoadReferencedAssemblies(mainAssembly, pathToDll, pathToDllFolder, assemblyLoadContext);
        return new GetAssemblyResponse(mainAssembly, referencedAssemblies);
    }

    private List<Assembly> LoadReferencedAssemblies(Assembly mainAssembly, string pathToDll, string pathToDllFolder,
        CustomAssemblyLoadContext assemblyLoadContext)
    {
        var referencedAssemblies = new List<Assembly>();
        var referencedAssemblyNames = mainAssembly.GetReferencedAssemblies().Where(ra => ra.Name is not null);
        foreach (var assemblyName in referencedAssemblyNames)
        {
            var pathToReferencedDll = Path.Combine(pathToDllFolder, $"{assemblyName.Name}.dll");
            if (File.Exists(pathToReferencedDll))
            {
                try
                {
                    var assy = assemblyLoadContext.LoadFromAssemblyPath(pathToReferencedDll);
                    referencedAssemblies.Add(assy);
                }
                catch (Exception)
                {
                    _logger.LogError("Could not load {DLL}", pathToReferencedDll);
                }
            }
        }

        return referencedAssemblies;
    }

    private string PickDllAndReturnPath(
        DirectoryInfo projectDirectory,
        string projectName)
    {
        var binDirectory = Path.Combine(projectDirectory!.FullName, "bin");
        var dllsFound = Directory
            .EnumerateFiles(
                path: binDirectory,
                searchPattern: $"{projectName}.dll",
                searchOption: SearchOption.AllDirectories)
            .ToImmutableList();
        if (!dllsFound.Any())
        {
            throw new InvalidOperationException(
                $"Could not find a dll in the directory: {binDirectory}, try to build your project.");
        }

        return RequestUserToPickDll(dllsFound);
    }

    private string RequestUserToPickDll(IImmutableList<string> dllsFound)
    {
        while (true)
        {
            Console.WriteLine("\n\nPick a dll");
            var widthForDll = dllsFound.Max(x => x.Length) + 1;
            for (int i = 1; i <= dllsFound.Count(); i++)
            {
                var lastWriteTime = new FileInfo(dllsFound[i - 1]).LastWriteTime;
                Console.WriteLine($"  [{i}] - {dllsFound[i - 1].PadRight(widthForDll, ' ')} - last written: {lastWriteTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
            }

            Console.WriteLine($"  [x] - to exit");

            var input = Console.ReadLine()?.ToLower();

            if (input is null)
            {
                continue;
            }

            if (input.Equals("x"))
            {
                throw new OperationCanceledException("Canceled by user");
            }

            if (int.TryParse(input, out int pick))
            {
                if (pick >= 1 && pick <= dllsFound.Count())
                {
                    var picked = dllsFound[pick - 1];
                    _logger.LogInformation($"Picked dll: {picked}");
                    return picked;
                }
            }
        }
    }

    static string? FindCsprojFile(string startDirectory)
    {
        string currentDirectory = startDirectory;

        while (!string.IsNullOrEmpty(currentDirectory))
        {
            // Look for .csproj files in the current directory
            string[] csprojFiles = Directory.GetFiles(currentDirectory, "*.csproj");

            if (csprojFiles.Length > 0)
            {
                // Return the first .csproj file found
                return csprojFiles[0];
            }

            // Move up to the parent directory
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        // If no .csproj file is found, return null
        return null;
    }

    private MSBuildWorkspace CreateMSBuildWorkspace()
    {
        // Optionally configure the workspace with necessary settings
        var properties = new Dictionary<string, string>
        {
            { "Configuration", "Debug" },
            { "Platform", "AnyCPU" }
        };

        MSBuildWorkspace workspace = MSBuildWorkspace.Create(properties);
        _logger.LogInformation("MSBuildWorkspace created successfully.");
        return workspace;
    }

    private static Assembly? ResolveAssembly(string assemblyName)
    {
        var deps = DependencyContext.Default;
        var assembly = new AssemblyName(assemblyName);

        // Tries to resolve the assembly from Deployment arefacd penReault
        foreach (var library in deps.RuntimeLibraries)
        {
            foreach (var name in library.GetDefaultAssemblyNames(deps))
            {
                if (name.FullName == assembly.FullName)
                {
                    return Assembly.Load(name);
                }
            }
        }

        return null;
    }
}