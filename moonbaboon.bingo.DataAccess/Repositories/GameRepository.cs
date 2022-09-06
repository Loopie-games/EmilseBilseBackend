using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class GameRepository: IGameRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);
        
        private const string Table = DBStrings.GameTable;

        private static string sql_select(string from)
        {
            return
                $"SELECT {DBStrings.GameTable}.{DBStrings.Id}, " +
                $"{DBStrings.UserTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname}, {DBStrings.UserTable}.{DBStrings.ProfilePic}, " +
                $"{DBStrings.GameTable}.{DBStrings.WinnerId} " +
                $"FROM `{from}` " +
                $"JOIN {DBStrings.UserTable} ON {DBStrings.UserTable}.{DBStrings.Id} = {DBStrings.GameTable}.{DBStrings.HostId} ";
        }
        
        private static Game ReaderToGame(MySqlDataReader reader)
        {
            var host = new UserSimple(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
            Game ent = new(host)
            {
                Id = reader.GetValue(0).ToString(),
                WinnerId = reader.GetValue(5).ToString(),
            };
            return ent;
        }

        public async Task<Game> FindById(string id)
        {   
            Game? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                sql_select(Table) +
                $"WHERE {DBStrings.GameTable}.{DBStrings.Id} = '{id}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToGame(reader);
            }
            await _connection.CloseAsync();
            return ent ?? throw new Exception("No game found with id: " + id);
        }

        public async Task<Game?> FindByHostId(string userId)
        {
            Game? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.GameTable}.{DBStrings.Id}, " +
                $"{DBStrings.UserTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname}, {DBStrings.UserTable}.{DBStrings.ProfilePic}, " +
                $"{DBStrings.GameTable}.{DBStrings.WinnerId} " +
                $"FROM `{DBStrings.GameTable}` " +
                $"JOIN {DBStrings.UserTable} ON {DBStrings.UserTable}.{DBStrings.Id} = {DBStrings.GameTable}.{DBStrings.HostId} " +
                $"WHERE {DBStrings.GameTable}.{DBStrings.HostId} = '{userId}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var host = new UserSimple(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                    reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
                ent = new Game(host)
                {
                    Id = reader.GetValue(0).ToString(),
                    WinnerId = reader.GetValue(5).ToString(),
                };
            }
            await _connection.CloseAsync();
            return ent;
        }

        public async Task<Game> Create(string hostId)
        {
            string uuid = Guid.NewGuid().ToString();
            Game? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO `{DBStrings.GameTable}`(`{DBStrings.Id}`, `{DBStrings.HostId}`) " +
                $"VALUES ('{uuid}','{hostId}'); " +
                sql_select(Table) +
                $"WHERE {DBStrings.GameTable}.{DBStrings.Id} = '{uuid}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {

                ent = ReaderToGame(reader);
            }
            
            await _connection.CloseAsync();

            if (ent == null)
            {
                throw new InvalidDataException($"ERROR in creating game with host: " + hostId);
            }
            return ent;
        }

        public async Task<List<UserSimple>> GetPlayers(string gameId)
        {
            var list = new List<UserSimple>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT User.id, User.username, User.nickname, User.ProfilePicURL " +
                $"From (SELECT Board.UserId as u1 FROM `Game` " +
                $"JOIN Board ON Board.GameId = Game.Id " +
                $"WHERE Game.Id = '{gameId}') As b " +
                $"JOIN User ON b.u1 = User.id", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = new UserSimple(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(),
                    reader.GetValue(2).ToString(), reader.GetValue(3).ToString());
                list.Add(ent);
            }
            await _connection.CloseAsync();
            return list;
        }

        public async Task<bool> Delete(string gameId)
        {
            var b = false;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"DELETE FROM `{DBStrings.GameTable}` " +
                $"WHERE `{DBStrings.Id}`='{gameId}'; " +
                $"SELECT ROW_COUNT()",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                b = (Convert.ToInt16(reader.GetValue(0).ToString())>0);
            }
            
            await _connection.CloseAsync();
            return b;
        }

        public async Task<Game> Update(Game game)
        {
            Game? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"UPDATE {Table} SET {Table}.{DBStrings.HostId}='{game.Host.Id}',{Table}.{DBStrings.WinnerId}='{game.WinnerId}' WHERE {Table}.{DBStrings.Id} = '{game.Id}'; " +
                sql_select(Table) +
                $"WHERE {Table}.{DBStrings.Id} = '{game.Id}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToGame(reader);
            }
            
            await _connection.CloseAsync();

            if (ent == null)
            {
                throw new InvalidDataException($"ERROR in updating game with id: " + game.Id);
            }
            return ent;
        }
    }
}