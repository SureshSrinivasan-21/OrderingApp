using Microsoft.AspNetCore.Mvc;
using OrderingWebAPI.Models;
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
        public async Task<IActionResult> SubmitOrder([FromBody]RequestDetails requestDetails)
        {
            if (requestDetails == null) 
            {
                return BadRequest("Invalid input");
            }
            if(requestDetails.Id <= 0)
            {
                return BadRequest("Invalid request id");
            }
            if (string.IsNullOrEmpty(requestDetails.DeliveryCity ) || string.IsNullOrWhiteSpace(requestDetails.DeliveryCity) || requestDetails.DeliveryCity.Length > 50) 
            {
                return BadRequest("City details needed and it should not exceed 50 characters");
            }
            if (string.IsNullOrEmpty(requestDetails.DeliveryState) || string.IsNullOrWhiteSpace(requestDetails.DeliveryState) || requestDetails.DeliveryState.Length > 50)
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
