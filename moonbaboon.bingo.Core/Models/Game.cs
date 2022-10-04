using System;
using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class Game
    {
        public Game(string? id, UserSimple host, UserSimple? winner, State state)
        {
            Id = id;
            Host = host;
            Winner = winner;
            State = state;
        }

        public Game(MySqlDataReader reader)
        {
            Id = reader.GetString("Game_Id");
            Host = new UserSimple(reader.GetString("Host_Id"), reader.GetString("Host_Username"),
                reader.GetString("Host_Nickname"),
                reader.GetValue("Host_ProfilePic").ToString());
            if (!string.IsNullOrEmpty(reader.GetValue("Winner_Id").ToString()))
                Winner = new UserSimple(reader.GetString("Winner_Id"), reader.GetString("Winner_Username"),
                    reader.GetString("Winner_Nickname"),
                    reader.GetValue("Winner_ProfilePic").ToString());
            State = Enum.Parse<State>(reader.GetString("Game_State"));
        }

        public string? Id { get; set; }
        public UserSimple Host { get; set; }
        public UserSimple? Winner { get; set; }

        public State State { get; set; }
    }

    public enum State
    {
        Ongoing,
        Paused,
        Ended
    }
}