using Entities.ModelsEf;
using RepositoryEFCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace BookApi.ContextFolder;

/// <summary>
/// Creates a DbContext instance which will aid migrations.
/// Used during design time.

/// </summary>
public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Needed because RepositoryEFCore is separate project.
        var builder = new DbContextOptionsBuilder<RepositoryContext>()
            .UseSqlServer(configuration.GetConnectionString(BooksDbEfNames.SqlConnectionString),
                    sqlBuilder => sqlBuilder.MigrationsAssembly(BooksDbEfNames.Database));
        
        return new RepositoryContext(builder.Options);
    }
}
