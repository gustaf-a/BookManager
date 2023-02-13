using BookApi.Data;
using System.Text.Json;

namespace BookApi.Middleware;

/// <summary>
/// Checks contentType and processes request body if set to "application/json".
/// Stores Book object in HttpContext.
/// </summary>
public class JsonToBookMiddleware
{
    private readonly RequestDelegate _next;

    private readonly List<string> _allowedContentTypes= new()
    {
        "application/json",
        "text/plain"
    };

	public JsonToBookMiddleware(RequestDelegate next)
	{
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType == null
            || !_allowedContentTypes.Select(ct => context.Request.ContentType.Contains(ct)).Any())
        {
            await _next(context);
            return;
        }

        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var book = JsonSerializer.Deserialize<Book>(requestBody);
        context.Items["book"] = book;

        await _next(context);
    }
}
