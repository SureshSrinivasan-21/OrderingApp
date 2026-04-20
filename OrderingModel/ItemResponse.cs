

namespace OrderingModel
{
    public class ItemResponse
    {
        public bool Success { get; set; }
        public string ConfirmationNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
