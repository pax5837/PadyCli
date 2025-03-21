namespace CsProjMover.Implementations;

public interface ISolutionFileUpdateService
{
    void UpdateAllSolutions(
        string fullPathToStartDirectory,
        string oldFullPathToProjectFolder,
        string newFullPathToProjectFolder,
        string projectName);
}