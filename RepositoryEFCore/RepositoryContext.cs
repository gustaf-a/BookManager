using Entities.ModelsEf;
using Microsoft.EntityFrameworkCore;
using RepositoryEFCore.Configuration;

namespace RepositoryEFCore;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookConfiguration());
    }

    public DbSet<BookEf>? Books { get; set; }
}
