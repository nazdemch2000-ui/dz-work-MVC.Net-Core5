using GameStore.Models;
using GameStore.Reposetory;

namespace GameStore.Tests;

public class CategoryRepositoryTests
{
    [Fact]
    public void GetAllCategories_ReturnsSeededCategories()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new CategoryRepository(context);

        var categories = repo.GetAllCategories().ToList();

        // Five categories are seeded in ApplicationContext.OnModelCreating.
        Assert.Equal(5, categories.Count);
        Assert.Contains(categories, c => c.Name == "Action");
    }

    [Fact]
    public void AddCategory_PersistsCategory()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new CategoryRepository(context);

        repo.AddCategory(new Category { Name = "Indie", Description = "Інді" });

        Assert.Contains(context.Categories, c => c.Name == "Indie");
    }

    [Fact]
    public void UpdateCategory_ChangesFields()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new CategoryRepository(context);
        var category = new Category { Name = "Temp", Description = "d" };
        repo.AddCategory(category);

        category.Name = "Updated";
        repo.UpdateCategory(category);

        Assert.Equal("Updated", context.Categories.Single(c => c.Id == category.Id).Name);
    }

    [Fact]
    public void DeleteCategory_RemovesCategory()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new CategoryRepository(context);
        var category = new Category { Name = "Temp", Description = "d" };
        repo.AddCategory(category);

        repo.DeleteCategory(category);

        Assert.DoesNotContain(context.Categories, c => c.Id == category.Id);
    }

    [Fact]
    public void GetCategory_ReturnsMatchingCategory()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new CategoryRepository(context);

        var found = repo.GetCategory(1); // seeded "Action"

        Assert.NotNull(found);
        Assert.Equal("Action", found!.Name);
    }

    [Fact]
    public void GetCategory_ReturnsNull_WhenMissing()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new CategoryRepository(context);

        Assert.Null(repo.GetCategory(999));
    }

    [Fact]
    public void UpdateAll_UpdatesEveryKnownCategory()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new CategoryRepository(context);

        repo.UpdateAll(new[]
        {
            new Category { Id = 1, Name = "Action+", Description = "d1" },
            new Category { Id = 2, Name = "RPG+", Description = "d2" }
        });

        Assert.Equal("Action+", context.Categories.Single(c => c.Id == 1).Name);
        Assert.Equal("RPG+", context.Categories.Single(c => c.Id == 2).Name);
        // untouched categories keep their seeded values
        Assert.Equal("Strategy", context.Categories.Single(c => c.Id == 3).Name);
    }
}
