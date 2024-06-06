using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JLMS.Models
{
    public class BooksExtendedInformation
    {
        [Key]
        [ForeignKey("Book")]
        public string BookISBN13 { get; set; }

        public string ImageURL { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Author { get; set; }
        public string Synopsis { get; set; }

        public Book Book { get; set; }
    }
}
