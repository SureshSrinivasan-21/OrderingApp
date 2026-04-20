using OrderingModel;

namespace OrderingViewModel.Services.Interfaces
{
    public interface IItemService
    {
        Task<IReadOnlyList<Items>> GetItemsAsync(CancellationToken cancellationToken = default);
    }
}
