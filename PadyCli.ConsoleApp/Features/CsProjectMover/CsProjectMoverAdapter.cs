using CsProjMover;

namespace PadyCli.ConsoleApp.Features.CsProjectMover;

internal class CsProjectMoverAdapter
{
    private readonly IProjectMoverService _moverService;

    public CsProjectMoverAdapter(IProjectMoverService moverService)
    {
        _moverService = moverService;
    }

    public int Run(CsProjectMoverOptions options)
    {
        _moverService.MoveProject(options.RelativePathToSourceFolder, options.RelativePathToTargetFolder);

        return 1;
    }
}