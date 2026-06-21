using Domain.Common.Responses;
using Domain.Entities.Abstracts;
using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories;

internal interface IBaseRepository<T> where T : Entity
{
    Task CreateAsync(T entity, CancellationToken cancellationToken);
    Task<T> CreateReturnEntity(T entity, CancellationToken cancellationToken);
    void Update(T entity);
    void Delete(T entity);

    Task<List<T>> GetAll(CancellationToken cancellationToken, int skip = 0, int take = 10);

    Task<T> GetWithParametersAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<List<T>> GetAllWithParametersAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        int skip = 0,
        int take = 10,
        params Expression<Func<T, object>>[] includes);

    void Attach<TConcrete>(TConcrete entity) where TConcrete : Entity;

    Task<List<TResult>> GetAllProjectedAsync<TResult>(
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, TResult>> selector = null!,
        CancellationToken cancellationToken = default,
        int skip = 0,
        int take = 10,
        params Expression<Func<T, object>>[] includes);

    Task<List<T>> GetAllWithParametersAsyncWithTracking(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        int skip = 0,
        int take = 10,
        params Expression<Func<T, object>>[] includes);

    Task<T?> GetWithParametersAsyncWithTracking(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<TResult> GetProjectedAsync<TResult>(
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, TResult>> selector = null!,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<List<TResult>> GetAllProjectedWithTrackingAsync<TResult>(
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, TResult>> selector = null!,
        CancellationToken cancellationToken = default,
        int skip = 0,
        int take = 10,
        params Expression<Func<T, object>>[] includes);
    Task<TResult> GetProjectedWithTrackingAsync<TResult>(
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, TResult>> selector = null!,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<PagedResult<T>> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);
}