using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingModel
{
    public class ItemResponse
    {
        public bool Success { get; set; }
        public string ConfirmationNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
