using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class PendingPlayerRepository : IPendingPlayerRepository
    {
        private const string Table = DbStrings.PendingPlayerTable;
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);

        public async Task<PendingPlayer> Create(PendingPlayer toCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"INSERT INTO `{DbStrings.PendingPlayerTable}`(`{DbStrings.Id}`, `{DbStrings.UserId}`, `{DbStrings.LobbyId}`) " +
                $"VALUES ('{uuid}','{toCreate.User.Id}','{toCreate.Lobby.Id}'); " +
                $"SELECT {DbStrings.PendingPlayerTable}.{DbStrings.Id} " +
                $"FROM `{DbStrings.PendingPlayerTable}` " +
                $"WHERE {DbStrings.PendingPlayerTable}.{DbStrings.Id} = '{uuid}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) toCreate.Id = reader.GetValue(0).ToString();

            await _connection.CloseAsync();
            if (toCreate.Id == null) throw new Exception($"ERROR: {nameof(PendingPlayer)} not created");

            return toCreate;
        }

        public async Task<PendingPlayer> GetByUserId(string userId)
        {
            PendingPlayer? pp = null;
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.PendingPlayerTable}.{DbStrings.Id}, {DbStrings.UserTable}.{DbStrings.Id}, {DbStrings.UserTable}.{DbStrings.Username}, {DbStrings.UserTable}.{DbStrings.Nickname}, {DbStrings.UserTable}.{DbStrings.ProfilePic}, {DbStrings.LobbyTable}.* " +
                $"FROM `{DbStrings.PendingPlayerTable}` " +
                $"JOIN {DbStrings.UserTable} ON {DbStrings.UserTable}.{DbStrings.Id} = {DbStrings.PendingPlayerTable}.{DbStrings.UserId} " +
                $"JOIN {DbStrings.LobbyTable} ON {DbStrings.LobbyTable}.{DbStrings.Id} = {DbStrings.PendingPlayerTable}.{DbStrings.LobbyId} " +
                $"WHERE {DbStrings.PendingPlayerTable}.{DbStrings.UserId} = '{userId}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) pp = ReaderToEnt(reader);

            await _connection.CloseAsync();
            return pp ?? throw new Exception($"no {Table} found with User-id: {userId}");
        }

        public async Task<List<PendingPlayer>> GetByLobbyId(string lobbyId)
        {
            List<PendingPlayer> list = new();

            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.PendingPlayerTable}.{DbStrings.Id}," +
                $"{DbStrings.UserTable}.{DbStrings.Id}, {DbStrings.UserTable}.{DbStrings.Username}, {DbStrings.UserTable}.{DbStrings.Nickname}, {DbStrings.UserTable}.{DbStrings.ProfilePic}, " +
                $"{DbStrings.LobbyTable}.{DbStrings.Id}, {DbStrings.LobbyTable}.{DbStrings.Host}, {DbStrings.LobbyTable}.{DbStrings.Pin}  " +
                $"FROM {DbStrings.PendingPlayerTable} " +
                $"JOIN {DbStrings.UserTable} ON {DbStrings.UserTable}.{DbStrings.Id} = {DbStrings.PendingPlayerTable}.{DbStrings.UserId} " +
                $"JOIN {DbStrings.LobbyTable} ON {DbStrings.LobbyTable}.{DbStrings.Id} = {DbStrings.PendingPlayerTable}.{DbStrings.LobbyId} " +
                $"WHERE {DbStrings.PendingPlayerTable}.{DbStrings.LobbyId} = '{lobbyId}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple player = new(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                    reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
                var lobby = new Lobby(reader.GetValue(5).ToString(), reader.GetValue(6).ToString(),
                    reader.GetValue(7).ToString());


                var ent = new PendingPlayer(player, lobby)
                {
                    Id = reader.GetValue(0).ToString()
                };
                list.Add(ent);
            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<PendingPlayer?> IsPlayerInLobby(string userId)
        {
            PendingPlayer? pp = null;
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.PendingPlayerTable}.{DbStrings.Id}, {DbStrings.UserTable}.{DbStrings.Id}, {DbStrings.UserTable}.{DbStrings.Username}, {DbStrings.UserTable}.{DbStrings.Nickname}, {DbStrings.UserTable}.{DbStrings.ProfilePic}, {DbStrings.LobbyTable}.* " +
                $"FROM `{DbStrings.PendingPlayerTable}` " +
                $"JOIN {DbStrings.UserTable} ON {DbStrings.UserTable}.{DbStrings.Id} = {DbStrings.PendingPlayerTable}.{DbStrings.UserId} " +
                $"JOIN {DbStrings.LobbyTable} ON {DbStrings.LobbyTable}.{DbStrings.Id} = {DbStrings.PendingPlayerTable}.{DbStrings.LobbyId} " +
                $"WHERE {DbStrings.PendingPlayerTable}.{DbStrings.UserId} = '{userId}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var user = new UserSimple(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                    reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
                var lobby = new Lobby(reader.GetValue(5).ToString(), reader.GetValue(6).ToString(),
                    reader.GetValue(7).ToString());


                pp = new PendingPlayer(user, lobby)
                {
                    Id = reader.GetValue(0).ToString()
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
                $"DELETE FROM `{DbStrings.PendingPlayerTable}` " +
                $"WHERE `{DbStrings.LobbyId}` = '{lobbyId}'; " +
                "SELECT ROW_COUNT()",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) b = Convert.ToInt16(reader.GetValue(0).ToString()) >= 0;

            await _connection.CloseAsync();
            return b;
        }

        public async Task<bool> Delete(string? ppId)
        {
            var b = false;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"DELETE FROM `{DbStrings.PendingPlayerTable}` " +
                $"WHERE `{DbStrings.Id}` = '{ppId}'; " +
                "SELECT ROW_COUNT()",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) b = Convert.ToInt16(reader.GetValue(0).ToString()) >= 0;

            await _connection.CloseAsync();
            return b;
        }

        public async Task<PendingPlayer> Update(PendingPlayer toUpdate)
        {
            PendingPlayer? pp = null;
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"UPDATE `PendingPlayer` SET `UserId`='[value-2]',`LobbyId`='[value-3]' WHERE {DbStrings.Id} = '{toUpdate.Id}'" +
                sql_select(Table) +
                $"WHERE {DbStrings.PendingPlayerTable}.{DbStrings.Id} = '{toUpdate.Id}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) pp = ReaderToEnt(reader);

            await _connection.CloseAsync();

            return pp ?? throw new Exception($"ERROR: {nameof(PendingPlayer)} with id: [{toUpdate.Id}] not updated");
        }

        private static string sql_select(string from)
        {
            return
                $"SELECT {DbStrings.PendingPlayerTable}.{DbStrings.Id}, " +
                $"{DbStrings.UserTable}.{DbStrings.Id}, {DbStrings.UserTable}.{DbStrings.Username}, {DbStrings.UserTable}.{DbStrings.Nickname}, {DbStrings.UserTable}.{DbStrings.ProfilePic}, " +
                $"{DbStrings.LobbyTable}.* " +
                $"FROM {from} " +
                $"JOIN {DbStrings.UserTable} ON {DbStrings.UserTable}.{DbStrings.Id} = {DbStrings.PendingPlayerTable}.{DbStrings.UserId} " +
                $"JOIN {DbStrings.LobbyTable} ON {DbStrings.LobbyTable}.{DbStrings.Id} = {DbStrings.PendingPlayerTable}.{DbStrings.LobbyId} ";
        }

        private static PendingPlayer ReaderToEnt(MySqlDataReader reader)
        {
            var user = new UserSimple(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
            var lobby = new Lobby(reader.GetValue(5).ToString(), reader.GetValue(6).ToString(),
                reader.GetValue(7).ToString());

            return new PendingPlayer(user, lobby)
            {
                Id = reader.GetValue(0).ToString()
            };
        }
    }
}