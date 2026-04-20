using OrderingModel;

namespace OrderingWebAPI.Services.Interfaces
{
    public interface IResponseService
    {
        Task<ItemResponse> OnSubmitOrderRespose(OrderRequest requestDetails);
    }
}
