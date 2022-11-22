using System.Text.Json;
namespace RealRender.ProductApiService.Exceptions;

public class ExceptionDetail
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
