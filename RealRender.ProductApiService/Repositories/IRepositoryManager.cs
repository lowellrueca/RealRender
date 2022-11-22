using RealRender.ProductApiService.Models;
namespace RealRender.ProductApiService.Repositories;

public interface IRepositoryManager
{
    public IRepository<Product> Products { get; }

    public Task SaveAsync();
}
