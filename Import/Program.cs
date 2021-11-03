// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Core;
using Core.Tsv;
using Core.v3;
using Infra;
using Microsoft.EntityFrameworkCore;

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


await using var context = new PgContext();

var wasJustNowCreated = await context.Database.EnsureCreatedAsync(ct);
if (wasJustNowCreated)
{
    await new SeedData(context).Plant();
    await context.SaveChangesAsync(ct);
}

var ploader = new PreKnownsLoader(context);
var preKnowns = await ploader.Load(ct);

var pgCopyV1 = new PgCopy(context);
IPersistCases repo = pgCopyV1;
using var pgCopyV2 = new PgCopyV2(context);
repo = pgCopyV2;
// var importer = new BulkImporter(repo, preKnowns, reader);
var importer = new Core.v3.RoughDraft(preKnowns, repo);

var t = new Stopwatch();
t.Start();
await importer.Load(reader, maxBatchSize: 1_000_000, maxInserts: 5_00_000, ct: ct);
t.Stop();
Console.WriteLine($"Overall time: {t.Elapsed.ToString()}");
