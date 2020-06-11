using System;

namespace Client.Dtos
{
    public class LogReadDto
    {
        public int Id { get; set; }
        public string Direction { get; set; }
        public string Client { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp {get; set;}
    }
}