using Infrastructure.DebugServices;

namespace CsProjMover.Implementations;

internal class ProjectFileUpdateService : IProjectFileUpdateService
{
    private readonly IDebugService debugService;

    public ProjectFileUpdateService(IDebugService debugService)
    {
        this.debugService = debugService;
    }

    public void UpdateAllDependentProjects(
        string fullPathToStartDirectory,
        string oldFullPathToFolder,
        string newFullPathToFolder,
        string project)
    {
        var projectFiles = Directory.GetFiles(
            fullPathToStartDirectory,
            "*.csproj",
            SearchOption.AllDirectories);

        foreach (var projectFile in projectFiles)
            UpdateDependentProject(projectFile, oldFullPathToFolder, newFullPathToFolder, project);
    }

    public void UpdateDependenciesInMovedProject(
        string fullPathToStartDirectory,
        string oldFullPathToProjectFolder,
        string newFullPathToProjectFolder,
        string project)
    {
        var currentProjectLocation = Directory.Exists(oldFullPathToProjectFolder)
            ? oldFullPathToProjectFolder
            : newFullPathToProjectFolder;

        var originalProjectContentLines = File.ReadAllLines(Path.Combine(currentProjectLocation, project));

        if (!originalProjectContentLines.Any(l => l.Contains(Path.Combine(oldFullPathToProjectFolder, project))))
            return;

        var newProjectContentLines = originalProjectContentLines.Select(line =>
            UpdateLine(line, oldFullPathToProjectFolder, newFullPathToProjectFolder));

        if (!originalProjectContentLines.SequenceEqual(newProjectContentLines)
            && debugService.FileSystemModificationsAllowed) File.WriteAllLines(project, newProjectContentLines);
    }

    private string UpdateLine(string line, string oldFullPathToProject, string newFullPathToProject)
    {
        if (!line.Contains('"')) return line;
        var parts = line.Split('"');
        return parts[1].Contains(".csproj")
            ? GetNewProjectLine(line, oldFullPathToProject, newFullPathToProject)
            : line;
    }

    private string GetNewProjectLine(string line, string oldFullPathToProject, string newFullPathToProject)
    {
        var parts = line.Split('"');
        var oldProjectPath = parts[1];
        var fullPathToReferencedProject = Path.Combine(
            Directory.GetParent(oldFullPathToProject).FullName,
            oldProjectPath);
        var newProjectPath = Path.GetRelativePath(
            Directory.GetParent(newFullPathToProject).FullName,
            fullPathToReferencedProject);
        return line.Replace(oldProjectPath, newProjectPath);
    }

    private void UpdateDependentProject(
        string fullPathToProjectFile,
        string oldFullPathToProject,
        string newFullPathToProject,
        string movedProjectName)
    {
        var projectDirectory = Directory.GetParent(fullPathToProjectFile).FullName;
        var oldProjectRelativePath =
            Path.Combine(Path.GetRelativePath(projectDirectory, oldFullPathToProject), movedProjectName);
        var newProjectRelativePath =
            Path.Combine(Path.GetRelativePath(projectDirectory, newFullPathToProject), movedProjectName);
        var originalProjectContent = File.ReadAllText(fullPathToProjectFile);
        if (!originalProjectContent.Contains(oldProjectRelativePath)) return;

        if (debugService.FileSystemModificationsAllowed)
        {
            var newProjectContent = originalProjectContent.Replace(oldProjectRelativePath, newProjectRelativePath);
            File.WriteAllText(fullPathToProjectFile, newProjectContent);
        }
    }
}