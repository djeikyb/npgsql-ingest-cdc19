using Core;

namespace Infra;

public class EfExtBulk : IPersistCases
{
    private readonly PgContext _context;

    public EfExtBulk(PgContext context)
    {
        _context = context;
    }

    public Task Begin(CancellationToken _) => Task.CompletedTask;
    public Task End(CancellationToken _) => Task.CompletedTask;
    public Task Persist(CovidCase entity, CancellationToken ct = default) => Persist(new []{entity}, ct);

    public async Task Persist(IEnumerable<CovidCase> cases, CancellationToken ct)
    {
        await _context.BulkInsertAsync(cases, ct);
    }

    public Task Persist(StreamReader reader, ImportConfig config, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task Persist(StreamReader reader, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
