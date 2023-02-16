using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.ModelsEf;

public class BookEf
{
    [Column("id")]
    [Required(ErrorMessage = $"{nameof(Id)} is a required field.")]
    public string? Id { get; set; }

    [Column("author")]
    public string? Author { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("price")]
    public double Price { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("genre")]
    public string? Genre { get; set; }

    [Column("publish_date")]
    public string PublishDate { get; set; }
}