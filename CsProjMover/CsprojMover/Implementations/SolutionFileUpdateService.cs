using Infrastructure.DebugServices;

namespace CsProjMover.Implementations;

internal class SolutionFileUpdateService : ISolutionFileUpdateService
{
    private readonly IDebugService debugService;

    public SolutionFileUpdateService(IDebugService debugService)
    {
        this.debugService = debugService;
    }

    public void UpdateAllSolutions(
        string fullPathToStartDirectory,
        string oldFullPathToProjectFolder,
        string newFullPathToProjectFolder,
        string projectName)
    {
        var solutionFiles = Directory.GetFiles(
            fullPathToStartDirectory,
            "*.sln",
            SearchOption.AllDirectories);

        foreach (var solutionFile in solutionFiles)
        {
            UpdateSolution(
                solutionFile,
                oldFullPathToProjectFolder,
                newFullPathToProjectFolder,
                projectName);
        }
    }

    private void UpdateSolution(string fullPathToSolutionFile, string oldFullPathToProject, string newFullPathToProject,
        string projectName)
    {
        var solutionDirectory = Directory.GetParent(fullPathToSolutionFile).FullName;
        var oldProjectRelativePath =
            Path.Combine(Path.GetRelativePath(solutionDirectory, oldFullPathToProject), projectName);
        var newProjectRelativePath =
            Path.Combine(Path.GetRelativePath(solutionDirectory, newFullPathToProject), projectName);
        var originalSolutionContent = File.ReadAllText(fullPathToSolutionFile);

        if (!originalSolutionContent.Contains(oldProjectRelativePath)) return;

        var newSolutionContent = originalSolutionContent.Replace(oldProjectRelativePath, newProjectRelativePath);

        if (debugService.FileSystemModificationsAllowed) File.WriteAllText(fullPathToSolutionFile, newSolutionContent);
    }
}