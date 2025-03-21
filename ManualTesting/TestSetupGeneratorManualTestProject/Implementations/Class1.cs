namespace TestSetupGeneratorManualTestProject.Implementations;

public class Class1
{
    public Class1(
        IInterface1 interface1,
        IInterface2 interface2)
    {
    }

    public async Task<string> DoSomethinAsync(string input)
    {
        await Task.Delay(1);

        return "hellpo";
        ;
    }

    public string DoSomethin(string input)
    {
        return "hellpo";
    }
}