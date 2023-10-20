using Core;
using Core.Tsv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Infra;

public class PgCopy : IPersistCases, IDisposable
{
    private readonly string _connString;
    private NpgsqlBinaryImporter? _writer;
    private NpgsqlConnection? _conn;

    public PgCopy(PgContext context)
    {
        _connString = context.Database.GetConnectionString() ?? throw new Exception("Null connection string");
    }

    public async Task BeginAsync(CancellationToken ct)
    {
        _conn = new NpgsqlConnection(_connString);
        await _conn.OpenAsync(ct).ConfigureAwait(false);

        var cols = Columns();
        var cmd = $"COPY covid_case ({cols}) FROM STDIN (FORMAT BINARY)";
        _writer = await _conn.BeginBinaryImportAsync(cmd, ct).ConfigureAwait(false);
    }

    public async Task EndAsync(CancellationToken ct)
    {
        if (_writer == null) throw new Exception($"Call {nameof(BeginAsync)} first.");
        await _writer.CompleteAsync(ct).ConfigureAwait(false);
    }

    public void Begin()
    {
        _conn = new NpgsqlConnection(_connString);
        _conn.Open();

        var cols = Columns();
        var cmd = $"COPY covid_case ({cols}) FROM STDIN (FORMAT BINARY)";
        _writer = _conn.BeginBinaryImport(cmd);
    }

    public void End()
    {
        if (_writer == null) throw new Exception($"Call {nameof(BeginAsync)} first.");
        _writer.Complete();
    }

    public async Task PersistAsync(CovidCase entity, CancellationToken ct)
    {
        // if (_writer == null) throw new Exception();
        await WriteRowAsync(_writer!, entity, ct).ConfigureAwait(false);
    }

    public async Task PersistAsync(IEnumerable<CovidCase> entities, CancellationToken ct = default)
    {
        foreach (var ent in entities)
        {
            await WriteRowAsync(_writer!, ent, ct).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void Persist(IEnumerable<CovidCase> entities, CancellationToken ct)
    {
        foreach (var ent in entities)
        {
            WriteRow(_writer!, ent);
            if (ct.IsCancellationRequested) break;
        }
    }

    /// <inheritdoc />
    public void Persist(CovidCase entity)
    {
        WriteRow(_writer!, entity);
    }

    public static async Task WriteRowAsync(NpgsqlBinaryImporter writer, CovidCase entity, CancellationToken ct)
    {
        await writer.StartRowAsync(ct).ConfigureAwait(false);

        await writer.WriteAsync(entity.CaseMonth, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.ResState, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.StateFipsCode, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.ResCounty, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.CountyFipsCode, NpgsqlDbType.Text, ct).ConfigureAwait(false);

        await WriteNullableIntAsync(writer, entity.AgeGroupId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.SexId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.RaceId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.EthnicityId, ct).ConfigureAwait(false);

        await writer.WriteAsync(entity.CasePositiveSpecimenInterval, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        await writer.WriteAsync(entity.CaseOnsetInterval, NpgsqlDbType.Text, ct).ConfigureAwait(false);

        await WriteNullableIntAsync(writer, entity.ProcessId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.ExposureYnId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.CurrentStatusId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.SymptomStatusId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.HospYnId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.IcuYnId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.DeathYnId, ct).ConfigureAwait(false);
        await WriteNullableIntAsync(writer, entity.UnderlyingConditionsYnId, ct).ConfigureAwait(false);
    }

    public static void WriteRow(NpgsqlBinaryImporter writer, CovidCase entity)
    {
        writer.StartRow();

        writer.Write(entity.CaseMonth, NpgsqlDbType.Text);
        writer.Write(entity.ResState, NpgsqlDbType.Text);
        writer.Write(entity.StateFipsCode, NpgsqlDbType.Text);
        writer.Write(entity.ResCounty, NpgsqlDbType.Text);
        writer.Write(entity.CountyFipsCode, NpgsqlDbType.Text);

        WriteNullableInt(writer, entity.AgeGroupId);
        WriteNullableInt(writer, entity.SexId);
        WriteNullableInt(writer, entity.RaceId);
        WriteNullableInt(writer, entity.EthnicityId);

        writer.Write(entity.CasePositiveSpecimenInterval, NpgsqlDbType.Text);
        writer.Write(entity.CaseOnsetInterval, NpgsqlDbType.Text);

        WriteNullableInt(writer, entity.ProcessId);
        WriteNullableInt(writer, entity.ExposureYnId);
        WriteNullableInt(writer, entity.CurrentStatusId);
        WriteNullableInt(writer, entity.SymptomStatusId);
        WriteNullableInt(writer, entity.HospYnId);
        WriteNullableInt(writer, entity.IcuYnId);
        WriteNullableInt(writer, entity.DeathYnId);
        WriteNullableInt(writer, entity.UnderlyingConditionsYnId);
    }


    static Task WriteNullableIntAsync(NpgsqlBinaryImporter writer, int? val, CancellationToken ct)
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

    static void WriteNullableInt(NpgsqlBinaryImporter writer, int? val)
    {
        if (!val.HasValue)
        {
            writer.WriteNull();
        }
        else
        {
            writer.Write(val.Value, NpgsqlDbType.Integer);
        }
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

    public Task PersistAsync(StreamReader reader, ImportConfig config, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _conn?.Dispose();
    }
}
