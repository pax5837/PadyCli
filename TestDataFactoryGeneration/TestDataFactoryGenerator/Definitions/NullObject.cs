namespace TestDataFactoryGenerator.Definitions;

public class NullObject
{
    public static readonly NullObject Instance = new();

    private NullObject()
    {
    }
}