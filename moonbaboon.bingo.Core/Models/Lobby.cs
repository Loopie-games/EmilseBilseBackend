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
        public LobbyForUser(string id, string hostNickname, string pin)
        {
            Id = id;
            HostNickname = hostNickname;
            Pin = pin;
        }

        public string Id { get; set; }

        public string HostNickname { get; set; }
        
        public string Pin { get; set; }
    }
}