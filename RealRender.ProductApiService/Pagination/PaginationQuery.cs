using System.ComponentModel.DataAnnotations;
namespace RealRender.ProductApiService.Pagination;

public class PaginationQuery
{
    public int PageNumber { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 12;
}
