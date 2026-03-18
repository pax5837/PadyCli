using System.Collections.Immutable;
using Spectre.Console;

namespace PadyCliInteractiveConsoleApp.Features.Docker;

internal class DockerService
{
    private const string NAME_PLACEHOLDER = "###%%NAME%%%###";
    private const string PORT_PLACEHOLDER = "###%%PORT%%%###";
    private const string IMAGE_PLACEHOLDER = "###%%IMAGE%%%###";

    private static IImmutableList<DockerStartContainerConfig> configs =
    [
        new DockerStartContainerConfig(
            Description: "'Seq', a logging sink",
            DefaultContainerName: "Seq",
            DefaultPort: 63000,
            Image: "datalust/seq:latest",
            DockerRunCommands: [$"docker run -d --restart unless-stopped --name {NAME_PLACEHOLDER} -e ACCEPT_EULA=Y -v C:/temp/data -p {PORT_PLACEHOLDER}:80  {IMAGE_PLACEHOLDER}"]),
        new DockerStartContainerConfig(
            Description: "PlantUML server",
            DefaultContainerName: "Plantuml",
            DefaultPort: 63001,
            Image: "plantuml/plantuml-server:jetty-v1.2022.13",
            DockerRunCommands: [$"docker run -d --restart unless-stopped -p {PORT_PLACEHOLDER}:8080 --name {NAME_PLACEHOLDER} {IMAGE_PLACEHOLDER}"]),
        new DockerStartContainerConfig(
            Description: "Kroki server, to display diagrams in browser with extension MArkdown diagrams",
            DefaultContainerName: "Kroki",
            DefaultPort: 63007,
            Image: "yuzutech/kroki",
            DockerRunCommands: [$"docker run -d --restart unless-stopped -p {PORT_PLACEHOLDER}:8000  --name {NAME_PLACEHOLDER} {IMAGE_PLACEHOLDER}"]),
        new DockerStartContainerConfig(Description: "MS SQL Server",
            DefaultContainerName: "MS SQL DB",
            DefaultPort: 63002,
            Image: "mcr.microsoft.com/mssql/server:2022-latest",
            DockerRunCommands: [$"docker run -e \"ACCEPT_EULA=Y\" -e \"MSSQL_SA_PASSWORD=pass1234!\" -p {PORT_PLACEHOLDER}:1433 -d --restart unless-stopped --name {NAME_PLACEHOLDER} -v e:/databases/db1/data:/var/opt/mssql/data -v e:/databases/db1/log:/var/opt/mssql/log  {IMAGE_PLACEHOLDER}"]),
        new DockerStartContainerConfig(
            Description: "Swagger editor",
            DefaultContainerName: "SwaggerEditor",
            DefaultPort: 63003,
            Image: "docker.swagger.io/swaggerapi/swagger-ui",
            DockerRunCommands: [$"docker run -d --name {NAME_PLACEHOLDER} -p {PORT_PLACEHOLDER}:8080 {IMAGE_PLACEHOLDER}"]),
        new DockerStartContainerConfig(
            Description: "Mongo DB with admin (takes 2 consecutive ports)",
            DefaultContainerName: "MongoDB",
            DefaultPort: 63004,
            Image: "mongo",
            DockerRunCommands:
            [
                $"docker network create -d bridge {NAME_PLACEHOLDER}Network",
                $"docker run -d --name {NAME_PLACEHOLDER} -p {PORT_PLACEHOLDER}:27017 --network {NAME_PLACEHOLDER}Network {IMAGE_PLACEHOLDER}",
                $"docker run -d --name {NAME_PLACEHOLDER}Admin -p {PORT_PLACEHOLDER}:8081 -e ME_CONFIG_MONGODB_SERVER={NAME_PLACEHOLDER} --network {NAME_PLACEHOLDER}Network -e ME_CONFIG_BASICAUTH_USERNAME=\"user\" -e ME_CONFIG_BASICAUTH_PASSWORD=\"pass1234\" mongo-express",
            ]),
        new DockerStartContainerConfig(
            Description: "ItTools",
            DefaultContainerName: "it-tools",
            DefaultPort: 63006,
            Image: "corentinth/it-tools",
            DockerRunCommands: [$"docker run -d -p {PORT_PLACEHOLDER}:80 --name {NAME_PLACEHOLDER} -it {IMAGE_PLACEHOLDER}"]),
    ];

    private readonly ProcessRunner _processRunner;

    public DockerService(ProcessRunner processRunner)
    {
        _processRunner = processRunner;
    }

    public OperationResult Run()
    {
        IImmutableDictionary<int, (DockerStartContainerConfig Config, int Index)> numberedConfigs
            = configs.Select((c, i) => (c, i + 1)).ToImmutableDictionary(x => x.Item2, x => x);

        while (true)
        {
            var operationResult = RunOneDockerContainer(numberedConfigs);
            
            if (operationResult == OperationResult.Abort)
            {
                return OperationResult.Continue;
            }

            if (operationResult == OperationResult.ExitApplication)
            {
                return OperationResult.ExitApplication;
            }
        }
    }

    private OperationResult RunOneDockerContainer(
        IImmutableDictionary<int, (DockerStartContainerConfig Config, int Index)> numberedConfigs)
    {
        AnsiConsole.MarkupLine("Listing current docker containers");
        _processRunner.Run("docker ps");

        var input = ReadDockerConfig(numberedConfigs);

        if (input.OperationResult == OperationResult.ExitApplication)
        {
            return OperationResult.ExitApplication;
        }

        if (input.OperationResult == OperationResult.Abort)
        {
            return OperationResult.Abort;
        }

        var (config, _) = numberedConfigs.Values.Single(x => x.Config.Description == input.Description);

        var containerName = ConfirmContainerName(config.DefaultContainerName);

        if (containerName is null)
        {
            return OperationResult.Abort;
        }

        var port = ConfirmPort(config.DefaultPort);

        if (port is null)
        {
            return OperationResult.Abort;
        }

        foreach (var dockerRunCommand in config.DockerRunCommands)
        {
            var containsPort = dockerRunCommand.Contains(PORT_PLACEHOLDER);
            var mappedCommand = dockerRunCommand
                .Replace(IMAGE_PLACEHOLDER, config.Image)
                .Replace(NAME_PLACEHOLDER, containerName)
                .Replace(PORT_PLACEHOLDER, port.ToString());
            if (containsPort)
            {
                port++;
            }
            AnsiConsole.MarkupLine($"\nRunning Docker command:\n[green]{mappedCommand}[/]\n\n");
            // Console.WriteLine($"\nRunning Docker command:\n{mappedCommand}\n\n");
            _processRunner.Run(mappedCommand);
        }

        return OperationResult.Continue;
    }

    private static string? ConfirmContainerName(string defaultContainerName)
    {
        // Console.WriteLine($"Using container name '{defaultContainerName}', enter to confirm, specify other name, or x to exit:", defaultContainerName);
        // var input = Console.ReadLine();
        var input = AnsiConsole.Ask<string>($"Using container name '{defaultContainerName}', enter to confirm, specify other name, or x to exit:", defaultContainerName);
        if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (string.IsNullOrEmpty(input))
        {
            return defaultContainerName;
        }

        return input;
    }

    private static int? ConfirmPort(int defaultPort)
    {
        while (true)
        {
            var input = AnsiConsole
                .Ask<string>($"Using port '{defaultPort}', enter to confirm, specify other name, or x to exit:", defaultPort.ToString());
            // Console.WriteLine($"Using port '{defaultPort}', [enter] to confirm, specify other name, or x to exit:");
            // var input = Console.ReadLine();
            if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (string.IsNullOrEmpty(input))
            {
                return defaultPort;
            }

            if (int.TryParse(input, out int selectedPort) && selectedPort > 0 && selectedPort < 65535)
            {
                return selectedPort;
            }
        }

        return null;
    }

    private record ReadDockerConfigResult(OperationResult OperationResult, string? Description);
    
    private static ReadDockerConfigResult ReadDockerConfig(
        IImmutableDictionary<int, (DockerStartContainerConfig Config, int Index)> numberedConfigs)
    {
        string[] choices = [..numberedConfigs.Values.Select(x => x.Config.Description), "Back", "Exit App"];

        var choice = AnsiConsole
            .Prompt(new SelectionPrompt<string>()
                .Title("Select a docker container to run")
                .AddChoices(choices));

        if (choice.Equals("Exit App"))
        {
            return new ReadDockerConfigResult(OperationResult.ExitApplication, null);
        }

        if (choice.Equals("Back"))
        {
            return new ReadDockerConfigResult(OperationResult.Abort, null);
        }
        
        return new ReadDockerConfigResult(OperationResult.Continue, choice);
    }

    private record DockerStartContainerConfig(
        string Description,
        string DefaultContainerName,
        int DefaultPort,
        string Image,
        IImmutableList<string> DockerRunCommands);
}