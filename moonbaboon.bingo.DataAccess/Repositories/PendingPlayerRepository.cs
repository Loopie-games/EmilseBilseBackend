using System;
using System.IO;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class PendingPlayerRepository :IPendingPlayerRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);
        public async Task<PendingPlayer?> Create(PendingPlayer toCreate)
        {
            string uuid = Guid.NewGuid().ToString();

            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO `{DBStrings.PendingPlayerTable}`(`{DBStrings.Id}`, `{DBStrings.UserId}`, `{DBStrings.LobbyId}`) " +
                $"VALUES ('{uuid}','{toCreate.User}','{toCreate.Lobby.Id}'); " +
                $"SELECT {DBStrings.PendingPlayerTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Id}, {DBStrings.LobbyTable}.* " +
                $"FROM `{DBStrings.PendingPlayerTable}` " +
                $"JOIN {DBStrings.UserTable} ON {DBStrings.UserTable}.{DBStrings.Id} = {DBStrings.PendingPlayerTable}.{DBStrings.UserId} " +
                $"JOIN {DBStrings.LobbyTable} ON {DBStrings.LobbyTable}.{DBStrings.Id} = {DBStrings.PendingPlayerTable}.{DBStrings.LobbyId} " +
                $"WHERE {DBStrings.LobbyTable}.{DBStrings.Id} = '{uuid}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetValue(1).ToString() != toCreate.User) continue;
                toCreate.Id = reader.GetValue(0).ToString();
                var lobby = new Lobby(reader.GetValue(3).ToString())
                {
                    Id = reader.GetValue(2).ToString(),
                    Pin = reader.GetValue(4).ToString()
                };

                toCreate.Lobby = lobby;
            }
            
            await _connection.CloseAsync();

            if (toCreate.Id == null)
            {
                throw new InvalidDataException("ERROR: User not created");
            }
            return toCreate;
        }
    }
}