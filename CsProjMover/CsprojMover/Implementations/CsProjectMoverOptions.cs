namespace CsProjMover.Implementations;

public record CsProjectMoverOptions(
    string RelativePathToSourceFolder,
    string RelativePathToTargetFolder);