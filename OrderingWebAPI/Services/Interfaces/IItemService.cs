using OrderingModel;

namespace OrderingWebAPI.Services.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<Items>> GetItemsAsync();
    }
}
