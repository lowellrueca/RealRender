using Microsoft.EntityFrameworkCore;
using RealRender.ProductApiService.Models;
using System.Linq.Expressions;
namespace RealRender.ProductApiService.Repositories;

public class RepositoryBase<TDbContext, TItem> : IRepository<TItem>  where TDbContext : DbContext where TItem : class, IProduct
{
    private readonly TDbContext _context;
    private readonly DbSet<TItem> _dbset;

    public RepositoryBase(TDbContext context)
    {
        _context = context;
        _dbset = _context.Set<TItem>();
    }

    public async Task<TItem> AddItemAsync(TItem item)
    {
        await _dbset.AddAsync(item);
        return item;
    }

    public async Task DeleteItemAsync(Guid id)
    {
        var item = await _dbset.FindAsync(id);
        if (item != null)
        {
            _dbset.Remove(item);
        }
    }

    public async Task<TItem> GetItemAsync(Expression<Func<TItem, bool>> expression, string[] includes = null)
    {
        IQueryable<TItem> queryable = _dbset;
        if (includes != null)
        {
            foreach (var include in includes)
            {
                queryable = queryable.Include(include);
            }
        }

        return await queryable.AsNoTracking().FirstOrDefaultAsync(expression);
    }

    public async Task<IEnumerable<TItem>> GetAllItemsAsync(Expression<Func<TItem, bool>> expression = null, Func<IQueryable<TItem>, IOrderedQueryable<TItem>> orderBy = null, string[] includes = null)
    {
        IQueryable<TItem> queryable = _dbset;
        if (expression != null)
        {
            queryable = queryable.Where(expression);
        }

        if (includes != null)
        {
            foreach (var include in includes)
            {
                queryable = queryable.Include(include);
            }
        }

        if (orderBy != null)
        {
            queryable = orderBy(queryable);
        }

        return await queryable.AsNoTracking().ToListAsync();
    }

    public async Task UpdateItemAsync(TItem item)
    {
        _dbset.Attach(item);
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
