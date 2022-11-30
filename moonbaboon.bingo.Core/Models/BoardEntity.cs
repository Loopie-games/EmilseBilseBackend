using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class BoardEntity
    {
        public BoardEntity(string? id, string gameId)
        {
            Id = id;
            GameId = gameId;
        }

        public BoardEntity(MySqlDataReader reader)
        {
            Id = reader.GetString("Board_Id");
            GameId = reader.GetString("Board_GameId");
            
        }

        public BoardEntity(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("Board_Id"));
            GameId = reader.GetString(reader.GetOrdinal("Board_GameId"));
            
        }

        public string? Id { get; set; }
        public string GameId { get; set; }
    }

    public class Board
    {
        public Board(string? id, Game game)
        {
            Id = id;
            Game = game;
        }

        public Board(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("Board_Id"));
            Game = new Game(reader);
        }

        public string? Id { get; set; }
        public Game Game { get; set; }
        
    }
}