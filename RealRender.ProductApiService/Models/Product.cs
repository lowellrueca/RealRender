namespace RealRender.ProductApiService.Models;

public class Product : IProduct
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
