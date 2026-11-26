using GameStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Tests;

/// <summary>
/// Creates a fresh in-memory <see cref="ApplicationContext"/> for each test,
/// isolated by a unique database name so tests never share state.
/// </summary>
public static class TestDbContextFactory
{
    public static ApplicationContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationContext(options);
        context.Database.EnsureCreated(); // applies the seeded categories from OnModelCreating
        return context;
    }
}
