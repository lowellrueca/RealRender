namespace RealRender.ProductApiService.Models;

public interface IProduct
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
