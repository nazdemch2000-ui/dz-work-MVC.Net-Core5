using GameStore.Models;
using GameStore.Reposetory;

namespace GameStore.Tests;

public class OrderRepositoryTests
{
    private static Order NewOrder(string customer = "Ivan") => new()
    {
        CustomerName = customer,
        Address = "Main st.",
        State = "Kyiv",
        ZipCode = "01001",
        Shipped = false,
        Lines = new List<OrderLine>
        {
            new() { ProductId = 1, Quantity = 2 }
        }
    };

    [Fact]
    public void AddOrder_PersistsOrderWithLines()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new OrderRepository(context);

        repo.AddOrder(NewOrder());

        var saved = context.Orders.Single();
        Assert.Equal("Ivan", saved.CustomerName);
        Assert.Single(context.OrderLines);
    }

    [Fact]
    public void GetAllOrders_ReturnsOrdersWithLines()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new OrderRepository(context);
        repo.AddOrder(NewOrder("A"));
        repo.AddOrder(NewOrder("B"));

        var all = repo.GetAllOrders().ToList();

        Assert.Equal(2, all.Count);
        Assert.All(all, o => Assert.NotNull(o.Lines)); // Include(e => e.Lines) populated
    }

    [Fact]
    public void GetOrder_ReturnsMatchingOrder()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new OrderRepository(context);
        var order = NewOrder("Petro");
        repo.AddOrder(order);

        var found = repo.GetOrder(order.Id);

        Assert.NotNull(found);
        Assert.Equal("Petro", found!.CustomerName);
    }

    [Fact]
    public void GetOrder_ReturnsNull_WhenMissing()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new OrderRepository(context);

        Assert.Null(repo.GetOrder(999));
    }

    [Fact]
    public void UpdateOrder_ChangesFields()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new OrderRepository(context);
        var order = NewOrder();
        repo.AddOrder(order);

        order.Shipped = true;
        order.CustomerName = "Changed";
        repo.UpdateOrder(order);

        var updated = context.Orders.Single();
        Assert.True(updated.Shipped);
        Assert.Equal("Changed", updated.CustomerName);
    }

    [Fact]
    public void DeleteOrder_RemovesOrder()
    {
        using var context = TestDbContextFactory.Create();
        var repo = new OrderRepository(context);
        var order = NewOrder();
        repo.AddOrder(order);

        repo.DeleteOrder(order);

        Assert.Empty(context.Orders);
    }
}
