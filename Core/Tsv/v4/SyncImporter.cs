using System.Diagnostics;

namespace Core.Tsv.v4;

/// <summary>
/// Basically same as v3, but eschews TAP. Establishes a baseline for a future
/// version that uses spans and stackalloc.
/// </summary>
public class SyncImporter : IBulkImporter
{
    private const int BufferSize = 1024;

    // public IEnumerable<(int col, char[] text)> Foo(StreamReader reader)
    // {
    //     Span<char> buffer = stackalloc char[BufferSize];
    //
    //     var maxFieldSize = 1024;
    //     Span<char> field = stackalloc char[maxFieldSize];
    //     field.Clear();
    //
    //     var fieldLength = 0;
    //     var colIdx = 0;
    //
    //     const char delim = '\t';
    //     const char newline = '\n';
    //
    //     int readLen;
    //     while ((readLen = reader.ReadBlock(buffer)) > 0)
    //     {
    //         // TODO try passing in a buffer of type Memory?
    //         for (var i = 0; i < readLen; i++)
    //         {
    //             var c = buffer[i];
    //
    //             if (c == delim || c == newline)
    //             {
    //                 yield return (colIdx, field.ToArray());
    //
    //                 // reset field buffer
    //                 fieldLength = 0;
    //
    //                 // shift the column index
    //                 if (c == delim)
    //                 {
    //                     // increase if delimiter
    //                     colIdx += 1;
    //                 }
    //                 else
    //                 {
    //                     // reset to zero if line separator
    //                     colIdx = 0;
    //                 }
    //             }
    //             else
    //             {
    //                 field[fieldLength] = c;
    //                 fieldLength += 1;
    //             }
    //         }
    //     }
    //
    // }

    /// <inheritdoc />
    public Task Import(Stream tsv, Playbook playbook, PreKnowns preKnowns, CancellationToken ct)
    {
        var maxInserts = playbook.Config.RecordsMax;

        playbook.Repo.Begin();

        var reader = new StreamReader(tsv);

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
            if (ct.IsCancellationRequested) break;

            playbook.RowParser.Parse(line, rawRow);
            RowEntityMapper.RowToEntity(rawRow, preKnowns, entity);
            playbook.Repo.Persist(entity);
            total += 1;

            if (total % printEvery == 0)
            {
                Console.WriteLine($"{printEvery} records took {sw.Elapsed.ToString()}");
                sw.Reset();
                sw.Start();
            }
        }

        playbook.Repo.End();

        return Task.CompletedTask;

        // foreach (var (col, text) in Foo(reader))
        // {
        //
        // }
        //
        //
        // var bufLen = 1024;
        // var bufLarge = new char[bufLen];
        // new Span<char>();
        // Memory<char> mem = new Memory<char>(bufLarge);
        // playbook.
        //
        // var read = reader.ReadBlock(mem.Span);
        //
        // while (read == bufLen)
        // {
        //
        //
        //     read = reader.ReadBlock(mem.Span);
        // }
        //
        // mem.Slice(0, read);
        //
        //
        // var fieldNum = 0;
        //
        // for (int i = 0; i < read; i++)
        // {
        //     var c = bufLarge[i];
        //     switch (c)
        //     {
        //         case '\t':
        //         {
        //             break;
        //         }
        //         case '\n':
        //         {
        //             break;
        //         }
        //         default:
        //         {
        //             break;
        //         }
        //     }
        // }
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
