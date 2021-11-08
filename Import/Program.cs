// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Core;
using Core.Tsv;
using Core.Tsv.RowParser;
using Core.Tsv.v3;
using Infra;
using Microsoft.EntityFrameworkCore;

namespace Import;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Canceling...");
            cts.Cancel();
            e.Cancel = true;
        };

        var ct = cts.Token;

        var fn = args[0];
        Console.WriteLine(fn);

        var fs = File.OpenRead(fn);
        // using var reader = new StreamReader(fs);
        //
        // // skip header
        // reader.ReadLine();


        await using var context = new PgContext();

        var wasJustNowCreated = await context.Database.EnsureCreatedAsync(ct);
        if (wasJustNowCreated)
        {
            await new SeedData(context).Plant();
            await context.SaveChangesAsync(ct);
        }

        var ploader = new PreKnownsLoader(context);
        var preKnowns = await ploader.Load(ct);


        // var pgCopyV1 = new PgCopy(context);
        // IPersistCases repo = pgCopyV1;
        // using var pgCopyV2 = new PgCopyV2(context);
        // repo = pgCopyV2;
        // // var importer = new BulkImporter(repo, preKnowns, reader);
        // var importer = new Core.Tsv.v3.RoughDraft(preKnowns, repo);
        //
        // var t = new Stopwatch();
        // t.Start();
        // await importer.Import(fs, repo, preKnowns, config, ct: ct);
        // t.Stop();
        // Console.WriteLine($"Overall time: {t.Elapsed.ToString()}");

        var config = new ImportConfig
        {
            RecordsMax = 5_000_000,
            BatchSizeMax = 1_000_000,
            PrintEverySoOften = 100_000,
        };

        var pb1 = new Playbook(
            new SplitStringRowParser(),
            new EfExtBulk(context),
            new Core.Tsv.v1.RoughV1(),
            config
        );

        var pb2 = new Playbook(
            new SplitStringRowParser(),
            new PgCopy(context),
            new Core.Tsv.v2.MediumGrit(),
            config
        );

        var pb3 = new Playbook(
            new SubstrRowParser(),
            new PgCopy(context),
            new Core.Tsv.v3.RoughDraft(),
            config
        );

        var pb4 = new Playbook(
            new SubstrRowParser(),
            new PgCopy(context),
            new Core.Tsv.v4.Next(),
            new ImportConfig
            {
                RecordsMax = 5_000_000,
                BatchSizeMax = 1_000_000,
                PrintEverySoOften = 100_000,
            }
        );

        // var playbook = pb1;
        // var playbook = pb2;
        // var playbook = pb3;
        var playbook = pb4;

        await playbook.Run(fs, preKnowns, ct);
    }
}
