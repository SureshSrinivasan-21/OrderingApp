using OrderingWebAPI.Models;

namespace OrderingWebAPI.Services.Interfaces
{
    public interface IResponseService
    {
        Task<ResponseDetails> OnSubmitOrderRespose(RequestDetails requestDetails);
    }
}
