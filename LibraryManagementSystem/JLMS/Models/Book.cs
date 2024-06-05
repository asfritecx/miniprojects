using System.ComponentModel.DataAnnotations;

namespace JLMS.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        [StringLength(13)]
        public string BookISBN13 { get; set; }

        [Required]
        [StringLength(255)]
        public string BookTitle { get; set; }

        [Required]
        [Range(1, 5)]
        public int Quantity { get; set; }

        [Required]
        [StringLength(6)]
        public string ShelfNo { get; set; }
    }
}
