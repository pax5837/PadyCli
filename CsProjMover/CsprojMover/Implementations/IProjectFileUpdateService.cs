namespace CsProjMover.Implementations;

public interface IProjectFileUpdateService
{
    void UpdateAllDependentProjects(
        string fullPathToStartDirectory,
        string oldFullPathToFolder,
        string newFullPathToFolder,
        string project);

    void UpdateDependenciesInMovedProject(
        string fullPathToStartDirectory,
        string oldFullPathToProjectFolder,
        string newFullPathToProjectFolder,
        string project);
}