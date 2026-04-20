using OrderingModel;
using OrderingWebAPI.Services.Interfaces;

namespace OrderingWebAPI.Services
{
    public class ItemService : IItemService
    {

        private readonly List<Items> _items = new List<Items>()
        {
            new Items { Id = 1, Name = "Laptop" },
            new Items { Id = 2, Name = "Phone" },
            new Items { Id = 3, Name = "Headphones" }
        };

        public Task<IEnumerable<Items>> GetItemsAsync()
        {
            return Task.FromResult<IEnumerable<Items>>(_items);
        }
    }
}
