using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BoardTileRepository : IBoardTileRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);

        public async Task<BoardTile?> FindById(string id)
        {
            BoardTile? ent = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT {DBStrings.BoardTileTable}.{DBStrings.Id}, " +
                $"{DBStrings.BoardTable}.{DBStrings.Id}, {DBStrings.BoardTable}.{DBStrings.GameId}, {DBStrings.BoardTable}.{DBStrings.UserId}, " +
                $"{DBStrings.BoardTileTable}.{DBStrings.TileId}, {DBStrings.BoardTileTable}.{DBStrings.Position}, {DBStrings.BoardTileTable}.{DBStrings.IsActivated} " +
                $"FROM {DBStrings.BoardTileTable} " +
                $"JOIN {DBStrings.BoardTable} ON {DBStrings.BoardTable}.{DBStrings.Id} = {DBStrings.BoardTileTable}.{DBStrings.BoardId} " +
                $"WHERE {DBStrings.BoardTileTable}.{DBStrings.Id} = '{id}';", 
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Board board = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString())
                {
                    Id = reader.GetValue(1).ToString()
                };
                ent = new BoardTile(board, reader.GetValue(4).ToString(), Int32.Parse( reader.GetValue(5).ToString()),
                    Boolean.Parse(reader.GetValue(6).ToString()))
                {
                    Id = reader.GetValue(0).ToString()
                };

            }

            await _connection.CloseAsync();
            return ent;        
        }

        public async Task<BoardTile?> Create(BoardTile toCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            BoardTile? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO `{DBStrings.BoardTileTable}`(`{DBStrings.Id}`, `{DBStrings.BoardId}`, `{DBStrings.TileId}`, `{DBStrings.Position}`, `{DBStrings.IsActivated}`) " +
                $"VALUES ('{uuid}','{toCreate.Board.Id}','{toCreate.TileId}','{toCreate.Position}','{Convert.ToInt32(toCreate.IsActivated)}'); " +
                $"SELECT {DBStrings.BoardTileTable}.{DBStrings.Id}, " +
                $"{DBStrings.BoardTable}.{DBStrings.Id}, {DBStrings.BoardTable}.{DBStrings.GameId}, {DBStrings.BoardTable}.{DBStrings.UserId}, " +
                $"{DBStrings.BoardTileTable}.{DBStrings.TileId}, {DBStrings.BoardTileTable}.{DBStrings.Position}, {DBStrings.BoardTileTable}.{DBStrings.IsActivated} " +
                $"FROM {DBStrings.BoardTileTable} " +
                $"JOIN {DBStrings.BoardTable} ON {DBStrings.BoardTable}.{DBStrings.Id} = {DBStrings.BoardTileTable}.{DBStrings.BoardId} " +
                $"WHERE {DBStrings.BoardTileTable}.{DBStrings.Id} = '{uuid}';", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                
                Board board = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString())
                {
                    Id = reader.GetValue(1).ToString()
                };
                ent = new BoardTile(board, reader.GetValue(4).ToString(), Int32.Parse( reader.GetValue(5).ToString()),
                    Boolean.Parse(reader.GetValue(6).ToString()))
                {
                    Id = reader.GetValue(0).ToString()
                };
            }
            
            await _connection.CloseAsync();

            if (ent == null)
            {
                throw new InvalidDataException($"ERROR: {nameof(BoardTile)} not created");
            }
            return ent;
        }

        public async Task<List<BoardTile>> FindByBoardId(string id)
        {
            List<BoardTile> list = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT {DBStrings.BoardTileTable}.{DBStrings.Id}, " +
                $"{DBStrings.BoardTable}.{DBStrings.Id}, {DBStrings.BoardTable}.{DBStrings.GameId}, {DBStrings.BoardTable}.{DBStrings.UserId}, " +
                $"{DBStrings.BoardTileTable}.{DBStrings.TileId}, {DBStrings.BoardTileTable}.{DBStrings.Position}, {DBStrings.BoardTileTable}.{DBStrings.IsActivated} " +
                $"FROM {DBStrings.BoardTileTable} " +
                $"JOIN {DBStrings.BoardTable} ON {DBStrings.BoardTable}.{DBStrings.Id} = {DBStrings.BoardTileTable}.{DBStrings.BoardId} " +
                $"WHERE {DBStrings.BoardTileTable}.{DBStrings.BoardId} = '{id}';", 
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Board board = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString())
                {
                    Id = reader.GetValue(1).ToString()
                };
                var boardTile = new BoardTile(board, reader.GetValue(4).ToString(), Int32.Parse( reader.GetValue(5).ToString()),
                    Boolean.Parse(reader.GetValue(6).ToString()))
                {
                    Id = reader.GetValue(0).ToString()
                };
                list.Add(boardTile);

            }

            await _connection.CloseAsync();
            return list;
        }
    }
}