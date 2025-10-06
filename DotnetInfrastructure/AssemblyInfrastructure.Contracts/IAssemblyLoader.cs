namespace DotnetInfrastructure.Contracts;

public interface IAssemblyLoader
{
    /// <summary>
    /// Gets an assembly for the first project found in the start directory, or any of its parents.
    /// </summary>
    /// <param name="projectDirectoryPath">The directory containing the project file (csproj). There can be only one csproj file in that folder.</param>
    /// <param name="includeReferencedAssemblies">Whether to include referenced assemblies.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns></returns>
    Task<GetAssemblyResponse> GetAssembly(
        string projectDirectoryPath,
        bool includeReferencedAssemblies,
        CancellationToken cancellationToken);
}