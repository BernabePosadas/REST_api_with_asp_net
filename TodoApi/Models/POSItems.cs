namespace TodoApi.Models
{
    public class POSItems
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal price { get; set; }
        public uint InStock { get; set; } = 0;
    }
}