using Ardalis.Specification.EntityFrameworkCore;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Archie.Api.Common;

namespace Archie.Api.Database;

public class ArchieRepository : IRepository
{
    private ArchieContext Db { get; }
    private ISpecificationEvaluator Evaluator { get; }

    public ArchieRepository(ArchieContext db)
    {
        Db = db;
        Evaluator = SpecificationEvaluator.Default;
    }

    public void Add<T>(T entity) where T : class
    {
        Db.Set<T>().Add(entity);
    }

    public Task<T?> FirstOrDefaultAsync<T>(ISpecification<T> spec, CancellationToken ct) where T : class
    {
        return ApplySpecification(spec).FirstOrDefaultAsync(ct);
    }

    public Task<TResult?> FirstOrDefaultAsync<T, TResult>(ISpecification<T, TResult> spec, CancellationToken ct) where T : class
    {
        return ApplySpecification(spec).FirstOrDefaultAsync(ct);
    }

    public ValueTask<T?> FindByIdAsync<T, TId>(TId id, CancellationToken ct) where T : class where TId : notnull
    { 
        return Db.Set<T>().FindAsync(new object[] { id }, ct);
    }

    public Task<List<T>> ListAsync<T>(ISpecification<T> spec, CancellationToken ct) where T : class
    {
        return ApplySpecification(spec).ToListAsync(ct);
    }

    public Task<List<TResult>> ListAsync<T, TResult>(ISpecification<T, TResult> spec, CancellationToken ct) where T : class
    {
        return ApplySpecification(spec).ToListAsync(ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return Db.SaveChangesAsync(ct);
    }

    protected virtual IQueryable<T> ApplySpecification<T>(ISpecification<T> spec, bool evaluateCriteriaOnly = false) where T : class
    {
        return Evaluator.GetQuery(Db.Set<T>().AsQueryable(), spec, evaluateCriteriaOnly);
    }

    protected virtual IQueryable<TResult> ApplySpecification<T, TResult>(ISpecification<T, TResult> spec) where T : class
    {
        return Evaluator.GetQuery(Db.Set<T>().AsQueryable(), spec);
    }
}
