namespace TodoApi.Models
{
    public class TransactionRequest
    {
        public long Id { get; set; }
        public uint Quantity { get; set; } = 1;
    }
}