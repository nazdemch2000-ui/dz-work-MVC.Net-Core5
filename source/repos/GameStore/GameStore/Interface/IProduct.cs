using GameStore.Models;
using GameStore.Pages;

namespace GameStore.Interface
{
    public interface IProduct
    {
        IEnumerable<Product> GetAllProducts();
        PagedList<Product> GetProducts(QueryOptions options);
        void AddProduct(Product product);
        Product? GetProduct(int id);
        void UpdateProduct(Product product);
        void UpdateAll(Product[] products);
        void DeleteProduct(Product product);
    }
}
