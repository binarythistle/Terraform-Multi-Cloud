using System;

namespace Client.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Direction { get; set; }
        public string Client { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp {get; set;}
    }
}