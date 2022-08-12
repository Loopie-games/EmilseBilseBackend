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
                $"VALUES ('{uuid}','{toCreate.User.Id}','{toCreate.Lobby.Id}'); " +
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

        public async Task<PendingPlayer?> GetByUserId(string userId)
        {
            PendingPlayer? pp = null;
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.PendingPlayerTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.UserId}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname}, {DBStrings.UserTable}.{DBStrings.ProfilePic}, {DBStrings.LobbyTable}.* " +
                $"FROM `{DBStrings.PendingPlayerTable}` " +
                $"JOIN {DBStrings.UserTable} ON {DBStrings.UserTable}.{DBStrings.Id} = {DBStrings.PendingPlayerTable}.{DBStrings.UserId} " +
                $"JOIN {DBStrings.LobbyTable} ON {DBStrings.LobbyTable}.{DBStrings.Id} = {DBStrings.PendingPlayerTable}.{DBStrings.LobbyId} " +
                $"WHERE {DBStrings.PendingPlayerTable}.{DBStrings.UserId} = '{userId}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var user = new UserSimple(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                    reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
                var lobby = new Lobby(reader.GetValue(7).ToString())
                {
                    Id = reader.GetValue(5).ToString(),
                    Pin = reader.GetValue(8).ToString()
                };

                pp = new PendingPlayer(user, lobby)
                {
                    Id = reader.GetValue(0).ToString(),
                };

            }
            
            await _connection.CloseAsync();
            return pp;
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

        public async Task<PendingPlayer?> IsPlayerInLobby(string userId, string lobbyId)
        {
            PendingPlayer? pp = null;
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.PendingPlayerTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname}, {DBStrings.UserTable}.{DBStrings.ProfilePic}, {DBStrings.LobbyTable}.* " +
                $"FROM `{DBStrings.PendingPlayerTable}` " +
                $"JOIN {DBStrings.UserTable} ON {DBStrings.UserTable}.{DBStrings.Id} = {DBStrings.PendingPlayerTable}.{DBStrings.UserId} " +
                $"JOIN {DBStrings.LobbyTable} ON {DBStrings.LobbyTable}.{DBStrings.Id} = {DBStrings.PendingPlayerTable}.{DBStrings.LobbyId} " +
                $"WHERE {DBStrings.PendingPlayerTable}.{DBStrings.LobbyId} = '{lobbyId}' " +
                    $"&& {DBStrings.PendingPlayerTable}.{DBStrings.UserId} = '{userId}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var user = new UserSimple(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                    reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
                var lobby = new Lobby(reader.GetValue(6).ToString())
                {
                    Id = reader.GetValue(5).ToString(),
                    Pin = reader.GetValue(7).ToString()
                };

                pp = new PendingPlayer(user, lobby)
                {
                    Id = reader.GetValue(0).ToString(),
                };
            }
            
            await _connection.CloseAsync();
            return pp;
        }

        public async Task<bool> DeleteWithLobbyId(string lobbyId)
        {
            var b = false;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"DELETE FROM `{DBStrings.PendingPlayerTable}` " +
                $"WHERE `{DBStrings.LobbyId}` = '{lobbyId}'; " +
                $"SELECT ROW_COUNT()",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                b = (Convert.ToInt16(reader.GetValue(0).ToString())>=0);
            }
            
            await _connection.CloseAsync();
            return b;
        }

        public async Task<bool> Delete(string? ppId)
        {
            var b = false;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"DELETE FROM `{DBStrings.PendingPlayerTable}` " +
                $"WHERE `{DBStrings.Id}` = '{ppId}'; " +
                $"SELECT ROW_COUNT()",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                b = (Convert.ToInt16(reader.GetValue(0).ToString())>=0);
            }
            
            await _connection.CloseAsync();
            return b;
        }
    }
}