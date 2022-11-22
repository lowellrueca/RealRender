using AutoMapper;
using RealRender.ProductApiService.Dto;
using RealRender.ProductApiService.Models;
namespace RealRender.ProductApiService.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductReadDto>();
        CreateMap<ProductWriteDto, Product>();
    }
}
