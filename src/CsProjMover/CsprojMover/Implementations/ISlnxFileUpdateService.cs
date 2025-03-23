namespace CsProjMover.Implementations;

public interface ISlnxFileUpdateService
{
    void UpdateAllSlnx(
        string fullPathToStartDirectory,
        string oldFullPathToProjectFolder,
        string newFullPathToProjectFolder,
        string projectName);
}