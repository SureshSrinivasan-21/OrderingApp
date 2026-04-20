using OrderingWebAPI.Models;

namespace OrderingWebAPI.Services.Interfaces
{
    public interface IRequestService
    {

        Task<RequestDetails> GetAllRequestsAsync();
    }
}
