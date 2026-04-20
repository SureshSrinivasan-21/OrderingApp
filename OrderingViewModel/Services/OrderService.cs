using OrderingModel;
using OrderingViewModel.Services.Interfaces;

namespace OrderingViewModel.Services
{
    public class OrderService : IOrderService
    {
        public async Task<ItemResponse> SubmitOrderAsync(OrderRequest request, CancellationToken cancellationToken = default)
        {
            await Task.Delay(800, cancellationToken);

            return new ItemResponse
            {
                Success = true,
                ConfirmationNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(System.Globalization.CultureInfo.InvariantCulture),
                Message = $"Order placed successfully for ItemId {request.ItemId}."
            };
        }
    }
}
