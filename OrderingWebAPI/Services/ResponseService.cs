using OrderingModel;
using OrderingWebAPI.Services.Interfaces;

namespace OrderingWebAPI.Services
{
    public class ResponseService : IResponseService
    {
        public Task<ItemResponse> OnSubmitOrderRespose(OrderRequest requestDetails)
        {
            var response = new ItemResponse()
            {
                Success = true,
                ConfirmationNumber = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant()
            };

            return Task.FromResult(response);
        }
    }
}
