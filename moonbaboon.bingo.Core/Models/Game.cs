using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class Game
    {
        public Game(string? id, User host, User? winner, State state)
        {
            Id = id;
            Host = host;
            Winner = winner;
            State = state;
        }

        public Game(MySqlDataReader reader)
        {
            Id = reader.GetString("Game_Id");
            Host = new User(reader.GetString("Host_Id"), reader.GetString("Host_Username"),
                reader.GetString("Host_Nickname"),
                reader.GetValue("Host_ProfilePic").ToString());
            if (!string.IsNullOrEmpty(reader.GetValue("Winner_Id").ToString()))
                Winner = new User(reader.GetString("Winner_Id"), reader.GetString("Winner_Username"),
                    reader.GetString("Winner_Nickname"),
                    reader.GetValue("Winner_ProfilePic").ToString());
            State = (State) reader.GetInt32("Game_State");
        }

        public string? Id { get; set; }
        public User Host { get; set; }
        public User? Winner { get; set; }

        public State State { get; set; }
    }

    public class GameEntity
    {
        public GameEntity(string? id, string hostId, string? winnerId, State state)
        {
            Id = id;
            HostId = hostId;
            WinnerId = winnerId;
            State = state;
        }

        public string? Id { get; set; }
        public string HostId { get; set; }
        public string? WinnerId { get; set; }

        public State State { get; set; }
    }

    public enum State
    {
        Ongoing,
        Paused,
        Ended
    }
}