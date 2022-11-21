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
        private static readonly Random Random = new();
        private readonly MySqlConnection _connection;

        public LobbyRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<string> Create(Lobby entity)
        {
            string uuid = Guid.NewGuid().ToString();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var pin = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[Random.Next(s.Length)]).ToArray());

            await using var con = _connection.Clone();

            await using var command = new MySqlCommand(
                @"INSERT INTO Lobby VALUES (@Id,@Host,@Pin); " +
                con);
            {
                command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = uuid;
                command.Parameters.Add("@Host", MySqlDbType.VarChar).Value = entity.Host;
                command.Parameters.Add("@Pin", MySqlDbType.VarChar).Value = pin;
            }
            command.ExecuteNonQuery();
            return uuid;
        }

        public async Task<Lobby> FindById(string id)
        {
            await using var con =_connection.Clone();

            await using var command = new MySqlCommand(
                @"SELECT * FROM Lobby WHERE Lobby_Id = @Id",
                con);
            {
                command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return new Lobby(reader);
            }

            throw new Exception("ERROR in creating lobby");
        }

        public async Task<Lobby?> FindByHostId(string hostId)
        {
            Lobby? ent = null;
            await using var con = _connection.Clone();

            await using var command = new MySqlCommand(
                @"SELECT * FROM Lobby WHERE Lobby_Host = @Host",
                con);
            {
                command.Parameters.Add("@Host", MySqlDbType.VarChar).Value = hostId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                if (reader.HasRows)
                    ent = new Lobby(reader);
            await _connection.CloseAsync();
            return ent;
        }

        /// <summary>
        ///     Finds the Lobby corresponding to the given Pin
        /// </summary>
        /// <param name="pin">Specific pin for lobby</param>
        /// <returns>Task with the Lobby as Result</returns>
        /// <exception cref="Exception">If No lobby with given Pin Exists</exception>
        public async Task<Lobby> FindByPin(string pin)
        {
            
            await using var con  = _connection.Clone();
            await using var command = new MySqlCommand(
                @"SELECT * FROM Lobby WHERE Lobby_Pin = @Pin",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return new Lobby(reader);
            }
            throw new Exception("No Lobby with given Id");
        }

        public async Task DeleteLobby(string lobbyId)
        {
           await using var con =_connection.Clone();

            await using var command = new MySqlCommand(
                @"DELETE FROM Lobby WHERE Lobby_Id = @LobbyId; " +
                _connection);
            {
                command.Parameters.Add("@LobbyId", MySqlDbType.VarChar).Value = lobbyId;
            }
            await using var reader = await command.ExecuteReaderAsync();
        }
    }
}