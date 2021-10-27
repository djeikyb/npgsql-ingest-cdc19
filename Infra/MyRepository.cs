using Ardalis.Specification.EntityFrameworkCore;
using Core;

namespace Infra
{
    public class MyRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
    {
        private readonly PgContext _context;

        public MyRepository(PgContext context) : base(context)
        {
            _context = context;
        }

        // Not required to implement anything. Add additional functionalities if required.
        public async Task BulkInsert(IEnumerable<T> entities, CancellationToken ct)
        {
            await _context.BulkInsertAsync(entities, ct);
        }
    }
}
