using Ardalis.Specification;

namespace Archie.Api.Common;

public interface IRepository
{
    void Add<T>(T entity) where T : class;
    Task<T?> FirstOrDefaultAsync<T>(ISpecification<T> spec, CancellationToken ct) where T : class;
    Task<TResult?> FirstOrDefaultAsync<T, TResult>(ISpecification<T, TResult> spec, CancellationToken ct) where T : class;
    Task<List<T>> ListAsync<T>(ISpecification<T> spec, CancellationToken ct) where T : class;
    Task<List<TResult>> ListAsync<T, TResult>(ISpecification<T, TResult> spec, CancellationToken ct) where T : class;
    Task<int> SaveChangesAsync(CancellationToken ct);
}
