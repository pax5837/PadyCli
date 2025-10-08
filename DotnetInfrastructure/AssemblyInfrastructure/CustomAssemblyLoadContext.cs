using System.Reflection;
using System.Runtime.Loader;

namespace DotnetInfrastructure;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    private readonly string _assemblyDirectory;
    private readonly HashSet<string> _loadedAssemblies = new();

    public CustomAssemblyLoadContext(string assemblyDirectory) : base(isCollectible: false)
    {
        _assemblyDirectory = assemblyDirectory;
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        string assemblyPath = Path.Combine(_assemblyDirectory, assemblyName.Name + ".dll");
        if (File.Exists(assemblyPath))
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null; // Fallback to default context
    }

    public void LoadAssemblyAndDependencies(string assemblyPath)
    {
        Assembly assembly = LoadFromAssemblyPath(assemblyPath);
        if (assembly != null)
        {
            LoadReferencedAssemblies(assembly);
        }
    }

    private void LoadReferencedAssemblies(Assembly assembly)
    {
        foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
        {
            if (!_loadedAssemblies.Contains(referencedAssembly.FullName))
            {
                try
                {
                    Assembly loadedAssembly = Load(referencedAssembly);
                    if (loadedAssembly != null)
                    {
                        _loadedAssemblies.Add(referencedAssembly.FullName);
                        LoadReferencedAssemblies(loadedAssembly);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load assembly {referencedAssembly.FullName}: {ex.Message}");
                }
            }
        }
    }
}