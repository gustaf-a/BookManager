using Contracts;
using Contracts.EF;
using RepositoryEFCore;
using RepositorySql;
using Service.EF;
using Service.SQL;

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

    public static void ConfigureEfCoreServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
    {
        services.AddScoped<ServiceEfManager>();

        services.AddScoped<IRepositoryManager, RepositoryManager>();

        services.AddSqlServer<RepositoryContext>(
            configurationRoot.GetConnectionString("sqlConnection"), 
            serviceProviderOptions => serviceProviderOptions.EnableRetryOnFailure());
    }

    public static void ConfigureSqliteServices(this IServiceCollection services)
    {
        services.AddScoped<ServiceSqlManager>();

        services.AddSingleton<IBookRepository, DatabaseBookRepository>();
        services.AddSingleton<IDatabaseAccess, SqliteDatabaseAccess>();
        services.AddSingleton<IDatabaseQueryCreator, SqliteDatabaseQueryCreator>();
    }
}
