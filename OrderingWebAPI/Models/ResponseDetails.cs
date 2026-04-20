namespace OrderingWebAPI.Models
{
    public class ResponseDetails
    {
        public bool IsSuccess { get; set; }

        public required string ConfirmationId { get; set; }
    }
}
