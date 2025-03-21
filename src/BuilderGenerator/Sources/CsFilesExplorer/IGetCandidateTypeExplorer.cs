using System.Collections.Immutable;

namespace BuilderGenerator.Sources.CsFilesExplorer;

public interface IGetCandidateTypeExplorer
{
    IImmutableList<CandidateType> GetCandidateTypes(IImmutableList<string> csFiles);

    public record CandidateType(string Namespace, string typeName);
}