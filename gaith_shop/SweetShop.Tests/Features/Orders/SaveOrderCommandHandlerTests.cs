using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Xunit;
using FluentAssertions;
using SweetShop.Features.Orders.Commands;
using SweetShop.Models.Interfaces;
using SweetShop.Models;
using SweetShop.ViewModels;

namespace SweetShop.Tests.Features.Orders
{
    public class SaveOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly SaveOrderCommandHandler _handler;

        public SaveOrderCommandHandlerTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _productRepoMock = new Mock<IProductRepository>();

            _handler = new SaveOrderCommandHandler(_orderRepoMock.Object, _productRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenNoItemsProvided()
        {
            // Arrange
            var model = new CreateOrderViewModel
            {
                Items = []
            };
            var command = new SaveOrderCommand(model);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("الرجاء إضافة منتج واحد على الأقل");

            _orderRepoMock.Verify(x => x.CreateOrderFromViewModelAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCreateOrderSuccessfully_WhenItemsAreValid()
        {
            // Arrange
            var productId = 1;
            var price = 10.5m;

            var model = new CreateOrderViewModel
            {
                CustomerName = "غيث",
                PhoneNumber = "0790000000",
                Items =
                [
                    new OrderItemViewModel { ProductId = productId, Quantity = 2, ProductName = "كنافة" }
                ]
            };
            var command = new SaveOrderCommand(model);

            _productRepoMock.Setup(repo => repo.GetProductById(productId))
                            .Returns(new Product { Id = productId, Price = price });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.OrderSummary.Should().Contain("غيث");

            _orderRepoMock.Verify(x =>
                x.CreateOrderFromViewModelAsync(It.Is<Order>(o => o.OrderTotal == 21.0m)),
                Times.Once);
        }
    }
}
