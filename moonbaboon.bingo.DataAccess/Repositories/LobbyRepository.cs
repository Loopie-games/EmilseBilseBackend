using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class LobbyRepository : ILobbyRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);
        private static readonly Random Random = new();

        public async Task<Lobby?> Create(Lobby lobbyToCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var pin = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
            
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO `{DBStrings.LobbyTable}`(`{DBStrings.Id}`, `{DBStrings.Host}`, `{DBStrings.Pin}`) " +
                $"VALUES ('{uuid}','{lobbyToCreate.Host}','{pin}'); " +
                $"SELECT * FROM {DBStrings.LobbyTable} WHERE {DBStrings.LobbyTable}.{DBStrings.Id} = '{uuid}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetValue(1).ToString() != lobbyToCreate.Host) continue;
                lobbyToCreate.Id = reader.GetValue(0).ToString();
                lobbyToCreate.Pin = reader.GetValue(2).ToString();
            }
            
            await _connection.CloseAsync();

            if (lobbyToCreate.Id == null)
            {
                throw new InvalidDataException($"ERROR: {nameof(Lobby)} not created");
            }
            return lobbyToCreate;
        }

        public async Task<LobbyForUser?> FindById_ForUser(string id)
        {
            LobbyForUser? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.LobbyTable}.{DBStrings.Id}, hostId.{DBStrings.Nickname}, {DBStrings.LobbyTable}.{DBStrings.Pin} " +
                $"FROM {DBStrings.LobbyTable} " +
                $"JOIN {DBStrings.UserTable} as hostId " +
                $"ON hostId.{DBStrings.Id} = {DBStrings.LobbyTable}.{DBStrings.Host} " +
                $"WHERE {DBStrings.LobbyTable}.{DBStrings.Id} = '{id}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.HasRows)
                {
                    ent = new LobbyForUser(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(),  reader.GetValue(2).ToString());
                }
            }
            await _connection.CloseAsync();
            return ent;
        }

        public async Task<Lobby?> FindById(string id)
        {
            Lobby? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT * FROM {DBStrings.LobbyTable} " +
                $"WHERE {DBStrings.LobbyTable}.{DBStrings.Id} = '{id}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.HasRows)
                {
                    ent = new Lobby(reader.GetValue(1).ToString())
                    {
                        Id = reader.GetValue(0).ToString(),
                        Pin = reader.GetValue(2).ToString()
                    };
                }
            }
            await _connection.CloseAsync();
            return ent;
        }

        public async Task<Lobby?> FindByHostId(string hostId)
        {
            Lobby? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT * FROM {DBStrings.LobbyTable} " +
                $"WHERE {DBStrings.LobbyTable}.{DBStrings.Host} = '{hostId}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.HasRows)
                {
                    ent = new Lobby(reader.GetValue(1).ToString())
                    {
                        Id = reader.GetValue(0).ToString(),
                        Pin = reader.GetValue(2).ToString()
                    };
                }
            }
            await _connection.CloseAsync();
            return ent;
        }

        /// <summary>
        /// Finds the Lobby corresponding to the given Pin
        /// </summary>
        /// <param name="pin">Specific pin for lobby</param>
        /// <returns>Task with the Lobby as Result</returns>
        /// <exception cref="Exception">If No lobby with given Pin Exists</exception>
        public async Task<Lobby> FindByPin(string pin)
        {
            Lobby? ent = null;
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT * FROM {DBStrings.LobbyTable} " +
                $"WHERE {DBStrings.LobbyTable}.{DBStrings.Pin} = '{pin}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.HasRows)
                {
                    ent = new Lobby(reader.GetValue(1).ToString())
                    {
                        Id = reader.GetValue(0).ToString(),
                        Pin = reader.GetValue(2).ToString()
                    };
                }
            }
            await _connection.CloseAsync();
            return ent ?? throw new Exception("No Lobby with given Id");
        }

        public async Task<bool> DeleteLobby(string lobbyId)
        {
            bool b = false;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"DELETE FROM `{DBStrings.LobbyTable}` " +
                $"WHERE `{DBStrings.Id}`='{lobbyId}'; " +
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
    }
}