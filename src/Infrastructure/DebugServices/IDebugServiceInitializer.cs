namespace Infrastructure.DebugServices;

public interface IDebugServiceInitializer
{
    bool FileSystemModificationsAllowed { set; }
}