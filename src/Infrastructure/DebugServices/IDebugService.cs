namespace Infrastructure.DebugServices;

public interface IDebugService
{
    bool FileSystemModificationsAllowed { get; }
}