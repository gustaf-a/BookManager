using Contracts;
using Contracts.EF;
using IdGeneratorService;
using Microsoft.AspNetCore.Diagnostics;
using RepositoryEFCore;
using RepositorySql;
using Service.EF;
using Service.SQL;
using Shared;
using System.Net;

namespace BookApi;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }

    /// <summary>
    /// Adds Exception handling middleware
    /// </summary>
    public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager loggerManager)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    loggerManager.LogError($"Global exception handling: Exception thrown by request sent to endpoint: {contextFeature.Endpoint}. Message: {contextFeature.Error.Message}");

                    await context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = $"Internal Server Error: {contextFeature.Error.Message}"
                    }.ToString());
                }
            });
        });
    }

    public static void ConfigureEfCoreServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
    {
        services.AddScoped<ServiceEfManager>();

        services.AddScoped<IRepositoryManager, RepositoryManager>();

        services.AddSqlServer<RepositoryContext>(configurationRoot.GetConnectionString("sqlConnection"));
    }

    public static void ConfigureSqliteServices(this IServiceCollection services)
    {
        services.AddScoped<ServiceSqlManager>();

        services.AddSingleton<IBookRepository, DatabaseBookRepository>();
        services.AddSingleton<IDatabaseAccess, SqliteDatabaseAccess>();
        services.AddSingleton<IDatabaseQueryCreator, SqliteDatabaseQueryCreator>();
    }
}
