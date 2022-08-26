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

        private static string sql_select(string from)
        {
            return
                $"SELECT {DBStrings.BoardTileTable}.{DBStrings.Id}, {DBStrings.BoardTileTable}.{DBStrings.Position}, {DBStrings.BoardTileTable}.{DBStrings.IsActivated}, " +
                $"{DBStrings.BoardTable}.{DBStrings.Id}, {DBStrings.BoardTable}.{DBStrings.GameId}, {DBStrings.BoardTable}.{DBStrings.UserId}, " +
                $"{DBStrings.UserTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname}, {DBStrings.UserTable}.{DBStrings.ProfilePic}, " +
                $"{DBStrings.TileTable}.{DBStrings.Id}, {DBStrings.TileTable}.{DBStrings.Action} " +
                $"FROM {from} " +
                $"JOIN {DBStrings.BoardTable} ON {DBStrings.BoardTileTable}.{DBStrings.BoardId} = {DBStrings.BoardTable}.{DBStrings.Id} " +
                $"JOIN {DBStrings.UserTable} on {DBStrings.BoardTileTable}.{DBStrings.AboutUserId} = {DBStrings.UserTable}.{DBStrings.Id} " +
                $"JOIN {DBStrings.TileTable} ON {DBStrings.BoardTileTable}.{DBStrings.TileId} = {DBStrings.TileTable}.{DBStrings.Id} ";
        }
        
        private static BoardTile ReaderToBoardTile(MySqlDataReader reader)
        {
            Board board = new(reader.GetValue(4).ToString(), reader.GetValue(5).ToString())
            {
                Id = reader.GetValue(3).ToString()
            };

            UserSimple user = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
            Tile tile = new Tile(reader.GetValue(10).ToString(), reader.GetValue(11).ToString());
            BoardTile boardTile = new(reader.GetValue(0).ToString(), board, tile, user, Convert.ToInt32(reader.GetValue(1).ToString()), bool.TryParse(reader.GetValue(2).ToString(), out var isActivated));

            return boardTile;
        }

        public async Task<BoardTile?> FindById(string id)
        {
            BoardTile? ent = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(DBStrings.BoardTileTable)+
                $"WHERE {DBStrings.BoardTileTable}.{DBStrings.Id} = '{id}';",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToBoardTile(reader);

            }

            await _connection.CloseAsync();
            return ent;        
        }

        public async Task<BoardTile> Create(BoardTile toCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            BoardTile? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO {DBStrings.BoardTileTable} " +
                $"VALUES ('{uuid}','{toCreate.AboutUser.Id}','{toCreate.Board.Id}','{toCreate.Tile.Id}','{toCreate.Position}','{Convert.ToInt32(toCreate.IsActivated)}'); " +
                sql_select(DBStrings.BoardTileTable) +
                $"WHERE {DBStrings.BoardTileTable}.{DBStrings.Id} = '{uuid}';", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {

                ent = ReaderToBoardTile(reader);
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
                sql_select(DBStrings.BoardTileTable) +
                $"WHERE {DBStrings.BoardTileTable}.{DBStrings.BoardId} = '{id}';", 
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                BoardTile boardTile = ReaderToBoardTile(reader);
                list.Add(boardTile);

            }

            await _connection.CloseAsync();
            return list;
        }
    }
}