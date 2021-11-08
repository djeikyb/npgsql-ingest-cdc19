using System.Diagnostics;

namespace Core.Tsv.v2;

public class MediumGrit : IBulkImporter
{
    /// <inheritdoc />
    /// <remarks>
    /// Persist is called every line instead of in batches.
    /// Not a great candidate for persisters that use entity framework.
    /// A new entity is allocated for each line.
    /// String split is used to parse each line.
    /// A new raw row is allocated for each line.
    /// </remarks>
    public async Task Import(Stream tsv, Playbook playbook, PreKnowns preKnowns, CancellationToken ct)
    {
        var maxInserts = playbook.Config.RecordsMax;

        using var reader = new StreamReader(tsv);

        // skip header
        reader.ReadLine();

        await playbook.Repo.Begin(ct);


        var printEvery = playbook.Config.PrintEverySoOften;

        int total = 0;

        var sw = new Stopwatch();
        sw.Start();

        var rawRow = new RawRow();
        string? line;
        while (
            null != (line = reader.ReadLine())
            && (total < maxInserts)
        )
        {
            playbook.RowParser.Parse(line, rawRow);
            var entity = RowEntityMapper.RawToEntity(rawRow, preKnowns);
            await playbook.Repo.Persist(entity, ct);

            total += 1;

            if (total % printEvery == 0)
            {
                Console.WriteLine($"{printEvery} records took {sw.Elapsed.ToString()}");
                sw.Reset();
                sw.Start();
            }
        }

        await playbook.Repo.End(ct);
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
