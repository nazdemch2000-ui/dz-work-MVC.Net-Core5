using System.ComponentModel.DataAnnotations;

namespace MVC_Dz5.Models
{
    public class BookImage
    {
        public int Id { get; set; }

        // Ссылка на изображение (URL или путь вида /images/books/..), хранится в БД
        [Required]
        public string Url { get; set; } = string.Empty;

        // Внешний ключ + навигация к книге
        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
