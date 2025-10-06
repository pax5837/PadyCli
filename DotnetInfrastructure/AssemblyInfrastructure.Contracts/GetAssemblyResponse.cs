using System.Reflection;

namespace DotnetInfrastructure.Contracts;

public class GetAssemblyResponse
{
    public Assembly MainAssembly { get; }
    public IReadOnlyList<Assembly> ReferencedAssemblies { get; }

    public GetAssemblyResponse(Assembly mainAssembly, IReadOnlyList<Assembly> referencedAssemblies)
    {
        MainAssembly = mainAssembly;
        ReferencedAssemblies = referencedAssemblies;
    }
}