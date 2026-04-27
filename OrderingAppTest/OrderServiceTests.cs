using OrderingViewModel.Services;
using OrderingModel;

namespace OrderingAppTest
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task SubmitOrderAsync_ReturnsSuccessAndConfirmation()
        {
            // Arrange
            var service = new OrderService();
            var request = new OrderRequest
            {
                ItemId = 1,
                Quantity = 2,
                City = "Seattle",
                State = "WA"
            };

            // Act
            var response = await service.SubmitOrderAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.False(string.IsNullOrEmpty(response.ConfirmationNumber));
            Assert.Equal(10, response.ConfirmationNumber.Length);
            Assert.Contains(request.ItemId.ToString(), response.Message);
        }
    }
}