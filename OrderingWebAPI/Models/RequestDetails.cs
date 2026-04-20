namespace OrderingWebAPI.Models
{
    public class RequestDetails
    {

        public required int Id { get; set; }
        public int Quantity { get; set; }
        public required string DeliveryCity {  get; set; }
        public required string DeliveryState { get; set; }
    }
}
