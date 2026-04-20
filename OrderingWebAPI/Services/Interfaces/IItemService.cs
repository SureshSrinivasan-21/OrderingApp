using OrderingWebAPI.Models;

namespace OrderingWebAPI.Services.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<ItemDetail>> GetItemsAsync();
    }
}
