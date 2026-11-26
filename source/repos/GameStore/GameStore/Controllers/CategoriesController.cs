using GameStore.Interface;
using GameStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategory _categories;

        public CategoriesController(ICategory categories)
        {
            _categories = categories;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_categories.GetAllCategories());
        }

        [HttpGet]
        public IActionResult UpdateCategory(int id)
        {
            return View(id == 0 ? new Category() : _categories.GetCategory(id));
        }

        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            if (category.Id == 0)
            {
                _categories.AddCategory(category);
            }
            else
            {
                _categories.UpdateCategory(category);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DeleteCategory(Category category)
        {
            _categories.DeleteCategory(category);
            return RedirectToAction(nameof(Index));
        }

        // Масове оновлення категорій (+ inline-додавання нової категорії порожнім рядком)
        [HttpGet]
        public IActionResult UpdateAll()
        {
            return View(_categories.GetAllCategories());
        }

        [HttpPost]
        public IActionResult UpdateAll(Category[] categories)
        {
            _categories.UpdateAll(categories.Where(c => c.Id > 0).ToArray());
            foreach (Category category in categories.Where(c => c.Id == 0 && !string.IsNullOrWhiteSpace(c.Name)))
            {
                _categories.AddCategory(category);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
