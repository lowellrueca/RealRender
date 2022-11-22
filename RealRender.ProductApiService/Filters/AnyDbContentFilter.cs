using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RealRender.ProductApiService.Db;
namespace RealRender.ProductApiService.Filters;

public class AnyDbContentFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        using (var scope = context.HttpContext.RequestServices.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (!dbContext.Products.Any())
            {
                context.Result = new NoContentResult();
            }
        }
    }
}
