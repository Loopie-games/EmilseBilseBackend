using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);

        public async Task<List<Friendship>> FindAll()
        {
            var list = new List<Friendship>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.FriendshipTable}.{DbStrings.Id}, {DbStrings.FriendshipTable}.{DbStrings.Accepted}, " +
                $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic}, " +
                $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                $"FROM {DbStrings.FriendshipTable} " +
                $"JOIN {DbStrings.UserTable} As U1 ON U1.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId1} " +
                $"JOIN {DbStrings.UserTable} As U2 ON U2.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId2}",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());

                var ent = new Friendship(reader.GetValue(0).ToString(), u1, u2,
                    Convert.ToBoolean(reader.GetValue(1).ToString()));
                list.Add(ent);
            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<List<Friendship>> FindFriendshipsByUserId(string userId)
        {
            var list = new List<Friendship>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.FriendshipTable}.{DbStrings.Id}, {DbStrings.FriendshipTable}.{DbStrings.Accepted}, " +
                $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic}, " +
                $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                $"FROM {DbStrings.FriendshipTable} " +
                $"JOIN {DbStrings.UserTable} As U1 ON U1.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId1} " +
                $"JOIN {DbStrings.UserTable} As U2 ON U2.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId2} " +
                $"WHERE {DbStrings.FriendshipTable}.{DbStrings.FriendId1} = '{userId}' OR {DbStrings.FriendshipTable}.{DbStrings.FriendId2} = '{userId}' ",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());

                var ent = new Friendship(reader.GetValue(0).ToString(), u1, u2,
                    Convert.ToBoolean(reader.GetValue(1).ToString()));
                list.Add(ent);
            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<List<Friendship>> FindAcceptedFriendshipsByUserId(string userId)
        {
            var list = new List<Friendship>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.FriendshipTable}.{DbStrings.Id}, {DbStrings.FriendshipTable}.{DbStrings.Accepted}, " +
                $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic}, " +
                $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                $"FROM {DbStrings.FriendshipTable} " +
                $"JOIN {DbStrings.UserTable} As U1 ON U1.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId1} " +
                $"JOIN {DbStrings.UserTable} As U2 ON U2.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId2} " +
                $"WHERE {DbStrings.FriendshipTable}.{DbStrings.Accepted} = 1 " +
                $"AND ({DbStrings.FriendshipTable}.{DbStrings.FriendId1} = '{userId}' OR {DbStrings.FriendshipTable}.{DbStrings.FriendId2} = '{userId}') ",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());

                var ent = new Friendship(reader.GetValue(0).ToString(), u1, u2,
                    Convert.ToBoolean(reader.GetValue(1).ToString()));
                list.Add(ent);
            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<Friendship?> FindFriendshipByUsers(string userId1, string userId2)
        {
            Friendship? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.FriendshipTable}.{DbStrings.Id}, {DbStrings.FriendshipTable}.{DbStrings.Accepted}, " +
                $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic}, " +
                $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                $"FROM {DbStrings.FriendshipTable} " +
                $"WHERE ({DbStrings.FriendshipTable}.{DbStrings.FriendId1} = '{userId1}' && {DbStrings.FriendshipTable}.{DbStrings.FriendId2} = '{userId2}') " +
                $"OR ({DbStrings.FriendshipTable}.{DbStrings.FriendId1} = '{userId2}' && {DbStrings.FriendshipTable}.{DbStrings.FriendId2} = '{userId1}')",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                if (reader.HasRows)
                {
                    UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                        reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                    UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                        reader.GetValue(8).ToString(), reader.GetValue(9).ToString());

                    ent = new Friendship(reader.GetValue(0).ToString(), u1, u2,
                        Convert.ToBoolean(reader.GetValue(1).ToString()));
                }

            await _connection.CloseAsync();
            return ent;
        }

        public async Task<bool> ValidateFriendship(string userId1, string userId2)
        {
            var ent = false;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.FriendshipTable}.{DbStrings.Accepted} " +
                $"FROM `{DbStrings.FriendshipTable}` " +
                $"WHERE ({DbStrings.FriendshipTable}.{DbStrings.FriendId1} = '{userId1}' && {DbStrings.FriendshipTable}.{DbStrings.FriendId2} = '{userId2}') " +
                $"OR ({DbStrings.FriendshipTable}.{DbStrings.FriendId1} = '{userId2}' && {DbStrings.FriendshipTable}.{DbStrings.FriendId2} = '{userId1}')",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                if (reader.HasRows)
                    ent = Convert.ToBoolean(reader.GetValue(0).ToString());
            await _connection.CloseAsync();
            return ent;
        }

        public async Task<Friendship?> Create(string fromUserId, string toUserId, bool b)
        {
            string uuid = Guid.NewGuid().ToString();
            Friendship? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO `{DbStrings.FriendshipTable}`" +
                $"VALUES ('{uuid}','{fromUserId}','{toUserId}','{Convert.ToInt32(b)}'); " +
                $"SELECT {DbStrings.FriendshipTable}.{DbStrings.Id}, {DbStrings.FriendshipTable}.{DbStrings.Accepted}, " +
                $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic}, " +
                $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                $"FROM {DbStrings.FriendshipTable} " +
                $"JOIN {DbStrings.UserTable} As U1 ON U1.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId1} " +
                $"JOIN {DbStrings.UserTable} As U2 ON U2.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId2} " +
                $"WHERE {DbStrings.FriendshipTable}.{DbStrings.Id} = '{uuid}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());

                ent = new Friendship(reader.GetValue(0).ToString(), u1, u2,
                    Convert.ToBoolean(reader.GetValue(1).ToString()));
            }

            await _connection.CloseAsync();
            return ent;
        }

        public async Task<List<Friendship>> FindFriendRequests_ByUserId(string userId)
        {
            var list = new List<Friendship>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.FriendshipTable}.{DbStrings.Id}, {DbStrings.FriendshipTable}.{DbStrings.Accepted}, " +
                $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic}, " +
                $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                $"FROM {DbStrings.FriendshipTable} " +
                $"JOIN {DbStrings.UserTable} As U1 ON U1.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId1} " +
                $"JOIN {DbStrings.UserTable} As U2 ON U2.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId2} " +
                $"WHERE {DbStrings.FriendshipTable}.{DbStrings.FriendId2} = '{userId}' AND {DbStrings.FriendshipTable}.{DbStrings.Accepted} = '0'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());

                var ent = new Friendship(reader.GetValue(0).ToString(), u1, u2,
                    Convert.ToBoolean(reader.GetValue(1).ToString()));
                list.Add(ent);
            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<Friendship?> FindById(string id)
        {
            Friendship? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DbStrings.FriendshipTable}.{DbStrings.Id}, {DbStrings.FriendshipTable}.{DbStrings.Accepted}, " +
                $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic}, " +
                $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                $"FROM {DbStrings.FriendshipTable} " +
                $"JOIN {DbStrings.UserTable} As U1 ON U1.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId1} " +
                $"JOIN {DbStrings.UserTable} As U2 ON U2.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId2} " +
                $"WHERE {DbStrings.FriendshipTable}.{DbStrings.Id} = '{id}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());

                ent = new Friendship(reader.GetValue(0).ToString(), u1, u2,
                    Convert.ToBoolean(reader.GetValue(1).ToString()));
            }

            await _connection.CloseAsync();
            return ent;
        }

        public async Task<Friendship?> AcceptFriendship(string friendshipId)
        {
            Friendship? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"UPDATE {DbStrings.FriendshipTable} " +
                $"SET {DbStrings.FriendshipTable}.{DbStrings.Accepted} ='1' " +
                $"WHERE {DbStrings.FriendshipTable}.{DbStrings.Id} = '{friendshipId}'; " +
                $"SELECT {DbStrings.FriendshipTable}.{DbStrings.Id}, {DbStrings.FriendshipTable}.{DbStrings.Accepted}, " +
                $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic}, " +
                $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                $"FROM {DbStrings.FriendshipTable} " +
                $"JOIN {DbStrings.UserTable} As U1 ON U1.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId1} " +
                $"JOIN {DbStrings.UserTable} As U2 ON U2.{DbStrings.Id} = {DbStrings.FriendshipTable}.{DbStrings.FriendId2} " +
                $"WHERE {DbStrings.FriendshipTable}.{DbStrings.Id} = '{friendshipId}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());

                ent = new Friendship(reader.GetValue(0).ToString(), u1, u2,
                    Convert.ToBoolean(reader.GetValue(1).ToString()));
            }

            await _connection.CloseAsync();
            return ent;
        }

        public async Task<List<Friend>> SearchUsers(string searchStr, string? loggedUserId)
        {
            var list = new List<Friend>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                "SELECT Friendship.Id, Friendship.Accepted, " +
                "User.id, User.username, User.nickname, User.ProfilePicURL " +
                "FROM User " +
                "Left JOIN Friendship " +
                $"ON (Friendship.FriendId1 = '{loggedUserId}' AND Friendship.FriendId2 = User.id) " +
                $"OR (Friendship.FriendId2 = '{loggedUserId}' AND Friendship.FriendId1 = User.id) " +
                $"WHERE User.username LIKE '%{searchStr}%' And User.id != '{loggedUserId}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());

                list.Add(new Friend(reader.GetValue(0).ToString(), u1,
                    bool.TryParse(reader.GetValue(1).ToString(), out var result)));
            }

            await _connection.CloseAsync();
            return list;
        }
    }
}