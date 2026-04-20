using OrderingModel;

namespace OrderingWebAPI.Services.Interfaces
{
    public interface IRequestService
    {

        Task<OrderRequest> GetAllRequestsAsync();
    }
}
