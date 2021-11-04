using System.Diagnostics;
using Core.Tsv;

namespace Core.v3;

public class RoughDraft
{
    private readonly PreKnowns _preKnowns;
    private readonly IPersistCases _perst;

    public RoughDraft(PreKnowns preKnowns, IPersistCases perst)
    {
        _preKnowns = preKnowns;
        _perst = perst;
    }

    public async Task Load(StreamReader csv, int maxBatchSize, int maxInserts, CancellationToken ct = default)
    {
        if (maxBatchSize > maxInserts) maxBatchSize = maxInserts;
        if (maxInserts < maxBatchSize) maxInserts = maxBatchSize;

        await _perst.Begin(ct);


        const int printEvery = 100_000;
        int total = 0;

        var sw = new Stopwatch();
        sw.Start();

        var rawRow = new RawRow();
        string? line;
        CovidCase entity = new CovidCase();
        while (
            null != (line = csv.ReadLine())
            && (total < maxInserts)
        )
        {
            rawRow.LoadV2(line);
            BulkImporter.RowToEntity(rawRow, _preKnowns, entity);
            await _perst.Persist(entity, ct).ConfigureAwait(false);
            total += 1;

            if (total % printEvery == 0)
            {
                Console.WriteLine($"{printEvery} records took {sw.Elapsed.ToString()}");
                sw.Reset();
                sw.Start();
            }
        }

        await _perst.End(ct);
    }
}
