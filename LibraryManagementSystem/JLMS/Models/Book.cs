using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JLMS.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        [MaxLength(13)]
        public string BookISBN13 { get; set; }

        [Required]
        [MaxLength(255)]
        public string BookTitle { get; set; }

        [Range(1, 5)]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(15)]
        public string ShelfNo { get; set; }

        public BooksExtendedInformation BooksExtendedInformation { get; set; }
    }
}
