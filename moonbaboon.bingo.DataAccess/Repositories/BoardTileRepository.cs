using System;
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
    }
}