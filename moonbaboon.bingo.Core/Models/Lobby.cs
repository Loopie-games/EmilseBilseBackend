namespace moonbaboon.bingo.Core.Models
{
    public class Lobby
    {
        public Lobby(string host)
        {
            Host = host;
        }

        public string? Id { get; set; }

        public string Host { get; set; }
        
        public string? Pin { get; set; }
    }
}