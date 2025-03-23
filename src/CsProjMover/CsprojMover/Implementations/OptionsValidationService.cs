using Infrastructure;
using Infrastructure.Either;

namespace CsProjMover.Implementations;

internal class OptionsValidationService : IOptionsValidationService
{
    public Either<CsProjectMoverOptions, GenericError> ValidateOptions(CsProjectMoverOptions options)
    {
        var currentProjectDirectoryFullPath = Directory.GetParent(options.RelativePathToSourceFolder).FullName;
        if (!currentProjectDirectoryFullPath.StartsWith(Directory.GetCurrentDirectory()))
            return new GenericError(
                "FileOrDirectoryNotFound",
                $"Project directory {currentProjectDirectoryFullPath} must be below current current directory {Directory.GetCurrentDirectory()}");

        var targetProjectDirectoryFullPath = Path.GetFullPath(options.RelativePathToTargetFolder);
        if (!targetProjectDirectoryFullPath.StartsWith(Directory.GetCurrentDirectory()))
            return new GenericError(
                "FileOrDirectoryNotFound",
                $"Target project directory {targetProjectDirectoryFullPath} must be below current current directory {Directory.GetCurrentDirectory()}");

        if (Directory.Exists(options.RelativePathToTargetFolder)
            && DirectoryIsNotEmpty(options.RelativePathToTargetFolder))
            return new GenericError(
                "FileOrDirectoryNotFound",
                $"Target directory {options.RelativePathToTargetFolder} must be empty");

        return options;
    }

    private static bool DirectoryIsNotEmpty(string pathToDirectory)
    {
        return Directory
            .GetFiles(
                pathToDirectory,
                "*,*",
                SearchOption.AllDirectories)
            .Any();
    }
}