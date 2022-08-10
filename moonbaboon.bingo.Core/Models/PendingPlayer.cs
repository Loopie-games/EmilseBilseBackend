namespace moonbaboon.bingo.Core.Models
{
    public class PendingPlayer
    {
        public PendingPlayer(string user, Lobby lobby)
        {
            User = user;
            Lobby = lobby;
        }

        public string? Id { get; set; }
        public string User { get; set; }
        public Lobby Lobby { get; set; }
        
    }

    public class PendingPlayerForUser
    {
        public string? Id { get; set; }
        public string Username { get; set; }
    }
}