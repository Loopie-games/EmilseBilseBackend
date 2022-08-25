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
                $"SELECT BoardTile.Id, BoardTile.Position, BoardTile.IsActivated, " +
                $"Board.id, Board.GameId, Board.UserId, " +
                $"User.id, User.username, User.nickname, User.ProfilePicURL, " +
                $"Tile.Id, Tile.Action " +
                $"FROM {from} " +
                $"JOIN Board ON BoardTile.BoardId = Board.id " +
                $"JOIN User on BoardTile.AboutUserId = User.id " +
                $"JOIN Tile ON BoardTile.TileId = Tile.Id ";
        }
        
        private static BoardTile ReaderToBoardTile(MySqlDataReader reader)
        {
            Board board = new(reader.GetValue(4).ToString(), reader.GetValue(5).ToString())
            {
                Id = reader.GetValue(3).ToString()
            };

            UserSimple user = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
            ITile tile = new Tile(reader.GetValue(10).ToString(), reader.GetValue(11).ToString());
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

        public async Task<BoardTile?> Create(BoardTile toCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            BoardTile? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO `{DBStrings.BoardTileTable}`(`{DBStrings.Id}`, `{DBStrings.BoardId}`, `{DBStrings.TileId}`, `{DBStrings.Position}`, `{DBStrings.IsActivated}`) " +
                $"VALUES ('{uuid}','{toCreate.Board.Id}','{toCreate.Tile.Id}','{toCreate.Position}','{Convert.ToInt32(toCreate.IsActivated)}'); " +
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
                BoardTile boardTile = ReaderToBoardTile(reader);
                list.Add(boardTile);

            }

            await _connection.CloseAsync();
            return list;
        }
    }
}