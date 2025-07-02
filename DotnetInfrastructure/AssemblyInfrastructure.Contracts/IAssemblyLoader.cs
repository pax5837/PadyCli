using System.Reflection;

namespace DotnetInfrastructure.Contracts;

public interface IAssemblyLoader
{
    /// <summary>
    /// Gets an assembly for the first project found in the start directory, or any of its parents.
    /// </summary>
    /// <param name="startDirectory">The start directory.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <param name="useOnTheFlyCompilation">Whether to use on the fly compilation. When set true it might not be able to load all dlls (nuget) and thus fail. When set to true, the user need to make sure the targeted project was built.</param>
    /// <returns></returns>
    Task<Assembly> GetAssembly(string startDirectory, CancellationToken cancellationToken,
        bool useOnTheFlyCompilation = false);
}