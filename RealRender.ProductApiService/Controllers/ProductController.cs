using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealRender.ProductApiService.Dto;
using RealRender.ProductApiService.Extensions;
using RealRender.ProductApiService.Filters;
using RealRender.ProductApiService.Models;
using RealRender.ProductApiService.Pagination;
using RealRender.ProductApiService.Repositories;
using RealRender.ProductApiService.Responses;
namespace RealRender.ProductApiService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IRepositoryManager _repositoryManager;
    private readonly IMapper _mapper;

    public ProductController(ILogger<ProductController> logger, IRepositoryManager repositoryManager, IMapper mapper)
    {
        _logger = logger;
        _repositoryManager = repositoryManager;
        _mapper = mapper;
    }

    [HttpGet(Name = nameof(GetAllProductsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPagination<ProductReadDto>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ServiceFilter(typeof(AnyDbContentFilter))]
    public async Task<IActionResult> GetAllProductsAsync([FromQuery] PaginationQuery parameters)
    {
        var products = await _repositoryManager.Products.GetAllItemsAsync();
        var items = _mapper.Map<IEnumerable<ProductReadDto>>(products);
        var data = items.Paginate(parameters, nameof(GetAllProductsAsync), Url);
        return Ok(data);
    }

    [HttpGet("{id}", Name = nameof(GetProductByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<ProductReadDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductByIdAsync([FromRoute] Guid id)
    {
        var product = await _repositoryManager.Products.GetItemAsync(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        var item = _mapper.Map<ProductReadDto>(product);
        var response = new Response<ProductReadDto> { Data = item };
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProductAsync([FromBody] ProductWriteDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = _mapper.Map<Product>(dto);
        var newProduct = await _repositoryManager.Products.AddItemAsync(product);
        await _repositoryManager.SaveAsync();
        var item = _mapper.Map<ProductReadDto>(newProduct);
        return CreatedAtRoute(nameof(GetProductByIdAsync), new { id = item.Id }, item);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProductAsync([FromRoute] Guid id, [FromBody] ProductWriteDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _repositoryManager.Products.GetItemAsync(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        _mapper.Map(dto, product);
        await _repositoryManager.Products.UpdateItemAsync(product);
        await _repositoryManager.SaveAsync();
        var item = _mapper.Map<ProductReadDto>(product);
        return CreatedAtRoute(nameof(GetProductByIdAsync), new { id = item.Id }, item);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProductAsync([FromRoute] Guid id)
    {
        await _repositoryManager.Products.DeleteItemAsync(id);
        await _repositoryManager.SaveAsync();
        return NoContent();
    }
}
