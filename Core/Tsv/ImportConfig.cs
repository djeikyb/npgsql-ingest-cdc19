namespace Core.Tsv;

public class ImportConfig
{
    public int RecordsMax { get; init; }
    public int BatchSizeMax { get; init; }
    public int PrintEverySoOften { get; set; } = 100_000;
}
