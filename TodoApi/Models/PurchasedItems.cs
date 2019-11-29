namespace TodoApi.Models
{
    public class PurchasedItem : TransactionRequest
    {
        public string item_name {get; set; }
        public decimal price { get; set; } 
    }
}