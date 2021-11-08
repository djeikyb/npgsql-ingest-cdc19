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

    public async Task Begin(CancellationToken ct)
    {
        _conn = new NpgsqlConnection(_connString);
        await _conn.OpenAsync(ct).ConfigureAwait(false);

        var cols = Columns();
        var cmd = $"COPY covid_case ({cols}) FROM STDIN (FORMAT BINARY)";
        _writer = await _conn.BeginBinaryImportAsync(cmd, ct).ConfigureAwait(false);
    }

    public async Task End(CancellationToken ct)
    {
        if (_writer == null) throw new Exception($"Call {nameof(Begin)} first.");
        await _writer.CompleteAsync(ct).ConfigureAwait(false);
    }

    public async Task Persist(CovidCase entity, CancellationToken ct)
    {
        // if (_writer == null) throw new Exception();
        await WriteRow(_writer!, entity, ct).ConfigureAwait(false);
    }

    public async Task Persist(IEnumerable<CovidCase> entities, CancellationToken ct = default)
    {
        foreach (var ent in entities)
        {
            await WriteRow(_writer!, ent, ct).ConfigureAwait(false);
        }
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
