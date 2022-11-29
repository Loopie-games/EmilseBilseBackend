﻿using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class BoardEntity
    {
        public BoardEntity(string? id, string gameId, string? userId)
        {
            Id = id;
            GameId = gameId;
        }

        public BoardEntity(MySqlDataReader reader)
        {
            Id = reader.GetString("Board_Id");
            GameId = reader.GetString("Board_GameId");
            //TODO turnedtiles
        }

        public BoardEntity(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("Board_Id"));
            GameId = reader.GetString(reader.GetOrdinal("Board_GameId"));
            //Todo turnedtiles
        }

        public string? Id { get; set; }
        public string GameId { get; set; }
        public int TurnedTiles { get; set; }
    }
}