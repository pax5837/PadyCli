namespace CsProjMover;

public interface IProjectMoverService
{
    void MoveProject(
        string sourceDirectory,
        string targetDirectory);
}