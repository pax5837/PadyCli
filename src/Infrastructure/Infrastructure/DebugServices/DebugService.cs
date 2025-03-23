namespace Infrastructure.DebugServices;

public class DebugService : IDebugService, IDebugServiceInitializer
{
    public bool FileSystemModificationsAllowed { get; set; }
}