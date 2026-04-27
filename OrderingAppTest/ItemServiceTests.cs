using OrderingWebAPI.Services;

namespace OrderingAppTest
{
    public class ItemServiceTests
    {
        [Fact]
        public async Task GetItemsAsync_ReturnsItemsList()
        {
            // Arrange
            var service = new ItemService();

            // Act
            var items = await service.GetItemsAsync();

            // Assert
            Assert.NotNull(items);
            var list = items.ToList();
            Assert.Equal(3, list.Count);
            Assert.Contains(list, i => i.Id == 1 && i.Name == "Laptop");
            Assert.Contains(list, i => i.Id == 2 && i.Name == "Phone");
            Assert.Contains(list, i => i.Id == 3 && i.Name == "Headphones");
        }
    }
}