using System.Linq.Expressions;

namespace RealRender.ProductApiService.Repositories;

public interface IRepository<T>
{
    public Task<IEnumerable<T>> GetAllItemsAsync(Expression<Func<T, bool>> expression = null,
                                             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                             string[] includes = null);
    public Task<T> GetItemAsync(Expression<Func<T, bool>> expression, string[] includes = null);
    public Task<T> AddItemAsync(T item);
    public Task UpdateItemAsync(T item);
    public Task DeleteItemAsync(Guid id);
}
