using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly MySqlConnection _connection;

        public FriendshipRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Friend>> FindFriendsByUserId(string userId)
        {
            var list = new List<Friend>();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT * FROM User
                JOIN Friendship ON Friendship.Friendship_IsAccepted = 1 
                                       AND ((Friendship.Friendship_User1Id = @User_Id AND Friendship.Friendship_User2Id = User_id) 
                                            OR (Friendship.Friendship_User1Id = User_id AND Friendship.Friendship_User2Id = @User_Id)) 
                WHERE User.User_id != @User_Id;",
                con);
            {
                command.Parameters.Add("@User_Id", MySqlDbType.VarChar).Value = userId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Friend(reader));
            }
            return list;
        }

        public async Task<bool> ValidateFriendship(string userId1, string userId2)
        {
            await using var con =_connection.Clone();

            await using var command = new MySqlCommand(
                @"SELECT COUNT(Friendship_Id) FROM Friendship 
WHERE ((Friendship_User1Id = @User1 AND Friendship_User2Id = @User2) OR (Friendship_User1Id = @User2 AND Friendship_User2Id = @User1)) AND Friendship_IsAccepted = 1",
                con);
            {
                command.Parameters.Add("@User1", MySqlDbType.VarChar).Value = userId1;
                command.Parameters.Add("@User2", MySqlDbType.VarChar).Value = userId2;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                return reader.GetInt32(0) > 0;
            throw new Exception("ERROR in validating friendship");
        }

        public async Task<string> Create(FriendshipEntity entity)
        {
            string uuid = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"INSERT INTO Friendship VALUES (@Id,@User1,@User2,@IsAccepted); " +
                con);
            {
                command.Parameters.Add("@Admin_Id", MySqlDbType.VarChar).Value = uuid;
                command.Parameters.Add("@Admin_Id", MySqlDbType.VarChar).Value = entity.UserId1;
                command.Parameters.Add("@Admin_Id", MySqlDbType.VarChar).Value = entity.UserId2;
                command.Parameters.Add("@Admin_Id", MySqlDbType.Bool).Value = entity.Accepted;
            }
            command.ExecuteNonQuery();
            return uuid;
        }

        public async Task<List<Friend>> FindFriendRequests_ByUserId(string userId)
        {
            var list = new List<Friend>();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT * FROM User
                JOIN Friendship ON Friendship.Friendship_IsAccepted = 0 
                                       AND ((Friendship.Friendship_User1Id = @User_Id AND Friendship.Friendship_User2Id = User_id) 
                                            OR (Friendship.Friendship_User1Id = User_id AND Friendship.Friendship_User2Id = @User_Id)) 
                WHERE User.User_id != @User_Id;",
                con);
            {
                command.Parameters.Add("@User_Id", MySqlDbType.VarChar).Value = userId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Friend(reader));
            }
            return list;
        }

        public async Task AcceptFriendship(string friendshipId, string acceptingUserId)
        {
            await using var con = _connection.Clone();
            await using var command = new MySqlCommand(
                @"UPDATE Friendship SET Friendship_IsAccepted ='1' WHERE Friendship_Id = @Id AND Friendship_User2Id = @acceptingUser" +
                _connection);
            {
                command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = friendshipId;
                command.Parameters.Add("@acceptingUser", MySqlDbType.VarChar).Value = acceptingUserId;
            }
            command.ExecuteNonQuery();
        }

        public async Task<List<Friend>> SearchUsers(string searchStr, string? loggedUserId)
        {
            var list = new List<Friend>();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT * FROM User
                JOIN Friendship ON Friendship.Friendship_IsAccepted = 1 
                                       AND ((Friendship.Friendship_User1Id = @User_Id AND Friendship.Friendship_User2Id = User_id) 
                                            OR (Friendship.Friendship_User1Id = User_id AND Friendship.Friendship_User2Id = @User_Id)) 
                WHERE User.User_id != @User_Id AND User_Username LIKE @SearchStr",
                con);
            {
                command.Parameters.Add("@User_Id", MySqlDbType.VarChar).Value = loggedUserId;
                command.Parameters.Add("@SearchStr", MySqlDbType.VarChar).Value = searchStr;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Friend(reader));
            }
            return list;
        }
    }
}