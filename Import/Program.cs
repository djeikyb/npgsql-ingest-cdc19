// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Core;
using Core.Tsv;
using Infra;

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
using var reader = new StreamReader(fs);

// skip header
reader.ReadLine();


var context = new PgContext();

var wasJustNowCreated = await context.Database.EnsureCreatedAsync(ct);
if (wasJustNowCreated)
{
    await new SeedData(context).Plant();
    await context.SaveChangesAsync(ct);
}

var ploader = new PreKnownsLoader(context);
var preKnowns = await ploader.Load(ct);

var repo = new MyRepository<CovidCase>(context);
var importer = new BulkImporter(repo, preKnowns, reader);

var t = new Stopwatch();
t.Start();
await importer.Load(maxBatchSize: 1000, maxInserts: 500_000, ct: ct);
t.Stop();
Console.WriteLine($"Overall time: {t.Elapsed.ToString()}");
