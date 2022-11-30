using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class Game
    {
        public Game(MySqlDataReader reader)
        {
            Id = reader.GetString("Game_Id");
            Name = reader.GetValue("Game_Name").ToString();
            Host = new User(reader);
            State = (State) reader.GetInt32("Game_State");
            if (!string.IsNullOrEmpty(reader.GetValue("Board_Id").ToString()))
                WinnerId = reader.GetString(reader.GetOrdinal("Board_Id"));
        }

        public Game(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("Game_Id"));
            Name = reader.GetValue(reader.GetOrdinal("Game_Name")).ToString();
            Host = new User(reader);
            State = (State) reader.GetInt32(reader.GetOrdinal("Game_State"));
            if (!string.IsNullOrEmpty(reader.GetValue(reader.GetOrdinal("Board_Id")).ToString()))
                WinnerId = reader.GetString(reader.GetOrdinal("Board_Id"));
        }

        public string Id { get; set; }
        public string? Name { get; set; }
        public User Host { get; set; }
        public string? WinnerId { get; set; }

        public State State { get; set; }
    }

    public class GameEntity
    {
        public GameEntity(string? id, string name, string hostId, string? winnerId, State state)
        {
            Id = id;
            HostId = hostId;
            WinnerId = winnerId;
            State = state;
            Name = name;
        }

        public GameEntity(Game game)
        {
            Id = game.Id;
            HostId = game.Host.Id;
            WinnerId = game.WinnerId;
            State = game.State;
            Name = game.Name;
        }

        public string? Id { get; set; }
        public string? Name { get; set; }
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