using System;
using System.Collections.Generic;

namespace TodoApi.Models
{
    public class Receipt
    {
        public string message { get; set; } = "Transaction Complete!";
        public List<PurchasedItem> purchased_items { get; set; } = new List<PurchasedItem>();
        public decimal total_price { get; set; }
        public decimal remaining_balance { get; set; }
    }
}