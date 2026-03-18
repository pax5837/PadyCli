using System.Collections.Immutable;

namespace PadyCliInteractiveConsoleApp.Features.Greetings;

internal class GreetingsService
{
    private const string UserNamePlaceholder = "{{{{{{{{UserName}}}}}}}}";
    
    private static readonly ImmutableArray<string> standardGreetings =
    [
        "Hello welcome to the world of PadyCli!",
        $"Hi [red]{UserNamePlaceholder}[/], we know who you are",
        $"Hi [blue]{UserNamePlaceholder}[/], does your mom know you are here?",
    ];
    
    private static readonly ImmutableArray<string> standardGoodbyes =
    [
        "Goodbye",
        "Did you wash your hands?",
        "Click like and subscribe",
    ];
    
    private static readonly string userName = Environment.UserName;
    
    private static readonly Random random = new();
    
    public string GetHelloMessage()
    {
        var now = DateTime.Now;
        if (now is { Month: 3, Day: 17 })
        {
            return "Happy st Patrick's day!";
        }
        
        return standardGreetings[random.Next(standardGreetings.Length)].Replace(UserNamePlaceholder, userName);
    }

    public string GetGoodbyeMessage()
    {
        var now = DateTime.Now;

        if (now is { Month: 5, Day: 4 })
        {
            return "May the fourth be with you";
        }
        
        return standardGoodbyes[random.Next(standardGreetings.Length)].Replace(UserNamePlaceholder, userName);
    }
}