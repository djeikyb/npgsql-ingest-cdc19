using System.Diagnostics;
using Core;
using Core.Tsv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Infra;

public class PgCopyV2 : IPersistCases, IDisposable
{
    private readonly string _connString;
    private NpgsqlBinaryImporter? _writer;

    public PgCopyV2(PgContext context)
    {
        _connString = context.Database.GetConnectionString() ?? throw new Exception("Null connection string");
    }

    public async Task Begin(CancellationToken ct)
    {
        await using var conn = new NpgsqlConnection(_connString);
        await conn.OpenAsync(ct);

        var cols = PgCopy.Columns();
        var cmd = $"COPY covid_case ({cols}) FROM STDIN (FORMAT BINARY)";
        _writer = await conn.BeginBinaryImportAsync(cmd, ct);
    }

    public async Task End(CancellationToken ct)
    {
        if (_writer == null) throw new Exception($"Call {nameof(Begin)} first.");
        await _writer.CompleteAsync(ct);
    }

    public async Task Persist(IEnumerable<CovidCase> entities, CancellationToken ct)
    {
        foreach (var ent in entities)
        {
            await PgCopy.WriteRow(_writer!, ent, ct);
        }
    }

    public async Task Persist(StreamReader reader, ImportConfig config, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _writer?.Dispose();
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
        await writer.StartRowAsync(ct);

        await writer.WriteAsync(entity.CaseMonth, NpgsqlDbType.Text, ct);
        await writer.WriteAsync(entity.ResState, NpgsqlDbType.Text, ct);
        await writer.WriteAsync(entity.StateFipsCode, NpgsqlDbType.Text, ct);
        await writer.WriteAsync(entity.ResCounty, NpgsqlDbType.Text, ct);
        await writer.WriteAsync(entity.CountyFipsCode, NpgsqlDbType.Text, ct);

        await WriteNullable(writer, entity.AgeGroupId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.SexId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.RaceId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.EthnicityId, NpgsqlDbType.Integer, ct);

        await writer.WriteAsync(entity.CasePositiveSpecimenInterval, NpgsqlDbType.Text, ct);
        await writer.WriteAsync(entity.CaseOnsetInterval, NpgsqlDbType.Text, ct);

        await WriteNullable(writer, entity.ProcessId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.ExposureYnId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.CurrentStatusId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.SymptomStatusId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.HospYnId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.IcuYnId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.DeathYnId, NpgsqlDbType.Integer, ct);
        await WriteNullable(writer, entity.UnderlyingConditionsYnId, NpgsqlDbType.Integer, ct);
    }

    static async Task WriteNullable(NpgsqlBinaryImporter writer, object? val, NpgsqlDbType type, CancellationToken ct)
    {
        if (val == null)
        {
            await writer.WriteNullAsync(ct);
        }
        else
        {
            await writer.WriteAsync(val, type, ct);
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

    private RawRow ReadRawLine(StreamReader csv)
    {
        string line = csv.ReadLine()
                      ?? throw new Exception("Unexpected end of file.");
        var rawRow = new RawRow();
        rawRow.Load(line);
        return rawRow;
    }
}
