using MVC_Dz5.Models;

namespace MVC_Dz5.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db)
        {
            // Если книги уже есть — не дублируем
            if (db.Books.Any())
                return;

            var books = new List<Book>
            {
                new Book
                {
                    Title = "Кобзар",
                    Author = "Тарас Шевченко",
                    Genre = "Поезія",
                    Price = 250m,
                    Images = new List<BookImage>
                    {
                        new BookImage { Url = "https://picsum.photos/seed/kobzar/300/400" }
                    }
                },
                new Book
                {
                    Title = "Тіні забутих предків",
                    Author = "Михайло Коцюбинський",
                    Genre = "Повість",
                    Price = 180m,
                    Images = new List<BookImage>
                    {
                        new BookImage { Url = "https://picsum.photos/seed/tini/300/400" }
                    }
                }
            };

            db.Books.AddRange(books);
            db.SaveChanges();
        }
    }
}
