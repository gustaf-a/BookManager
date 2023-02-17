using Entities.ModelsEf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace RepositoryEFCore.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<BookEf>
{
    public void Configure(EntityTypeBuilder<BookEf> builder)
    {
        var books = JsonSerializer.Deserialize<IEnumerable<BookEf>>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "books.json")));

        builder.HasData(books);

        //builder.HasData
        //(
        //    new BookEf
        //    {
        //        Id = "B1",
        //        Author = "Kutner, Joe",
        //        Title = "Deploying with JRuby",
        //        Genre = "Computer",
        //        Price = 33.00,
        //        PublishDate = "2021-08-15",
        //        Description = "Deploying with JRuby is the missing link between enjoying JRuby and using it in the real world to build high-performance, scalable applications."
        //    },
        //    new BookEf
        //    {
        //        Id = "B",
        //        Author = "",
        //        Title = "",
        //        Genre = "",
        //        Price = 0,
        //        PublishDate = "",
        //        Description = ""
        //    }
        //    //TODO Add more if necessary
        //);
    }
}
