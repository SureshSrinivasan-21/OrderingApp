using Moq;
using OrderingModel;
using OrderingViewModel;
using OrderingViewModel.Services.Interfaces;

namespace OrderingAppTest
{
    public class MainWindowViewModelTests
    {
        private static IReadOnlyList<Items> SampleItems() =>
            new List<Items>
            {
                new Items { Id = 1, Name = "Laptop" },
                new Items { Id = 2, Name = "Phone" },
                new Items { Id = 3, Name = "Headphones" }
            };

        private static async Task<bool> WaitForAsync(Func<bool> condition, int timeoutMs = 3000, int pollMs = 50)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (condition()) return true;
                await Task.Delay(pollMs);
            }
            return false;
        }

        [Fact]
        public async Task LoadItems_PopulatesItems_OnSuccess()
        {
            var mockItemService = new Mock<IItemService>();
            mockItemService.Setup(s => s.GetItemsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(SampleItems());

            var mockOrderService = new Mock<IOrderService>();

            var vm = new MainWindowViewModel(mockItemService.Object, mockOrderService.Object);

            var loaded = await WaitForAsync(() => vm.Items.Count == 3);
            Assert.True(loaded, "Items were not loaded in time.");
            Assert.Equal(3, vm.Items.Count);
            Assert.False(vm.IsLoading);
            Assert.Equal(string.Empty, vm.ErrorMessage);
        }

        [Fact]
        public async Task LoadItems_SetsErrorMessage_OnException()
        {
            var mockItemService = new Mock<IItemService>();
            mockItemService.Setup(s => s.GetItemsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("boom"));

            var mockOrderService = new Mock<IOrderService>();

            var vm = new MainWindowViewModel(mockItemService.Object, mockOrderService.Object);

            var finished = await WaitForAsync(() => vm.IsLoading == false);
            Assert.True(finished, "Load did not finish in time.");
            Assert.Contains("Failed to load items", vm.ErrorMessage);
            Assert.Empty(vm.Items);
        }

        [Fact]
        public async Task Validation_PreventsPlaceOrder_WhenFieldsInvalid()
        {
            var mockItemService = new Mock<IItemService>();
            mockItemService.Setup(s => s.GetItemsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(SampleItems());

            var mockOrderService = new Mock<IOrderService>();

            var vm = new MainWindowViewModel(mockItemService.Object, mockOrderService.Object);
            await WaitForAsync(() => vm.Items.Count > 0);

            vm.SelectedItem = vm.Items.First();
            vm.QuantityText = "not-a-number"; // invalid
            vm.City = "X";
            vm.SelectedState = "KA";

            // Setting invalid quantity triggers validation
            Assert.True(vm.HasErrors);
            Assert.False(vm.PlaceOrderCommand.CanExecute(null));
        }

        [Fact]
        public async Task PlaceOrder_Success_SetsConfirmationMessage()
        {
            var mockItemService = new Mock<IItemService>();
            mockItemService.Setup(s => s.GetItemsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(SampleItems());

            var expectedResponse = new ItemResponse
            {
                Success = true,
                ConfirmationNumber = "CONF123456",
                Message = "ok"
            };

            var mockOrderService = new Mock<IOrderService>();
            mockOrderService.Setup(s => s.SubmitOrderAsync(It.IsAny<OrderRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var vm = new MainWindowViewModel(mockItemService.Object, mockOrderService.Object);
            await WaitForAsync(() => vm.Items.Count > 0);

            vm.SelectedItem = vm.Items.First();
            vm.QuantityText = "2";
            vm.City = "Seattle";
            vm.SelectedState = "KA";

            Assert.True(vm.PlaceOrderCommand.CanExecute(null));
            vm.PlaceOrderCommand.Execute(null);

            var finished = await WaitForAsync(() => vm.IsSubmitting == false);
            Assert.True(finished, "Submission did not finish in time.");

            Assert.Contains(expectedResponse.ConfirmationNumber, vm.ConfirmationMessage);
            Assert.Equal(string.Empty, vm.ErrorMessage);
        }

        [Fact]
        public async Task PlaceOrder_Failure_SetsErrorMessage()
        {
            var mockItemService = new Mock<IItemService>();
            mockItemService.Setup(s => s.GetItemsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(SampleItems());

            var expectedResponse = new ItemResponse
            {
                Success = false,
                ConfirmationNumber = string.Empty,
                Message = "out of stock"
            };

            var mockOrderService = new Mock<IOrderService>();
            mockOrderService.Setup(s => s.SubmitOrderAsync(It.IsAny<OrderRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var vm = new MainWindowViewModel(mockItemService.Object, mockOrderService.Object);
            await WaitForAsync(() => vm.Items.Count > 0);

            vm.SelectedItem = vm.Items.First();
            vm.QuantityText = "2";
            vm.City = "Seattle";
            vm.SelectedState = "KA";

            vm.PlaceOrderCommand.Execute(null);

            var finished = await WaitForAsync(() => vm.IsSubmitting == false);
            Assert.True(finished, "Submission did not finish in time.");

            Assert.Equal(expectedResponse.Message, vm.ErrorMessage);
            Assert.Equal(string.Empty, vm.ConfirmationMessage);
        }

        [Fact]
        public async Task Reset_ClearsFieldsAndErrors()
        {
            var mockItemService = new Mock<IItemService>();
            mockItemService.Setup(s => s.GetItemsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(SampleItems());

            var mockOrderService = new Mock<IOrderService>();

            var vm = new MainWindowViewModel(mockItemService.Object, mockOrderService.Object);
            await WaitForAsync(() => vm.Items.Count > 0);

            vm.SelectedItem = vm.Items.First();
            vm.QuantityText = "2";
            vm.City = "Seattle";
            vm.SelectedState = "KA";

            // Introduce an error
            vm.QuantityText = "abc";
            Assert.True(vm.HasErrors);

            vm.ResetCommand.Execute(null);

            Assert.Null(vm.SelectedItem);
            Assert.Equal(string.Empty, vm.QuantityText);
            Assert.Equal(string.Empty, vm.City);
            Assert.Null(vm.SelectedState);
            Assert.Equal(string.Empty, vm.ErrorMessage);
            Assert.Equal(string.Empty, vm.ConfirmationMessage);
        }
    }
}