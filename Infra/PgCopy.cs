using System.Diagnostics;
using Core;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Infra;

public class PgCopyV2 : IPersistCases, IDisposable
{
    private readonly string _connString;
    private NpgsqlBinaryImporter? _writer;
    private NpgsqlConnection? _conn;

    public PgCopyV2(PgContext context)
    {
        _connString = context.Database.GetConnectionString() ?? throw new Exception("Null connection string");
    }

    public async Task Begin(CancellationToken ct)
    {
        _conn = new NpgsqlConnection(_connString);
        await _conn.OpenAsync(ct);

        var cols = PgCopy.Columns();
        var cmd = $"COPY covid_case ({cols}) FROM STDIN (FORMAT BINARY)";
        _writer = await _conn.BeginBinaryImportAsync(cmd, ct);
    }

    public async Task End(CancellationToken ct)
    {
        if (_writer == null) throw new Exception($"Call {nameof(Begin)} first.");
        await _writer.CompleteAsync(ct);
    }

    public async Task Persist(CovidCase entity, CancellationToken ct)
    {
        // if (_writer == null) throw new Exception();
        await PgCopy.WriteRow(_writer!, entity, ct).ConfigureAwait(false);
    }

    public async Task Persist(IEnumerable<CovidCase> entities, CancellationToken ct = default)
    {
        foreach (var ent in entities)
        {
            await PgCopy.WriteRow(_writer!, ent, ct);
        }
    }

    public Task Persist(StreamReader reader, ImportConfig config, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _conn?.Dispose();
    }
}

public class PgCopy : IPersistCases
{
    private readonly PgContext _context;

    public PgCopy(PgContext context)
    {
        _context = context;
    }

    public Task Begin(CancellationToken _) => Task.CompletedTask;
    public Task End(CancellationToken _) => Task.CompletedTask;
    public Task Persist(CovidCase entity, CancellationToken ct = default) => Persist(new []{entity}, ct);

    public async Task Persist(IEnumerable<CovidCase> cases, CancellationToken ct)
    {
        var connString = _context.Database.GetConnectionString();
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync(ct);

        var cols = Columns();

        await using var writer =
            await conn.BeginBinaryImportAsync($"COPY covid_case ({cols}) FROM STDIN (FORMAT BINARY)", ct);
        foreach (var ent in cases)
        {
            await WriteRow(writer, ent, ct);
        }

        await writer.CompleteAsync(ct);
    }


    public static string Columns()
    {
        var cols = string.Empty;

        cols += "case_month";
        cols += ",res_state";
        cols += ",state_fips_code";
        cols += ",res_county";
        cols += ",county_fips_code";

        cols += ",age_group_id";
        cols += ",sex_id";
        cols += ",race_id";
        cols += ",ethnicity_id";

        cols += ",case_positive_specimen_interval";
        cols += ",case_onset_interval";

        cols += ",process_id";
        cols += ",exposure_yn_id";
        cols += ",current_status_id";
        cols += ",symptom_status_id";
        cols += ",hosp_yn_id";
        cols += ",icu_yn_id";
        cols += ",death_yn_id";
        cols += ",underlying_conditions_yn_id";
        return cols;
    }

    public static async Task WriteRow(NpgsqlBinaryImporter writer, CovidCase entity, CancellationToken ct)
    {
        await writer.StartRowAsync(ct).ConfigureAwait(false);

        await writer.WriteAsync(entity.CaseMonth, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.ResState, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.StateFipsCode, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.ResCounty, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.CountyFipsCode, NpgsqlDbType.Text, ct).ConfigureAwait(false);

        await WriteNullableInt(writer, entity.AgeGroupId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.SexId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.RaceId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.EthnicityId, ct).ConfigureAwait(false);

        await writer.WriteAsync(entity.CasePositiveSpecimenInterval, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.CaseOnsetInterval, NpgsqlDbType.Text, ct).ConfigureAwait(false);

        await WriteNullableInt(writer, entity.ProcessId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.ExposureYnId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.CurrentStatusId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.SymptomStatusId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.HospYnId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.IcuYnId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.DeathYnId, ct).ConfigureAwait(false);
        await WriteNullableInt(writer, entity.UnderlyingConditionsYnId, ct).ConfigureAwait(false);
    }

    static Task WriteNullableInt(NpgsqlBinaryImporter writer, int? val, CancellationToken ct)
    {
        if (!val.HasValue)
        {
            return writer.WriteNullAsync(ct);
        }
        else
        {
            return writer.WriteAsync(val.Value, NpgsqlDbType.Integer, ct);
        }
    }

    public async Task Persist(StreamReader reader, ImportConfig config, CancellationToken ct)
    {
        var connString = _context.Database.GetConnectionString();
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync(ct);

        var cols = Columns();

        await using var writer =
            await conn.BeginBinaryImportAsync($"COPY covid_case ({cols}) FROM STDIN (FORMAT BINARY)", ct);

        var periodic = new Stopwatch();
        periodic.Start();

        var linesRead = 0;
        for (var ln = reader.ReadLine(); ln != null || linesRead >= config.RecordsMax; ln = reader.ReadLine())
        {
        }
    }

    // private RawRow ReadRawLine(StreamReader csv)
    // {
    //     string line = csv.ReadLine()
    //                   ?? throw new Exception("Unexpected end of file.");
    //     var rawRow = new RawRow();
    //     rawRow.Load(line);
    //     return rawRow;
    // }
}
