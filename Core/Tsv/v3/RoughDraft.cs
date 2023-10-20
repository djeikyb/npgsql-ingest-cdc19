using System.Diagnostics;

namespace Core.Tsv.v3;

public class RoughDraft : IBulkImporter
{
    /// <inheritdoc />
    /// <remarks>
    /// Do not use with entity framework based persisters.
    /// One raw row instance is reused to reduce allocations.
    /// One entity instance is reused to reduce allocations.
    /// Persist is called every line instead of in batches.
    /// Lines are parsed with substring instead of split.
    /// </remarks>
    public async Task Import(Stream tsv, Playbook playbook, PreKnowns preKnowns, CancellationToken ct)
    {
        var maxInserts = playbook.Config.RecordsMax;

        await playbook.Repo.BeginAsync(ct).ConfigureAwait(false);

        using var reader = new StreamReader(tsv);

        // skip header
        reader.ReadLine();

        var printEvery = playbook.Config.PrintEverySoOften;
        int total = 0;

        var sw = new Stopwatch();
        sw.Start();

        var rawRow = new RawRow();
        string? line;
        CovidCase entity = new CovidCase();
        while (
            null != (line = reader.ReadLine())
            && (total < maxInserts)
        )
        {
            playbook.RowParser.Parse(line, rawRow);
            RowEntityMapper.RowToEntity(rawRow, preKnowns, entity);
            await playbook.Repo.PersistAsync(entity, ct).ConfigureAwait(false);
            total += 1;

            if (total % printEvery == 0)
            {
                Console.WriteLine($"{printEvery} records took {sw.Elapsed.ToString()}");
                sw.Reset();
                sw.Start();
            }
        }

        await playbook.Repo.EndAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
