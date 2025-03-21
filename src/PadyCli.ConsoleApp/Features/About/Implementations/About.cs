using System.Reflection;

namespace PadyCli.ConsoleApp.Features.About.Implementations;

public class About
{
    private const string ThirdPartyNoticesFileName = "THIRD-PARTY-NOTICES.TXT";

    public async Task<int> RunAsync(AboutOptions options)
    {
        var appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
        var appName = Assembly.GetExecutingAssembly().GetName().Name;

        var thirdPartyNotices = GetPathToThirdPartyNotices(appFolder) ?? GetAlternatePathPathToThirdPartyNotices(appFolder);
        var thirdPartyNoticeLines = await File.ReadAllLinesAsync(thirdPartyNotices);

        Console.WriteLine($"{appName} v{appVersion}");
        Console.WriteLine();
        Console.WriteLine("This application uses the following third-party libraries:");
        Console.WriteLine();
        foreach (var line in thirdPartyNoticeLines)
        {
            Console.WriteLine(line);
        }

        return 0;
    }

    private static string? GetPathToThirdPartyNotices(string appFolder)
    {
        var pathToThirdPartyNotices = Path.Combine(appFolder, ThirdPartyNoticesFileName);

        return File.Exists(pathToThirdPartyNotices) ? pathToThirdPartyNotices : null;
    }

    private static string? GetAlternatePathPathToThirdPartyNotices(string appFolder)
    {
        var dirs = appFolder.Split(Path.DirectorySeparatorChar);
        var commonDir = string.Join(Path.DirectorySeparatorChar, dirs.Take(dirs.Length - 3).Append("content"));

        return Path.Combine(commonDir, ThirdPartyNoticesFileName);
    }
}