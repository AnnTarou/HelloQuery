using System.ComponentModel.DataAnnotations.Schema;

namespace HelloQuery.Models
{
    [Table("books")]
    public class Book
    {
        [Column("book_id")]
        public int BookId { get; set; }

        [Column("title", TypeName = "NVARCHAR(25)")]
        public string Title { get; set; }

        [Column("author", TypeName = "NVARCHAR(10)")]
        public string Author { get; set; }

        [Column("publication_date", TypeName = "DATE")]
        public DateTime PublicationDate { get; set; }

        [Column("pages")]
        public int Pages { get; set; }

        [Column("price")]
        public int Price { get; set; }
    }
}