using OrderingModel;

namespace OrderingViewModel.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ItemResponse> SubmitOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);
    }
}
