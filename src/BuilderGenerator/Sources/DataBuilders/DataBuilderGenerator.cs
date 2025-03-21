using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuilderGenerator.Sources.CsFilesExplorer;
using DotnetInfrastructure.Contracts;

namespace BuilderGenerator.Sources.DataBuilders;

public class DataBuilderGenerator : IDataBuilderGenerator
{
    private readonly IGetCandidateTypeExplorer _getCandidateTypeExplorer;
    private readonly IAssemblyLoader _assemblyLoader;

    public DataBuilderGenerator(
        IGetCandidateTypeExplorer getCandidateTypeExplorer,
        IAssemblyLoader assemblyLoader)
    {
        _getCandidateTypeExplorer = getCandidateTypeExplorer;
        _assemblyLoader = assemblyLoader;
    }

    public async Task GenerateBuildersAsync(
        string sourceDirectory,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        var csFiles = Directory.GetFiles(sourceDirectory, "*.cs", SearchOption.TopDirectoryOnly).ToImmutableList();
        var typeCandidatesInFolder = _getCandidateTypeExplorer.GetCandidateTypes(csFiles);
        var assemblies = await _assemblyLoader.GetAssemblyAsync(sourceDirectory, cancellationToken);
        var typesToProcess = assemblies
            .GetTypes()
            .Where(t => typeCandidatesInFolder.Any(tc => t.Namespace == tc.Namespace && t.Name == tc.typeName))
            .ToImmutableList();

        foreach (var typeToProcess in typesToProcess)
        {
            Console.WriteLine(typeToProcess.FullName);
        }
    }
}