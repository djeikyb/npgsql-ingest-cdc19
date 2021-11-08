using System.Diagnostics;
using Core.Tsv.RowParser;

namespace Core.Tsv.v1;

public class BulkImporter
{
    private readonly IPersistCases _repo;
    private readonly PreKnowns _preKnowns;

    private readonly StreamReader _csv;
    // private readonly RawRow _rawRow = new();
    private readonly IRowParser _rowParser = new SplitStringRowParser();

    public BulkImporter(
        IPersistCases repo,
        PreKnowns preKnowns,
        StreamReader csv
    )
    {
        _repo = repo;
        _preKnowns = preKnowns;
        _csv = csv;
    }

    private RawRow ReadRawLine()
    {
        string line = _csv.ReadLine()
                      ?? throw new Exception("Unexpected end of file.");
        var rawRow = new RawRow();
        _rowParser.Parse(line, rawRow);
        return rawRow;
    }

    public async Task Load(int maxBatchSize, int maxInserts, CancellationToken ct = default)
    {
        if (maxBatchSize > maxInserts) maxBatchSize = maxInserts;
        if (maxInserts < maxBatchSize) maxInserts = maxBatchSize;

        var hundrThou = new Stopwatch();
        hundrThou.Start();

        await _repo.Begin(ct);

        int total = 0;
        int batchNumber = 1;
        var t = new Stopwatch();
        while (total < maxInserts)
        {
            t.Reset();
            t.Start();
            var result = await Batch(maxBatchSize, ct);
            t.Stop();

            total += result.Inserted;

            // Console.WriteLine($"Batch #{batchNumber,5}. Total inserts: {total,9}. Batch size: {maxBatchSize}. Seconds: {t.Elapsed.TotalSeconds}");

            var printEvery = 100_000;
            if (total % printEvery == 0)
            {
                Console.WriteLine($"{printEvery} records took {hundrThou.Elapsed.ToString()}");
                hundrThou.Reset();
                hundrThou.Start();
            }

            if (result.NoMoreData) break;

            batchNumber += 1;
        }

        await _repo.End(ct);
    }

    private async Task<(int Inserted, bool NoMoreData)> Batch(int batchSize, CancellationToken ct)
    {
        int inserts;
        bool eof = false;

        var entities = new List<CovidCase>();
        for (inserts = 0; inserts < batchSize; inserts++)
        {
            var line = _csv.ReadLine();
            if (line == null)
            {
                // end of stream, no more data
                eof = true;
                break;
            }

            var rawRow = new RawRow();
            _rowParser.Parse(line, rawRow);
            var entity = RowEntityMapper.RawToEntity(rawRow, _preKnowns);
            entities.Add(entity);
        }

        await _repo.Persist(entities, ct);

        return (Inserted: inserts, NoMoreData: eof);
    }

    public async Task Load(int quantity, CancellationToken ct = default)
    {
        var entities = new List<CovidCase>();
        for (int i = 0; i < quantity; i++)
        {
            var rawRow = ReadRawLine();
            var entity = RowEntityMapper.RawToEntity(rawRow, _preKnowns);
            entities.Add(entity);
        }

        await _repo.Persist(entities, ct);
    }
}
