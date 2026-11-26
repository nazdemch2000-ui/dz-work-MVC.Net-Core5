using GameStore.Controllers;
using GameStore.Interface;
using GameStore.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests;

public class OrdersControllerTests
{
    [Fact]
    public void Index_ReturnsAllOrders()
    {
        var orders = new[] { new Order { Id = 1, CustomerName = "A" } };
        var orderMock = new Mock<IOrder>();
        orderMock.Setup(o => o.GetAllOrders()).Returns(orders);
        var controller = new OrdersController(Mock.Of<IProduct>(), orderMock.Object);

        var result = Assert.IsType<ViewResult>(controller.Index());

        Assert.Same(orders, result.Model);
    }

    [Fact]
    public void EditOrder_NewOrder_BuildsLineForEveryProduct()
    {
        var products = new[]
        {
            new Product { Id = 1, Name = "P1" },
            new Product { Id = 2, Name = "P2" }
        };
        var productMock = new Mock<IProduct>();
        productMock.Setup(p => p.GetAllProducts()).Returns(products);
        var controller = new OrdersController(productMock.Object, Mock.Of<IOrder>());

        var result = Assert.IsType<ViewResult>(controller.EditOrder(0));

        var order = Assert.IsType<Order>(result.Model);
        Assert.Equal(0, order.Id);
        IEnumerable<OrderLine> lines = controller.ViewBag.Lines;
        var lineList = lines.ToList();
        Assert.Equal(2, lineList.Count);
        Assert.All(lineList, l => Assert.Equal(0, l.Quantity));
    }

    [Fact]
    public void EditOrder_ExistingOrder_MergesExistingQuantities()
    {
        var products = new[]
        {
            new Product { Id = 1, Name = "P1" },
            new Product { Id = 2, Name = "P2" }
        };
        var existing = new Order
        {
            Id = 10,
            Lines = new List<OrderLine> { new() { ProductId = 1, Quantity = 4 } }
        };
        var productMock = new Mock<IProduct>();
        productMock.Setup(p => p.GetAllProducts()).Returns(products);
        var orderMock = new Mock<IOrder>();
        orderMock.Setup(o => o.GetOrder(10)).Returns(existing);
        var controller = new OrdersController(productMock.Object, orderMock.Object);

        var result = Assert.IsType<ViewResult>(controller.EditOrder(10));

        IEnumerable<OrderLine> linesEnumerable = controller.ViewBag.Lines;
        var lines = linesEnumerable.ToList();
        Assert.Equal(4, lines.Single(l => l.ProductId == 1).Quantity);
        Assert.Equal(0, lines.Single(l => l.ProductId == 2).Quantity);
    }

    [Fact]
    public void AddOrUpdateOrder_AddsOrder_WhenIdZero()
    {
        var orderMock = new Mock<IOrder>();
        var controller = new OrdersController(Mock.Of<IProduct>(), orderMock.Object);
        var order = new Order
        {
            Id = 0,
            Lines = new List<OrderLine> { new() { Id = 0, ProductId = 1, Quantity = 3 } }
        };

        var result = Assert.IsType<RedirectToActionResult>(controller.AddOrUpdateOrder(order));

        orderMock.Verify(o => o.AddOrder(order), Times.Once);
        orderMock.Verify(o => o.UpdateOrder(It.IsAny<Order>()), Times.Never);
        Assert.Equal(nameof(OrdersController.Index), result.ActionName);
    }

    [Fact]
    public void AddOrUpdateOrder_UpdatesOrder_WhenIdNonZero()
    {
        var orderMock = new Mock<IOrder>();
        var controller = new OrdersController(Mock.Of<IProduct>(), orderMock.Object);
        var order = new Order
        {
            Id = 2,
            Lines = new List<OrderLine> { new() { Id = 5, ProductId = 1, Quantity = 1 } }
        };

        controller.AddOrUpdateOrder(order);

        orderMock.Verify(o => o.UpdateOrder(order), Times.Once);
        orderMock.Verify(o => o.AddOrder(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public void AddOrUpdateOrder_DropsZeroQuantityNewLines()
    {
        var orderMock = new Mock<IOrder>();
        var controller = new OrdersController(Mock.Of<IProduct>(), orderMock.Object);
        var order = new Order
        {
            Id = 0,
            Lines = new List<OrderLine>
            {
                new() { Id = 0, ProductId = 1, Quantity = 0 }, // new + empty -> dropped
                new() { Id = 0, ProductId = 2, Quantity = 2 }, // new + filled -> kept
                new() { Id = 7, ProductId = 3, Quantity = 0 }  // existing -> kept
            }
        };

        controller.AddOrUpdateOrder(order);

        Assert.Equal(2, order.Lines.Count());
        Assert.DoesNotContain(order.Lines, l => l.ProductId == 1);
    }

    [Fact]
    public void AddOrUpdateOrder_HandlesNullLines()
    {
        var orderMock = new Mock<IOrder>();
        var controller = new OrdersController(Mock.Of<IProduct>(), orderMock.Object);
        var order = new Order { Id = 0, Lines = null! };

        var exception = Record.Exception(() => controller.AddOrUpdateOrder(order));

        Assert.Null(exception); // regression guard: null Lines must not throw
        Assert.Empty(order.Lines);
        orderMock.Verify(o => o.AddOrder(order), Times.Once);
    }

    [Fact]
    public void DeleteOrder_RemovesOrder_AndRedirects()
    {
        var orderMock = new Mock<IOrder>();
        var controller = new OrdersController(Mock.Of<IProduct>(), orderMock.Object);
        var order = new Order { Id = 9 };

        var result = Assert.IsType<RedirectToActionResult>(controller.DeleteOrder(order));

        orderMock.Verify(o => o.DeleteOrder(order), Times.Once);
        Assert.Equal(nameof(OrdersController.Index), result.ActionName);
    }
}
