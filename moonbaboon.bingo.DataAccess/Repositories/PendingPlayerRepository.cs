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
        private readonly MySqlConnection _connection;

        public PendingPlayerRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<PendingPlayer> ReadById(string id)
        {
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command =
                    new(
                        @"SELECT * From PendingPlayer 
JOIN Lobby L on L.Lobby_Id = PendingPlayer.PendingPlayer_LobbyId
JOIN User U on U.User_id = PendingPlayer.PendingPlayer_UserId
WHERE PendingPlayer_Id = @Id",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read()) return new PendingPlayer(reader);
            }

            throw new Exception($"No {nameof(BugReport)} with id: {id}");
        }

        public async Task<string> Create(PendingPlayerEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"INSERT INTO PendingPlayer VALUES (@Id,@UserId,@LobbyId); ",
                con);
            {
                command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = entity.Id;
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = entity.User;
                command.Parameters.Add("@LobbyId", MySqlDbType.VarChar).Value = entity.Lobby;
            }
            command.ExecuteNonQuery();
            return entity.Id;
        }

        public async Task<PendingPlayer> GetByUserId(string userId)
        {
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT * FROM PendingPlayer JOIN User U on PendingPlayer.PendingPlayer_UserId = U.User_id WHERE PendingPlayer_UserId = @UserId",
                con);
            {
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = userId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return new PendingPlayer(reader);

            throw new Exception($"no {nameof(User)} found with User-id: {userId}");
        }

        public async Task<List<PendingPlayer>> GetByLobbyId(string lobbyId)
        {
            List<PendingPlayer> list = new();

            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT * FROM PendingPlayer 
                JOIN User U on PendingPlayer.PendingPlayer_UserId = U.User_id 
                JOIN Lobby L on PendingPlayer.PendingPlayer_LobbyId = L.Lobby_Id
                WHERE PendingPlayer_LobbyId = @LobbyId",
                con);
            {
                command.Parameters.Add("@LobbyId", MySqlDbType.VarChar).Value = lobbyId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new PendingPlayer(reader));

            return list;
        }

        public async Task<PendingPlayer?> IsPlayerInLobby(string userId)
        {
            PendingPlayer? pp = null;
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT * FROM PendingPlayer 
    JOIN User U on PendingPlayer.PendingPlayer_UserId = U.User_id 
    JOIN Lobby L on PendingPlayer.PendingPlayer_LobbyId = L.Lobby_Id 
WHERE PendingPlayer_UserId = @UserId",
                con);
            {
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = userId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) pp = new PendingPlayer(reader);

            return pp;
        }

        public async Task DeleteWithLobbyId(string lobbyId)
        {
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"DELETE FROM PendingPlayer WHERE PendingPlayer_LobbyId = @LobbyId; ",
                con);
            {
                command.Parameters.Add("@LobbyId", MySqlDbType.VarChar).Value = lobbyId;
            }
            command.ExecuteNonQuery();
        }

        public async Task Delete(string id)
        {
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"DELETE FROM PendingPlayer WHERE PendingPlayer.PendingPlayer_Id = @Id; ",
                con);
            command.ExecuteNonQuery();
        }

        public async Task Update(PendingPlayer entity)
        {
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"UPDATE PendingPlayer SET PendingPlayer_UserId=@UserId, PendingPlayer_LobbyId=@LobbyId WHERE PendingPlayer_Id = @Id" +
                _connection);
            {
                command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = entity.Id;
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = entity.User.Id;
                command.Parameters.Add("@LobbyId", MySqlDbType.VarChar).Value = entity.Lobby.Id;
            }
            command.ExecuteNonQuery();
        }
    }
}