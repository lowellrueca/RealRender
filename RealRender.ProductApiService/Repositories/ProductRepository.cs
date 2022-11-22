using RealRender.ProductApiService.Db;
using RealRender.ProductApiService.Models;

namespace RealRender.ProductApiService.Repositories;

public class ProductRepository : RepositoryBase<ApplicationDbContext, Product>
{
    public ProductRepository(ApplicationDbContext context) 
        : base(context)
    {

    }
}
