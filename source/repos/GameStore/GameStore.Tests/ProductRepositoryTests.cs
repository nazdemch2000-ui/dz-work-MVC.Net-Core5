using GameStore.Models;
using GameStore.Pages;
using GameStore.Reposetory;

namespace GameStore.Tests;

public class ProductRepositoryTests
{
    private static Product NewProduct(string name = "Game", int categoryId = 1) => new()
    {
        Name = name,
        CategoryId = categoryId,
        PurchasePrice = 10m,
        RetailPrice = 20m
    };

    [Fact]
    public void AddProduct_PersistsProduct()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);

        repo.AddProduct(NewProduct("Halo"));

        Assert.Single(context.Products);
        Assert.Equal("Halo", context.Products.Single().Name);
    }

    [Fact]
    public void GetAllProducts_ReturnsAllWithCategory()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);
        repo.AddProduct(NewProduct("A", 1));
        repo.AddProduct(NewProduct("B", 2));

        var all = repo.GetAllProducts().ToList();

        Assert.Equal(2, all.Count);
        Assert.All(all, p => Assert.NotNull(p.Category)); // Include(e => e.Category) populated
    }

    [Fact]
    public void GetProduct_ReturnsMatchingProduct()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);
        var product = NewProduct("Doom");
        repo.AddProduct(product);

        var found = repo.GetProduct(product.Id);

        Assert.NotNull(found);
        Assert.Equal("Doom", found!.Name);
    }

    [Fact]
    public void GetProduct_ReturnsNull_WhenMissing()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);

        Assert.Null(repo.GetProduct(999));
    }

    [Fact]
    public void UpdateProduct_ChangesScalarFields()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);
        var product = NewProduct("Old", 1);
        repo.AddProduct(product);

        repo.UpdateProduct(new Product
        {
            Id = product.Id,
            Name = "New",
            CategoryId = 2,
            PurchasePrice = 5m,
            RetailPrice = 15m
        });

        var updated = context.Products.Single();
        Assert.Equal("New", updated.Name);
        Assert.Equal(2, updated.CategoryId);
        Assert.Equal(5m, updated.PurchasePrice);
        Assert.Equal(15m, updated.RetailPrice);
    }

    [Fact]
    public void DeleteProduct_RemovesProduct()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);
        var product = NewProduct();
        repo.AddProduct(product);

        repo.DeleteProduct(product);

        Assert.Empty(context.Products);
    }

    [Fact]
    public void GetProducts_AppliesPaging()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);
        for (int i = 1; i <= 25; i++)
        {
            repo.AddProduct(new Product { Name = $"P{i:00}", CategoryId = 1, PurchasePrice = i, RetailPrice = i });
        }

        var page = repo.GetProducts(new QueryOptions { CurrentPage = 2, PageSize = 10 });

        Assert.Equal(10, page.Count);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(2, page.CurrentPage);
        Assert.True(page.HasPreviousPage);
        Assert.True(page.HasNextPage);
    }

    [Fact]
    public void GetProducts_SearchByName_Filters()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);
        repo.AddProduct(new Product { Name = "Doom", CategoryId = 1 });
        repo.AddProduct(new Product { Name = "Halo", CategoryId = 1 });

        var page = repo.GetProducts(new QueryOptions
        {
            SearchPropertyName = "Name",
            SearchTerm = "Doom",
            PageSize = 100
        });

        Assert.Single(page);
        Assert.Equal("Doom", page[0].Name);
    }

    [Fact]
    public void GetProducts_SearchByCategoryName_Filters()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);
        repo.AddProduct(new Product { Name = "G1", CategoryId = 1 }); // Action
        repo.AddProduct(new Product { Name = "G2", CategoryId = 2 }); // RPG

        var page = repo.GetProducts(new QueryOptions
        {
            SearchPropertyName = "Category.Name",
            SearchTerm = "RPG",
            PageSize = 100
        });

        Assert.Single(page);
        Assert.Equal("G2", page[0].Name);
    }

    [Fact]
    public void GetProducts_SortByRetailPriceDescending()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);
        repo.AddProduct(new Product { Name = "Cheap", CategoryId = 1, RetailPrice = 5 });
        repo.AddProduct(new Product { Name = "Expensive", CategoryId = 1, RetailPrice = 50 });
        repo.AddProduct(new Product { Name = "Mid", CategoryId = 1, RetailPrice = 20 });

        var page = repo.GetProducts(new QueryOptions
        {
            OrderPropertyName = "RetailPrice",
            DescendingOrder = true,
            PageSize = 100
        });

        Assert.Equal(new[] { "Expensive", "Mid", "Cheap" }, page.Select(p => p.Name).ToArray());
    }

    [Fact]
    public void UpdateAll_UpdatesEveryKnownProduct()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new ProductRepository(context);
        var p1 = NewProduct("P1", 1);
        var p2 = NewProduct("P2", 1);
        repo.AddProduct(p1);
        repo.AddProduct(p2);

        repo.UpdateAll(new[]
        {
            new Product { Id = p1.Id, Name = "P1-new", CategoryId = 2, PurchasePrice = 1m, RetailPrice = 2m },
            new Product { Id = p2.Id, Name = "P2-new", CategoryId = 3, PurchasePrice = 3m, RetailPrice = 4m }
        });

        var updated1 = context.Products.Single(p => p.Id == p1.Id);
        var updated2 = context.Products.Single(p => p.Id == p2.Id);
        Assert.Equal("P1-new", updated1.Name);
        Assert.Equal(2, updated1.CategoryId); // CategoryId is updated (was the bug)
        Assert.Equal("P2-new", updated2.Name);
        Assert.Equal(3, updated2.CategoryId);
    }
}
