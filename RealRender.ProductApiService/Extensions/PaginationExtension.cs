using Microsoft.AspNetCore.Mvc;
using RealRender.ProductApiService.Pagination;

namespace RealRender.ProductApiService.Extensions;

public static class PaginationExtension
{
    public static IPagination<TDto> Paginate<TDto>(this IEnumerable<TDto> source, PaginationQuery paginationQuery, string endpointName, IUrlHelper url)
    {
        var pageNumber = paginationQuery.PageNumber;
        var pageSize = paginationQuery.PageSize;
        var start = (pageNumber - 1) * pageSize;
        var totalItems = source.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var data = source.Skip(start).Take(pageSize);
        var hasPreviousPage = pageNumber > 1;
        var hasNextPage = pageNumber < totalPages;

        IPagination<TDto> pagedResponse = new Pagination<TDto>()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItem = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = hasPreviousPage,
            HasNextPage = hasNextPage,
            Data = data,
        };
        pagedResponse.GenerateLink(endpointName, url);
        return pagedResponse;
    }

    public static void GenerateLink<TDto>(this IPagination<TDto> ipage, string endpointName, IUrlHelper url)
    {
        #region first page
        var firstPage = url.RouteUrl(endpointName, new { PageNumber = 1, PageSize = ipage.PageSize });
        ipage.AddLink(PageNavigation.First, new PageUrl { Href = firstPage });
        #endregion

        #region previous page
        var previousPage = url.RouteUrl(endpointName, new { PageNumber = ipage.PageNumber - 1, PageSize = ipage.PageSize });
        if (!ipage.HasPreviousPage)
        {
            ipage.AddLink(PageNavigation.Previous, new PageUrl { Href = String.Empty });
        }

        if (ipage.HasPreviousPage)
        {
            ipage.AddLink(PageNavigation.Previous, new PageUrl { Href = previousPage });
        }
        #endregion

        #region next page 
        var nextPage = url.RouteUrl(endpointName, new { PageNumber = ipage.PageNumber + 1, PageSize = ipage.PageSize });
        if (!ipage.HasNextPage)
        {
            ipage.AddLink(PageNavigation.Next, new PageUrl { Href = String.Empty });
        }
        if (ipage.HasNextPage)
        {
            ipage.AddLink(PageNavigation.Next, new PageUrl { Href = nextPage });
        }
        #endregion

        #region last page
        var lastPage = url.RouteUrl(endpointName, new { PageNumber = ipage.TotalPages, PageSize = ipage.PageSize });
        ipage.AddLink(PageNavigation.Last, new PageUrl { Href = lastPage });
        #endregion
    }

    public static void AddLink<TDto>(this IPagination<TDto> ipage, PageNavigation nav, PageUrl url)
    {
        ipage.Links ??= new Dictionary<PageNavigation, PageUrl>();
        ipage.Links[nav] = url;
    }
}
