using Contracts;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Shared;

namespace BookApi;

public static class ExceptionHandlingMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager loggerManager)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        DatabaseAccessException => StatusCodes.Status500InternalServerError,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    loggerManager.LogError($"Global exception handling: Exception thrown by request sent to endpoint: {contextFeature.Endpoint}. Exception: {contextFeature.Error}");

                    await context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message
                    }.ToString());
                }
            });
        });
    }
}
