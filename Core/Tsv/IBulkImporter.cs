namespace Core.Tsv;

public interface IBulkImporter : IDisposable
{
    Task Import(Stream tsv, Playbook playbook, PreKnowns preKnowns, CancellationToken ct);
}
