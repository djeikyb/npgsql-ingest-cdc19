namespace Core.Tsv;

public interface IPersistCases
{
    /// <summary>
    /// May return Task.CompletedTask
    /// </summary>
    Task BeginAsync(CancellationToken ct);

    /// <summary>
    /// May return Task.CompletedTask
    /// </summary>
    Task EndAsync(CancellationToken ct);

    void Begin();
    void End();

    Task PersistAsync(CovidCase entity, CancellationToken ct);
    Task PersistAsync(IEnumerable<CovidCase> entities, CancellationToken ct);
    Task PersistAsync(StreamReader reader, ImportConfig config, CancellationToken ct);

    void Persist(CovidCase entity);
    void Persist(IEnumerable<CovidCase> entities, CancellationToken ct);
}
