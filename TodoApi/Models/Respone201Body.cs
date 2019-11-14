using System;

namespace TodoApi.Models
{
    public class Response201Body
    {
        public string message { get; set; } = "Operation executed successfully!";
        public string ExceutedBy { get; set; }
        public POSItems data  { get; set; }

        public DateTime timestamp { get; set; }
    }
}