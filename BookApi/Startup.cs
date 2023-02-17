using Contracts;
using LoggerService;
using Microsoft.FeatureManagement;
using NLog;
using RepositorySql;
using RepositorySql.Configuration;
using RepositorySql.Database;
using RepositorySql.Database.SQLite;
using Service;
using Service.Contracts;
using Service.SQL;

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
        LogManager.LoadConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));

        services.Configure<DatabaseOptions>(_configurationManager.GetSection(DatabaseOptions.Database));
        services.Configure<ConnectionStringsOptions>(_configurationManager.GetSection(ConnectionStringsOptions.ConnectionString));

        services.AddControllers()
            .AddApplicationPart(typeof(BookApi.Presentation.AssemblyReference).Assembly);

        services.ConfigureCors();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1",
                new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BookApi", Version = "v1" });
        });

        services.AddSingleton<ILoggerManager, LoggerManager>();

        services.AddFeatureManagement();

        services.AddSingleton<IServiceManagerFactory, ServiceManagerFactory>();
        
        //------------ SQL Services ------------------
        services.AddSingleton<IBookRepository, DatabaseBookRepository>();
        services.AddSingleton<IDatabaseAccess, SqliteDatabaseAccess>();
        services.AddSingleton<IDatabaseQueryCreator, SqliteDatabaseQueryCreator>();
        services.AddSingleton<IDatabaseIdGenerator, SqliteDatabaseIdGenerator>();

        services.AddSingleton<IBookService, BookService>();
        services.AddSingleton<ServiceSqlManager>();

        //------------ EF Core Services ---------------

        //services.ConfigureRespositoryManager();
        //services.ConfigureServiceManager();
        //services.ConfigureSqlServerContext(_configurationManager);
    }

    public void Configure(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.ConfigureExceptionHandler(app.Services.GetService<ILoggerManager>());

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}
