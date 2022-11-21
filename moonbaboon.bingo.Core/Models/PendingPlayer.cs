using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class PendingPlayer
    {
        public PendingPlayer(User user, Lobby lobby)
        {
            User = user;
            Lobby = lobby;
            IsHost = lobby.Host == user.Id;
        }

        public PendingPlayer(MySqlDataReader reader)
        {
            Id = reader.GetString("PendingPlayer_Id");
            User = new User(reader);
            Lobby = new Lobby(reader);
        }

        public string? Id { get; set; }
        public User User { get; set; }
        public Lobby Lobby { get; set; }
        public bool IsHost { get; }
    }
}