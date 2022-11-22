using System.ComponentModel.DataAnnotations;
namespace RealRender.ProductApiService.Dto;

public class ProductWriteDto : IProductDto
{
    [Required]
    public string Name { get; set; }
}
