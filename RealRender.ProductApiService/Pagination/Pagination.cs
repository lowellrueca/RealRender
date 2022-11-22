namespace RealRender.ProductApiService.Pagination;

public class Pagination<TDto> : IPagination<TDto>
{
    int pageNumber;
    int pageSize;
    const int pageMaxSize = 12;
    public IEnumerable<TDto> Data { get; set; } = null!;
    public int PageNumber { get => pageNumber; set => pageNumber = (value < 1) ? 1 : value; }
    public int PageSize { get => pageSize; set => pageSize = (value > pageMaxSize) ? pageMaxSize : value; }
    public int TotalItem { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public IDictionary<PageNavigation, PageUrl> Links { get; set; } = null!;
}