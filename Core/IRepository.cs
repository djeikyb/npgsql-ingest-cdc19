using Ardalis.Specification;

namespace Core
{
    public interface IRepository<T> : IRepositoryBase<T> where T : class
    {
        Task BulkInsert(IEnumerable<T> entities, CancellationToken ct);
    }

    public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class
    {
    }
}
