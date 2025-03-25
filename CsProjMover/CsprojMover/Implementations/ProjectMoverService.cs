using Infrastructure;
using Infrastructure.DebugServices;
using Infrastructure.Piping;

namespace CsProjMover.Implementations;

internal class ProjectMoverService : IProjectMoverService
{
    private readonly IDebugService debugService;
    private readonly IOptionsValidationService optionsValidationService;
    private readonly IProjectFileUpdateService projectFileUpdateService;
    private readonly ISolutionFileUpdateService solutionFileUpdateService;
    private readonly ISlnxFileUpdateService slnxFileUpdateService;

    public ProjectMoverService(
        ISolutionFileUpdateService solutionFileUpdateService,
        ISlnxFileUpdateService slnxFileUpdateService,
        IProjectFileUpdateService projectFileUpdateService,
        IOptionsValidationService optionsValidationService,
        IDebugService debugService)
    {
        this.solutionFileUpdateService = solutionFileUpdateService;
        this.slnxFileUpdateService = slnxFileUpdateService;
        this.projectFileUpdateService = projectFileUpdateService;
        this.optionsValidationService = optionsValidationService;
        this.debugService = debugService;
    }

    public void MoveProject(
        string sourceDirectory,
        string targetDirectory)
    {
        new CsProjectMoverOptions(sourceDirectory, targetDirectory)
            .Pipe(optionsValidationService.ValidateOptions)
            .Pipe(CalculateAdditionalParameters)
            .Pipe(MoveProjectToNewDirectory)
            .Pipe(UpdateProjectFile)
            .Pipe(UpdateDependentProjects)
            .Pipe(UpdateAllSolutions)
            .Pipe(UpdateAllSlnxFiles)
            .Pipe(WriteErrorToConsole);
    }

    private void MoveProjectToNewDirectory(Parameters parameters)
    {
        if (!(debugService.FileSystemModificationsAllowed))
        {
            return;
        }

        Directory.Delete(Path.Combine(parameters.Options.RelativePathToSourceFolder, "bin"), true);
        Directory.Delete(Path.Combine(parameters.Options.RelativePathToSourceFolder, "obj"), true);

        if (!Directory.Exists(parameters.TargetsParentDirectoryFullPath))
        {
            Directory.CreateDirectory(parameters.TargetsParentDirectoryFullPath);
        }

        Directory.Move(
            parameters.Options.RelativePathToSourceFolder,
            parameters.Options.RelativePathToTargetFolder);
    }

    private void UpdateAllSolutions(Parameters parameters)
    {
        solutionFileUpdateService.UpdateAllSolutions(
            parameters.CurrentDirectory,
            parameters.Options.RelativePathToSourceFolder,
            parameters.Options.RelativePathToTargetFolder,
            parameters.ProjectFileName);
    }

    private void UpdateAllSlnxFiles(Parameters parameters)
    {
        slnxFileUpdateService.UpdateAllSlnx(
            parameters.CurrentDirectory,
            parameters.Options.RelativePathToSourceFolder,
            parameters.Options.RelativePathToTargetFolder,
            parameters.ProjectFileName);
    }

    private void UpdateDependentProjects(Parameters parameters)
    {
        projectFileUpdateService.UpdateAllDependentProjects(
            parameters.CurrentDirectory,
            parameters.Options.RelativePathToSourceFolder,
            parameters.Options.RelativePathToTargetFolder,
            parameters.ProjectFileName);
    }

    private void UpdateProjectFile(Parameters parameters)
    {
        projectFileUpdateService.UpdateDependenciesInMovedProject(
            parameters.CurrentDirectory,
            parameters.Options.RelativePathToSourceFolder,
            parameters.Options.RelativePathToTargetFolder,
            parameters.ProjectFileName);
    }

    private void WriteErrorToConsole(GenericError genericError)
    {
        foreach (var error in genericError.Errors) Console.WriteLine(error.errorValue);
    }

    private Parameters CalculateAdditionalParameters(CsProjectMoverOptions options)
    {
        var targetDirFullPath = Path.GetFullPath(options.RelativePathToTargetFolder);
        var parentOfTarget = Directory.GetParent(targetDirFullPath)!.FullName;

        var fullPathToProjectFile = Directory
            .GetFiles(
                options.RelativePathToSourceFolder,
                "*.csproj",
                SearchOption.TopDirectoryOnly)
            .Single();

        return new Parameters(
            Directory.GetCurrentDirectory(),
            parentOfTarget,
            Path.GetFileName(fullPathToProjectFile),
            options);
    }

    private record Parameters(
        string CurrentDirectory,
        string TargetsParentDirectoryFullPath,
        string ProjectFileName,
        CsProjectMoverOptions Options);
}