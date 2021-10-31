namespace Core;

public interface IPersistCases
{
    /// <summary>
    /// May return Task.CompletedTask
    /// </summary>
    Task Begin(CancellationToken ct);

    /// <summary>
    /// May return Task.CompletedTask
    /// </summary>
    Task End(CancellationToken ct);

    Task Persist(IEnumerable<CovidCase> entities, CancellationToken ct);
    Task Persist(StreamReader reader, ImportConfig config, CancellationToken ct);
}

public class ImportConfig
{
    public int RecordsMax { get; set; }
    public int BatchSizeMax { get; set; }
    public int PrintEverySoOften { get; set; }
}
