using Microsoft.AspNetCore.Mvc;
using OrderingWebAPI.Services.Interfaces;

namespace OrderingWebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var item = await _itemService.GetItemsAsync();
            return Ok(item);
        }

    }
}
