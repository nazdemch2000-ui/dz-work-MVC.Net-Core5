using GameStore.Controllers;
using GameStore.Interface;
using GameStore.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests;

public class CategoriesControllerTests
{
    [Fact]
    public void Index_ReturnsAllCategories()
    {
        var categories = new[] { new Category { Id = 1, Name = "A" } };
        var mock = new Mock<ICategory>();
        mock.Setup(c => c.GetAllCategories()).Returns(categories);
        var controller = new CategoriesController(mock.Object);

        var result = Assert.IsType<ViewResult>(controller.Index());

        Assert.Same(categories, result.Model);
    }

    [Fact]
    public void UpdateCategory_Get_ReturnsEmptyCategory_WhenIdZero()
    {
        var controller = new CategoriesController(Mock.Of<ICategory>());

        var result = Assert.IsType<ViewResult>(controller.UpdateCategory(0));

        var model = Assert.IsType<Category>(result.Model);
        Assert.Equal(0, model.Id);
    }

    [Fact]
    public void UpdateCategory_Get_LoadsExistingCategory_WhenIdNonZero()
    {
        var existing = new Category { Id = 3, Name = "Strategy" };
        var mock = new Mock<ICategory>();
        mock.Setup(c => c.GetCategory(3)).Returns(existing);
        var controller = new CategoriesController(mock.Object);

        var result = Assert.IsType<ViewResult>(controller.UpdateCategory(3));

        Assert.Same(existing, result.Model);
    }

    [Fact]
    public void UpdateCategory_Post_AddsCategory_WhenIdZero()
    {
        var mock = new Mock<ICategory>();
        var controller = new CategoriesController(mock.Object);
        var category = new Category { Id = 0, Name = "New" };

        var result = Assert.IsType<RedirectToActionResult>(controller.UpdateCategory(category));

        mock.Verify(c => c.AddCategory(category), Times.Once);
        mock.Verify(c => c.UpdateCategory(It.IsAny<Category>()), Times.Never);
        Assert.Equal(nameof(CategoriesController.Index), result.ActionName);
    }

    [Fact]
    public void UpdateCategory_Post_UpdatesCategory_WhenIdNonZero()
    {
        var mock = new Mock<ICategory>();
        var controller = new CategoriesController(mock.Object);
        var category = new Category { Id = 2, Name = "Edit" };

        controller.UpdateCategory(category);

        mock.Verify(c => c.UpdateCategory(category), Times.Once);
        mock.Verify(c => c.AddCategory(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public void DeleteCategory_RemovesCategory_AndRedirects()
    {
        var mock = new Mock<ICategory>();
        var controller = new CategoriesController(mock.Object);
        var category = new Category { Id = 5 };

        var result = Assert.IsType<RedirectToActionResult>(controller.DeleteCategory(category));

        mock.Verify(c => c.DeleteCategory(category), Times.Once);
        Assert.Equal(nameof(CategoriesController.Index), result.ActionName);
    }

    [Fact]
    public void UpdateAll_UpdatesExisting_AndAddsFilledNewRow()
    {
        var mock = new Mock<ICategory>();
        var controller = new CategoriesController(mock.Object);
        var categories = new[]
        {
            new Category { Id = 1, Name = "Existing" },   // update
            new Category { Id = 0, Name = "Fresh" },      // inline add -> kept
            new Category { Id = 0, Name = "   " }          // empty inline row -> ignored
        };

        var result = Assert.IsType<RedirectToActionResult>(controller.UpdateAll(categories));

        mock.Verify(c => c.UpdateAll(It.Is<Category[]>(a => a.Length == 1 && a[0].Id == 1)), Times.Once);
        mock.Verify(c => c.AddCategory(It.Is<Category>(x => x.Name == "Fresh")), Times.Once);
        mock.Verify(c => c.AddCategory(It.IsAny<Category>()), Times.Once); // the blank row was not added
        Assert.Equal(nameof(CategoriesController.Index), result.ActionName);
    }
}
