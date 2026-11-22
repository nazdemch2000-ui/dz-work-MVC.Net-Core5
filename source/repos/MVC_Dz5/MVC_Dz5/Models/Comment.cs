using System.ComponentModel.DataAnnotations;

namespace MVC_Dz5.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public string? Author { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Внешний ключ + навигация к книге
        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
