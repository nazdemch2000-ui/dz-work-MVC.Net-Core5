using GameStore.Data;
using GameStore.Interface;
using GameStore.Models;

namespace GameStore.Reposetory
{
    public class CategoryRepository : ICategory
    {
        private ApplicationContext _context;

        public CategoryRepository(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories;
        }

        public Category? GetCategory(int id)
        {
            return _context.Categories.FirstOrDefault(e => e.Id == id);
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void UpdateAll(Category[] categories)
        {
            Dictionary<int, Category> data = categories.ToDictionary(e => e.Id);
            IEnumerable<Category> baseline = _context.Categories.Where(e => data.Keys.Contains(e.Id));
            foreach (Category category in baseline)
            {
                Category request = data[category.Id];
                category.Name = request.Name;
                category.Description = request.Description;
            }
            _context.SaveChanges();
        }

        public void DeleteCategory(Category category)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
    }



}
