using CommandLine;

namespace PadyCli.ConsoleApp.Features.CsProjectMover;

[Verb("movecsproj", HelpText = "moves a csproj")]
public class CsProjectMoverOptions
{
    [Option('s', "source", Required = true, HelpText = "Relative Path to the source folder containing a project file.")]
    public string RelativePathToSourceFolder { get; set; }

    [Option('t', "target", Required = true, HelpText = "Relative path to the target folder")]
    public string RelativePathToTargetFolder { get; set; }
}