using System.Threading;
using System.Threading.Tasks;

namespace BuilderGenerator;

public interface IDataBuilderGenerator
{
    Task GenerateBuildersAsync(string sourceDirectory, string outputDirectory, CancellationToken cancellationToken);
}