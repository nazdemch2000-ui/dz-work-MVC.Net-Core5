using System.ComponentModel.DataAnnotations;

namespace MVC_Dz5.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public string Genre { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        // Навигационные свойства
        public List<Comment> Comments { get; set; } = new();
        public List<BookImage> Images { get; set; } = new();
    }
}
