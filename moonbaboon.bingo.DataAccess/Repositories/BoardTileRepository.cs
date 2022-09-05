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
        private const string Table = DBStrings.BoardTileTable;

        private static string sql_select(string from)
        {
            return
                $"SELECT {Table}.{DBStrings.Id}, {Table}.{DBStrings.Position}, {Table}.{DBStrings.IsActivated}, " +
                $"{DBStrings.BoardTable}.{DBStrings.Id}, {DBStrings.BoardTable}.{DBStrings.GameId}, {DBStrings.BoardTable}.{DBStrings.UserId}, " +
                $"{DBStrings.UserTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname}, {DBStrings.UserTable}.{DBStrings.ProfilePic}, " +
                $"{DBStrings.TileTable}.{DBStrings.Id}, {DBStrings.TileTable}.{DBStrings.Action} " +
                $"FROM {from} " +
                $"JOIN {DBStrings.BoardTable} ON {Table}.{DBStrings.BoardId} = {DBStrings.BoardTable}.{DBStrings.Id} " +
                $"JOIN {DBStrings.UserTable} on {Table}.{DBStrings.AboutUserId} = {DBStrings.UserTable}.{DBStrings.Id} " +
                $"JOIN {DBStrings.TileTable} ON {Table}.{DBStrings.TileId} = {DBStrings.TileTable}.{DBStrings.Id} ";
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
            BoardTile boardTile = new(reader.GetValue(0).ToString(), board, tile, user, Convert.ToInt32(reader.GetValue(1).ToString()), Convert.ToBoolean(reader.GetValue(2).ToString()));

            return boardTile;
        }

        public async Task<BoardTile> FindById(string id)
        {
            BoardTile? ent = null;
            await _connection.OpenAsync();
            await using MySqlCommand command = new(
                sql_select(Table)+
                $"WHERE {Table}.{DBStrings.Id} = '{id}';",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToBoardTile(reader);

            }
            await _connection.CloseAsync();
            return ent ?? throw new Exception($"no {Table} found with id: " + id);        
        }

        public async Task<BoardTile> Create(BoardTile toCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            BoardTile? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO {Table} " +
                $"VALUES ('{uuid}','{toCreate.AboutUser.Id}','{toCreate.Board.Id}','{toCreate.Tile.Id}','{toCreate.Position}','{Convert.ToInt32(toCreate.IsActivated)}'); " +
                sql_select(Table) +
                $"WHERE {Table}.{DBStrings.Id} = '{uuid}';", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {

                ent = ReaderToBoardTile(reader);
            }
            
            await _connection.CloseAsync();

            if (ent == null)
            {
                throw new InvalidDataException($"ERROR: {Table} not created");
            }
            return ent;
        }

        public async Task<List<BoardTile>> FindByBoardId(string id)
        {
            List<BoardTile> list = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE {Table}.{DBStrings.BoardId} = '{id}';", 
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

        public async Task<BoardTile> Update(BoardTile toUpdate)
        {
            BoardTile? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"UPDATE {Table} " +
                $"SET `{DBStrings.AboutUserId}`='{toUpdate.AboutUser.Id}',`{DBStrings.BoardId}`='{toUpdate.Board.Id}',`{DBStrings.TileId}`='{toUpdate.Tile.Id}',`{DBStrings.Position}`='{toUpdate.Position}',`{DBStrings.IsActivated}`='{Convert.ToInt32(toUpdate.IsActivated)}' " +
                $"WHERE {DBStrings.Id} = '{toUpdate.Id}';" +
                sql_select(Table) +
                $"WHERE {Table}.{DBStrings.Id} = '{toUpdate.Id}';", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToBoardTile(reader);
            }
            
            await _connection.CloseAsync();

            if (ent == null)
            {
                throw new InvalidDataException($"ERROR: {Table} with id {toUpdate.Id} not updated");
            }
            return ent;
        }
    }
}