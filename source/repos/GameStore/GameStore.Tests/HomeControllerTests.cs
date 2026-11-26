using GameStore.Interface;
using GameStore.Models;
using GameStore.Pages;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests;

public class HomeControllerTests
{
    [Fact]
    public void Index_ReturnsPagedProducts()
    {
        var products = new[] { new Product { Id = 1, Name = "A" }, new Product { Id = 2, Name = "B" } };
        var paged = new PagedList<Product>(products.AsQueryable(), new QueryOptions());
        var productMock = new Mock<IProduct>();
        productMock.Setup(p => p.GetProducts(It.IsAny<QueryOptions>())).Returns(paged);
        var controller = new HomeController(productMock.Object, Mock.Of<ICategory>());

        var result = Assert.IsType<ViewResult>(controller.Index(new QueryOptions()));

        var model = Assert.IsType<PagedList<Product>>(result.Model);
        Assert.Equal(2, model.Count);
        productMock.Verify(p => p.GetProducts(It.IsAny<QueryOptions>()), Times.Once);
    }

    [Fact]
    public void UpdateProduct_Get_ReturnsEmptyProduct_WhenIdZero()
    {
        var categoryMock = new Mock<ICategory>();
        categoryMock.Setup(c => c.GetAllCategories()).Returns(Array.Empty<Category>());
        var controller = new HomeController(Mock.Of<IProduct>(), categoryMock.Object);

        var result = Assert.IsType<ViewResult>(controller.UpdateProduct(0));

        var model = Assert.IsType<Product>(result.Model);
        Assert.Equal(0, model.Id);
    }

    [Fact]
    public void UpdateProduct_Get_LoadsExistingProduct_WhenIdNonZero()
    {
        var existing = new Product { Id = 7, Name = "Existing" };
        var productMock = new Mock<IProduct>();
        productMock.Setup(p => p.GetProduct(7)).Returns(existing);
        var categoryMock = new Mock<ICategory>();
        categoryMock.Setup(c => c.GetAllCategories()).Returns(Array.Empty<Category>());
        var controller = new HomeController(productMock.Object, categoryMock.Object);

        var result = Assert.IsType<ViewResult>(controller.UpdateProduct(7));

        Assert.Same(existing, result.Model);
    }

    [Fact]
    public void UpdateProduct_Post_AddsProduct_WhenIdZero()
    {
        var productMock = new Mock<IProduct>();
        var controller = new HomeController(productMock.Object, Mock.Of<ICategory>());
        var product = new Product { Id = 0, Name = "New" };

        var result = Assert.IsType<RedirectToActionResult>(controller.UpdateProduct(product));

        productMock.Verify(p => p.AddProduct(product), Times.Once);
        productMock.Verify(p => p.UpdateProduct(It.IsAny<Product>()), Times.Never);
        Assert.Equal(nameof(HomeController.Index), result.ActionName);
    }

    [Fact]
    public void UpdateProduct_Post_UpdatesProduct_WhenIdNonZero()
    {
        var productMock = new Mock<IProduct>();
        var controller = new HomeController(productMock.Object, Mock.Of<ICategory>());
        var product = new Product { Id = 3, Name = "Edit" };

        var result = Assert.IsType<RedirectToActionResult>(controller.UpdateProduct(product));

        productMock.Verify(p => p.UpdateProduct(product), Times.Once);
        productMock.Verify(p => p.AddProduct(It.IsAny<Product>()), Times.Never);
        Assert.Equal(nameof(HomeController.Index), result.ActionName);
    }

    [Fact]
    public void DeleteProduct_RemovesProduct_AndRedirects()
    {
        var productMock = new Mock<IProduct>();
        var controller = new HomeController(productMock.Object, Mock.Of<ICategory>());
        var product = new Product { Id = 5 };

        var result = Assert.IsType<RedirectToActionResult>(controller.DeleteProduct(product));

        productMock.Verify(p => p.DeleteProduct(product), Times.Once);
        Assert.Equal(nameof(HomeController.Index), result.ActionName);
    }

    [Fact]
    public void UpdateAll_Get_ReturnsProducts_AndLoadsCategories()
    {
        var products = new[] { new Product { Id = 1, Name = "A" } };
        var categories = new[] { new Category { Id = 1, Name = "Action" } };
        var productMock = new Mock<IProduct>();
        productMock.Setup(p => p.GetAllProducts()).Returns(products);
        var categoryMock = new Mock<ICategory>();
        categoryMock.Setup(c => c.GetAllCategories()).Returns(categories);
        var controller = new HomeController(productMock.Object, categoryMock.Object);

        var result = Assert.IsType<ViewResult>(controller.UpdateAll());

        var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
        Assert.Equal(products, model);
        var loadedCategories = Assert.IsAssignableFrom<IEnumerable<Category>>(controller.ViewBag.Categories);
        Assert.Equal(categories, loadedCategories);
    }

    [Fact]
    public void UpdateAll_Post_UpdatesExisting_AndAddsFilledNewRow()
    {
        var productMock = new Mock<IProduct>();
        var controller = new HomeController(productMock.Object, Mock.Of<ICategory>());
        var products = new[]
        {
            new Product { Id = 1, Name = "Existing" }, // update
            new Product { Id = 0, Name = "Fresh" },    // inline add -> kept
            new Product { Id = 0, Name = "" }          // empty inline row -> ignored
        };

        var result = Assert.IsType<RedirectToActionResult>(controller.UpdateAll(products));

        productMock.Verify(p => p.UpdateAll(It.Is<Product[]>(a => a.Length == 1 && a[0].Id == 1)), Times.Once);
        productMock.Verify(p => p.AddProduct(It.Is<Product>(x => x.Name == "Fresh")), Times.Once);
        productMock.Verify(p => p.AddProduct(It.IsAny<Product>()), Times.Once); // blank row not added
        Assert.Equal(nameof(HomeController.Index), result.ActionName);
    }
}
