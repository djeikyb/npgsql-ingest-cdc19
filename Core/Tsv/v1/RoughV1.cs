using System.Diagnostics;

namespace Core.Tsv.v1;

public class RoughV1 : IBulkImporter
{
    /// <inheritdoc />
    /// <remarks>
    /// Respects batch config.
    /// Good candidate for an entity framework persister.
    /// A new entity is allocated for each line.
    /// String split is used to parse each line.
    /// A new raw row is allocated for each line.
    /// </remarks>
    public async Task Import(
        Stream tsv,
        Playbook playbook,
        PreKnowns preKnowns,
        CancellationToken ct
    )
    {
        var maxBatchSize = playbook.Config.BatchSizeMax;
        var maxInserts = playbook.Config.RecordsMax;

        // I expect max inserts to be rather high, like ten million. And the
        // batch size I expect to be orders of magnitude less. Perhaps a batch
        // is a hundred lines, or a hundred thousand lines.

        // It would be reasonable to throw an exception. The batch size ought
        // not be larger than the maximum records. But nah, just shrink the
        // batch size and run the loop once.
        if (maxBatchSize > maxInserts) maxBatchSize = maxInserts;

        // Another plausible (if questionable) behaviour could be to instead
        // always process a full batch. Eh.
        // if (maxInserts < maxBatchSize) maxInserts = maxBatchSize;

        using var reader = new StreamReader(tsv);

        // skip header
        reader.ReadLine();

        await playbook.Repo.BeginAsync(ct);

        // number of entities that have been OR WILL BE saved
        int total = 0;

        var sw = new Stopwatch();
        sw.Start();

        var entities = new List<CovidCase>(maxBatchSize);

        string? line;
        while (
            null != (line = reader.ReadLine())
            && (total < maxInserts)
        )
        {
            var rawRow = new RawRow();
            playbook.RowParser.Parse(line, rawRow);

            var entity = new CovidCase();
            RowEntityMapper.RowToEntity(rawRow, preKnowns, entity);
            entities.Add(entity);

            // Immediately increment cause we need the modulo ops to not fire
            // when total is zero. One of a few reasons to avoid a counting for
            // loop.
            total += 1;

            // Could look at the count property of the list, which is prolly an
            // O(1) operation? But this is definitely O(1).
            if (total % maxBatchSize == 0)
            {
                await playbook.Repo.PersistAsync(entities, ct);
                entities = new List<CovidCase>(maxBatchSize);
            }

            if (total % playbook.Config.PrintEverySoOften == 0)
            {
                Console.WriteLine($"{playbook.Config.PrintEverySoOften} records took {sw.Elapsed.ToString()}");
                sw.Reset();
                sw.Start();
            }
        }

        // The loop can exit without saving the last little bit. What if the
        // last quantity of lines is less than the batch size?
        if (entities.Count > 0)
        {
            await playbook.Repo.PersistAsync(entities, ct);
        }

        await playbook.Repo.EndAsync(ct);
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
