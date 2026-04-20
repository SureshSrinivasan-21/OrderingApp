using Microsoft.AspNetCore.Mvc;
using OrderingModel;
using OrderingWebAPI.Services.Interfaces;

namespace OrderingWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController : ControllerBase
    {
        private readonly IResponseService _responseService;

        public ResponseController(IResponseService responseService)
        {
            _responseService = responseService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody]OrderRequest requestDetails)
        {
            if (requestDetails == null) 
            {
                return BadRequest("Invalid input");
            }
            if(requestDetails.ItemId <= 0)
            {
                return BadRequest("Invalid request id");
            }
            if (string.IsNullOrEmpty(requestDetails.City ) || string.IsNullOrWhiteSpace(requestDetails.City) || requestDetails.City.Length > 50) 
            {
                return BadRequest("City details needed and it should not exceed 50 characters");
            }
            if (string.IsNullOrEmpty(requestDetails.State) || string.IsNullOrWhiteSpace(requestDetails.State) || requestDetails.State.Length > 2)
            {
                return BadRequest("State details needed and it should not exceed 50 characters");
            }
            if (requestDetails.Quantity < 1 ||  requestDetails.Quantity > 100)
            {
                return BadRequest("Quantity must be between 1 and 100");
            }

            var response = await _responseService.OnSubmitOrderRespose(requestDetails);
            return Ok(response);
        }
    }
}
