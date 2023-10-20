using Core;
using Core.Tsv;

namespace Infra;

public class EfExtBulk : IPersistCases
{
    private readonly PgContext _context;

    public EfExtBulk(PgContext context)
    {
        _context = context;
    }

    public Task BeginAsync(CancellationToken _) => Task.CompletedTask;
    public Task EndAsync(CancellationToken _) => Task.CompletedTask;

    /// <inheritdoc />
    public void Begin()
    {
    }

    /// <inheritdoc />
    public void End()
    {
    }

    public Task PersistAsync(CovidCase entity, CancellationToken ct = default) => PersistAsync(new []{entity}, ct);

    public async Task PersistAsync(IEnumerable<CovidCase> cases, CancellationToken ct)
    {
        await _context.BulkInsertAsync(cases, ct).ConfigureAwait(false);
    }

    public Task PersistAsync(StreamReader reader, ImportConfig config, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Persist(CovidCase entity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Persist(IEnumerable<CovidCase> entities, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task Persist(StreamReader reader, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
