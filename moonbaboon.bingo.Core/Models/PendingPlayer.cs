namespace moonbaboon.bingo.Core.Models
{
    public class PendingPlayer
    {
        public PendingPlayer(UserSimple user, Lobby lobby)
        {
            User = user;
            Lobby = lobby;
        }

        public string? Id { get; set; }
        public UserSimple User { get; set; }
        public Lobby Lobby { get; set; }
        
    }
}