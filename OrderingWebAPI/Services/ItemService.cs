using OrderingWebAPI.Models;
using OrderingWebAPI.Services.Interfaces;

namespace OrderingWebAPI.Services
{
    public class ItemService : IItemService
    {

        private readonly List<ItemDetail> _items = new List<ItemDetail>()
        {
            new ItemDetail { Id = 1, Name = "Laptop" },
            new ItemDetail { Id = 2, Name = "Phone" },
            new ItemDetail { Id = 3, Name = "Headphones" }
        };

        public Task<IEnumerable<ItemDetail>> GetItemsAsync()
        {
            return Task.FromResult<IEnumerable<ItemDetail>>(_items);
        }
    }
}
