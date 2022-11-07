using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class GameMode
    {
        public GameMode(string? id, string name)
        {
            Id = id;
            Name = name;
        }

        public GameMode(MySqlDataReader reader)
        {
            Id = reader.GetString("GameMode_Id");
            Name = reader.GetString("GameMode_Name");
        }

        public string? Id { get; set; }
        public string Name { get; set; }
    }
}