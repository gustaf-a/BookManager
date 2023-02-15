using Serilog;

namespace BookApi;

public class Program
{
    public static void Main(string[] args)
    {
        ConfigureLogging();

        var builder = WebApplication.CreateBuilder(args);

        var startup = new Startup();
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app);

        app.Run();
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File($"logs/{nameof(BookApi)}.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}