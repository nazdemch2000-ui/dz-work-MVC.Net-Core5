using GameStore.Interface;
using GameStore.Models;
using GameStore.Pages;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
        private readonly IProduct _products;
        private readonly ICategory _categories;

        public HomeController(IProduct products, ICategory categories)
        {
            _products = products;
            _categories = categories;
        }
        [HttpGet]
        public IActionResult Index(QueryOptions options)
        {
            ViewBag.searches = new[] { "Name", "Category.Name" };
            ViewBag.sorts = new[] { "Name", "Category.Name", "PurchasePrice", "RetailPrice" };
            return View(_products.GetProducts(options));
        }
        [HttpGet]
        public IActionResult UpdateProduct(int id)
        {
            ViewBag.Categories = _categories.GetAllCategories();
            return View(id == 0 ? new Product() : _products.GetProduct(id));
        }
        [HttpPost]
        public IActionResult UpdateProduct(Product product)
        {
            if (product.Id == 0)
            {
                _products.AddProduct(product);
            }
            else
            {
                _products.UpdateProduct(product);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult DeleteProduct(Product product)
        {
            _products.DeleteProduct(product);
            return RedirectToAction(nameof(Index));
        }

        // Масове оновлення товарів (+ inline-додавання нового товару порожнім рядком)
        [HttpGet]
        public IActionResult UpdateAll()
        {
            // Матеріалізуємо обидві вибірки, щоб уникнути вкладеного читання одного з'єднання.
            ViewBag.Categories = _categories.GetAllCategories().ToList();
            return View(_products.GetAllProducts().ToList());
        }
        [HttpPost]
        public IActionResult UpdateAll(Product[] products)
        {
            // Існуючі товари оновлюємо, порожній рядок із заповненою назвою додаємо.
            _products.UpdateAll(products.Where(p => p.Id > 0).ToArray());
            foreach (Product product in products.Where(p => p.Id == 0 && !string.IsNullOrWhiteSpace(p.Name)))
            {
                _products.AddProduct(product);
            }
            return RedirectToAction(nameof(Index));
        }
}