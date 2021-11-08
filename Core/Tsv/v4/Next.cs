using System.Diagnostics;
using System.Xml.Serialization;
using Core.Tsv.RowParser;

namespace Core.Tsv.v4;

public class Next : IBulkImporter
{
    /// <inheritdoc />
    public async Task Import(Stream tsv, Playbook playbook, PreKnowns preKnowns, CancellationToken ct)
    {
        var maxInserts = playbook.Config.RecordsMax;

        var repo = playbook.Repo;
        // var rowParser = (SubstrRowParser)playbook.RowParser;
        var rowParser = playbook.RowParser;

        await repo.Begin(ct).ConfigureAwait(false);

        var reader = new StreamReader(tsv);
        // tsv.Read();

        var buf = new char[1024];
        Memory<char> mem = new Memory<char>(buf);
        var read = reader.ReadBlock(mem.Span);

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
            rowParser.Parse(line, rawRow);
            RowEntityMapper.RowToEntity(rawRow, preKnowns, entity);
            await repo.Persist(entity, ct).ConfigureAwait(false);
            total += 1;

            if (total % printEvery == 0)
            {
                Console.WriteLine($"{printEvery} records took {sw.Elapsed.ToString()}");
                sw.Reset();
                sw.Start();
            }
        }

        await repo.End(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
