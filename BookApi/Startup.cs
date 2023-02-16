using BookApi.Services;
using Contracts;
using RepositorySql;
using RepositorySql.Configuration;
using RepositorySql.Database;
using RepositorySql.Database.SQLite;

namespace BookApi;

public class Startup
{
    private readonly ConfigurationManager _configurationManager;

    public Startup(ConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<DatabaseOptions>(_configurationManager.GetSection(DatabaseOptions.Database));
        services.Configure<ConnectionStringsOptions>(_configurationManager.GetSection(ConnectionStringsOptions.ConnectionString));

        services.AddControllers();

        services.ConfigureCors();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1",
                new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BookApi", Version = "v1" });
        });

        services.AddSingleton<IBookRepository, DatabaseBookRepository>();
        services.AddSingleton<IDatabaseAccess, SqliteDatabaseAccess>();
        services.AddSingleton<IDatabaseQueryCreator, SqliteDatabaseQueryCreator>();
        services.AddSingleton<IDatabaseIdGenerator, SqliteDatabaseIdGenerator>();

        services.AddSingleton<IBookService, BookService>();
    }

    public void Configure(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //Adds global exception handling
        app.ConfigureExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}
