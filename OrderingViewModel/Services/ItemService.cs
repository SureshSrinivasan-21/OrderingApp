using OrderingModel;
using OrderingViewModel.Services.Interfaces;
using System.Net.Http.Json;

namespace OrderingViewModel.Services
{
    public class ItemService : IItemService
    {
        private readonly HttpClient _httpClient;

        public ItemService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<Items>> GetItemsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<Items>>("api/items", cancellationToken);
                if (items is { Count: > 0 })
                    return items;
            }
            catch (Exception)
            {

            }

            await Task.Delay(500, cancellationToken);
            return new List<Items>
        {
            new() { Id = 1, Name = "Laptop" },
            new() { Id = 2, Name = "Phone" },
            new() { Id = 3, Name = "Headphones" }
        };
        }
    }
}
