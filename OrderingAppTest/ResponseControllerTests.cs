using Moq;
using OrderingWebAPI.Controllers;
using OrderingWebAPI.Services.Interfaces;
using OrderingModel;
using Microsoft.AspNetCore.Mvc;

namespace OrderingAppTest
{
    public class ResponseControllerTests
    {
        private static OrderRequest ValidRequest() => new OrderRequest
        {
            ItemId = 1,
            Quantity = 2,
            City = "Seattle",
            State = "WA"
        };

        [Fact]
        public async Task SubmitOrder_NullRequest_ReturnsBadRequest()
        {
            var mock = new Mock<IResponseService>();
            var controller = new ResponseController(mock.Object);

            var result = await controller.SubmitOrder(null);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input", bad.Value);
        }

        [Fact]
        public async Task SubmitOrder_InvalidItemId_ReturnsBadRequest()
        {
            var mock = new Mock<IResponseService>();
            var controller = new ResponseController(mock.Object);

            var req = ValidRequest();
            req.ItemId = 0;

            var result = await controller.SubmitOrder(req);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request id", bad.Value);
        }

        [Fact]
        public async Task SubmitOrder_InvalidCity_ReturnsBadRequest()
        {
            var mock = new Mock<IResponseService>();
            var controller = new ResponseController(mock.Object);

            var req = ValidRequest();
            req.City = null;

            var result = await controller.SubmitOrder(req);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("City details needed and it should not exceed 50 characters", bad.Value);
        }

        [Fact]
        public async Task SubmitOrder_InvalidState_ReturnsBadRequest()
        {
            var mock = new Mock<IResponseService>();
            var controller = new ResponseController(mock.Object);

            var req = ValidRequest();
            req.State = null;

            var result = await controller.SubmitOrder(req);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("State details needed and it should not exceed 50 characters", bad.Value);
        }

        [Fact]
        public async Task SubmitOrder_InvalidQuantity_ReturnsBadRequest()
        {
            var mock = new Mock<IResponseService>();
            var controller = new ResponseController(mock.Object);

            var req = ValidRequest();
            req.Quantity = 0;

            var result = await controller.SubmitOrder(req);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Quantity must be between 1 and 100", bad.Value);
        }

        [Fact]
        public async Task SubmitOrder_ValidRequest_ReturnsOkWithResponse()
        {
            var expected = new ItemResponse
            {
                Success = true,
                ConfirmationNumber = "CONF123456",
                Message = "ok"
            };

            var mock = new Mock<IResponseService>();
            mock.Setup(s => s.OnSubmitOrderRespose(It.IsAny<OrderRequest>()))
                .ReturnsAsync(expected);

            var controller = new ResponseController(mock.Object);

            var req = ValidRequest();

            var result = await controller.SubmitOrder(req);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ItemResponse>(ok.Value);
            Assert.Equal(expected.Success, value.Success);
            Assert.Equal(expected.ConfirmationNumber, value.ConfirmationNumber);
            Assert.Equal(expected.Message, value.Message);
        }
    }
}