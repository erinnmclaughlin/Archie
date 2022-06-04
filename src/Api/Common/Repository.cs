﻿using Ardalis.Specification.EntityFrameworkCore;
using Ardalis.Specification;
using Archie.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace Archie.Api.Common;

public class Repository : IRepository
{
    private ArchieContext Db { get; }
    private ISpecificationEvaluator Evaluator { get; }

    public Repository(ArchieContext db)
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
