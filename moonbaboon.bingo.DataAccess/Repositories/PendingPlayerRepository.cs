using System;
using System.Collections.Generic;
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
                $"SELECT {DBStrings.PendingPlayerTable}.{DBStrings.Id}, {DBStrings.PendingPlayerTable}.{DBStrings.UserId}, {DBStrings.LobbyTable}.* " +
                $"FROM `{DBStrings.PendingPlayerTable}` " +
                $"JOIN {DBStrings.LobbyTable} ON {DBStrings.LobbyTable}.{DBStrings.Id} = {DBStrings.PendingPlayerTable}.{DBStrings.LobbyId} " +
                $"WHERE {DBStrings.PendingPlayerTable}.{DBStrings.Id} = '{uuid}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
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
                throw new InvalidDataException($"ERROR: {nameof(PendingPlayer)} not created");
            }
            return toCreate;
        }

        public async Task<List<PendingPlayerForUser>> GetByLobbyId(string lobbyId)
        {
            List<PendingPlayerForUser> list = new();
            
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.PendingPlayerTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname} " +
                $"FROM {DBStrings.PendingPlayerTable} " +
                $"JOIN {DBStrings.UserTable} ON {DBStrings.UserTable}.{DBStrings.Id} = {DBStrings.PendingPlayerTable}.{DBStrings.UserId} " +
                $"WHERE {DBStrings.PendingPlayerTable}.{DBStrings.LobbyId} = '{lobbyId}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = new PendingPlayerForUser(reader.GetValue(1).ToString(), reader.GetValue(2).ToString())
                    {
                        Id = reader.GetValue(0).ToString()
                    };
                list.Add(ent);
            }
            await _connection.CloseAsync();
            return list;
        }
    }
}