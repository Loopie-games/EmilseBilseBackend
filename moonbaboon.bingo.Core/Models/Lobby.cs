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

    public class LobbyForUser
    {
        public LobbyForUser(string id, string hostUsername, string pin)
        {
            Id = id;
            HostUsername = hostUsername;
            Pin = pin;
        }

        public string Id { get; set; }

        public string HostUsername { get; set; }
        
        public string Pin { get; set; }
    }
}