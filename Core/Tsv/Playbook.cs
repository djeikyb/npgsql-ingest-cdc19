using System.Diagnostics;
using Core.Tsv.RowParser;

namespace Core.Tsv;

public class Playbook
{
    public readonly IRowParser RowParser;
    public readonly IPersistCases Repo;
    private readonly IBulkImporter _importer;
    public readonly ImportConfig Config;

    public Playbook(
        IRowParser rowParser,
        IPersistCases repo,
        IBulkImporter importer,
        ImportConfig config
    )
    {
        RowParser = rowParser;
        Repo = repo;
        _importer = importer;
        Config = config;
    }

    public async Task Run(Stream stream, PreKnowns preKnowns, CancellationToken ct)
    {
        Console.WriteLine(RowParser.GetType().FullName);
        Console.WriteLine(Repo.GetType().FullName);
        Console.WriteLine(_importer.GetType().FullName);
        Console.WriteLine();

        var t = new Stopwatch();
        t.Start();
        await _importer.Import(stream, this, preKnowns, ct);
        t.Stop();
        Console.WriteLine($"Overall time: {t.Elapsed.ToString()}");

        Console.WriteLine();
        Console.WriteLine(RowParser.GetType().FullName);
        Console.WriteLine(Repo.GetType().FullName);
        Console.WriteLine(_importer.GetType().FullName);
    }
}
