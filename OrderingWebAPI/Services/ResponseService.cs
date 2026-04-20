using OrderingWebAPI.Models;
using OrderingWebAPI.Services.Interfaces;

namespace OrderingWebAPI.Services
{
    public class ResponseService : IResponseService
    {
        public Task<ResponseDetails> OnSubmitOrderRespose(RequestDetails requestDetails)
        {
            var response = new ResponseDetails()
            {
                IsSuccess = true,
                ConfirmationId = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant()
            };

            return Task.FromResult(response);
        }
    }
}
