using Azure;
using Azure.Data.Tables;

namespace ABC_MVC.Models
{

    public class CustomerProfile : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        // Required for ITableEntity
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

}
