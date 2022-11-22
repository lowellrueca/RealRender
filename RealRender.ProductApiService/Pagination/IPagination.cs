namespace RealRender.ProductApiService.Pagination;

public interface IPagination<TDto>
{
    public IEnumerable<TDto> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItem { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public IDictionary<PageNavigation, PageUrl> Links { get; set; }
}
