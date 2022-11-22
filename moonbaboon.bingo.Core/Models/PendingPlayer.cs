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
            IsHost = Lobby.Host == User.Id;
        }

        public string? Id { get; set; }
        public User User { get; set; }
        public Lobby Lobby { get; set; }
        public bool IsHost { get; }
    }

    public class PendingPlayerEntity
    {
        public PendingPlayerEntity(string? id, string user, string lobby)
        {
            Id = id;
            User = user;
            Lobby = lobby;
        }

        public string? Id { get; set; }
        public string User { get; set; }
        public string Lobby { get; set; }
    }
}