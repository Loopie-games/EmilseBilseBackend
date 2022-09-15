namespace moonbaboon.bingo.Core.Models
{
    public class PendingPlayer
    {
        public PendingPlayer(UserSimple user, Lobby lobby)
        {
            User = user;
            Lobby = lobby;
            IsHost = lobby.Host == user.Id;
        }

        public string? Id { get; set; }
        public UserSimple User { get; set; }
        public Lobby Lobby { get; set; }
        public bool IsHost { get; }
    }
}