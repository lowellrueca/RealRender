using RealRender.ProductApiService.Db;
using RealRender.ProductApiService.Models;
namespace RealRender.ProductApiService.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private ApplicationDbContext _context = null!;
    private ProductRepository _productRepository;

    public IRepository<Product> Products => _productRepository ??= new ProductRepository(_context);

    public RepositoryManager(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
