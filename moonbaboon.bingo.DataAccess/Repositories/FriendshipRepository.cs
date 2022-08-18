using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class FriendshipRepository: IFriendshipRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);

        public async Task<List<Friendship>> FindAll()
        {
            var list = new List<Friendship>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.FriendshipTable}.{DBStrings.Id}, {DBStrings.FriendshipTable}.{DBStrings.Accepted}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic}, " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM {DBStrings.FriendshipTable} " +
                $"JOIN {DBStrings.UserTable} As U1 ON U1.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId1} " +
                $"JOIN {DBStrings.UserTable} As U2 ON U2.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId2}",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new UserSimple(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
                
                var ent = new Friendship(reader.GetValue(0).ToString(),u1, u2, Convert.ToBoolean(reader.GetValue(1).ToString()));
                list.Add(ent);
            }
            await _connection.CloseAsync();
            return list;
        }

        public async Task<List<Friend>> FindAcceptedFriendshipsByUserId(string userId)
        {
            var list = new List<Friend>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.FriendshipTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname} " +
                $"FROM {DBStrings.FriendshipTable} " +
                $"INNER JOIN {DBStrings.UserTable} on {DBStrings.UserTable}.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId1} " +
                $"WHERE {DBStrings.FriendshipTable}.{DBStrings.Accepted} = {DBStrings.True} " +
                $"AND {DBStrings.FriendshipTable}.{DBStrings.FriendId2}= '{userId}' " +
                $"UNION SELECT {DBStrings.FriendshipTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname} " +
                $"FROM {DBStrings.FriendshipTable} " +
                $"INNER JOIN {DBStrings.UserTable} ON {DBStrings.UserTable}.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId2} " +
                $"WHERE {DBStrings.FriendshipTable}.{DBStrings.Accepted} = 1 " +
                $"AND ({DBStrings.FriendshipTable}.{DBStrings.FriendId1} = '{userId}' OR {DBStrings.FriendshipTable}.{DBStrings.FriendId1} = '{userId}') ", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = new Friend(reader.GetValue(1).ToString(),reader.GetValue(2).ToString())
                {
                    Id = reader.GetValue(0).ToString(),

                };
                list.Add(ent);
            }
            await _connection.CloseAsync();
            return list;
        }

        public async Task<bool> ValidateFriendship(string userId1, string userId2)
        {
            var ent = false;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.FriendshipTable}.{DBStrings.Accepted} " +
                $"FROM `{DBStrings.FriendshipTable}` " +
                $"WHERE ({DBStrings.FriendshipTable}.{DBStrings.FriendId1} = '{userId1}' && {DBStrings.FriendshipTable}.{DBStrings.FriendId2} = '{userId2}') " +
                $"OR ({DBStrings.FriendshipTable}.{DBStrings.FriendId1} = '{userId2}' && {DBStrings.FriendshipTable}.{DBStrings.FriendId2} = '{userId1}')",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.HasRows)
                {
                    ent = Convert.ToBoolean(reader.GetValue(0).ToString());
                }
            }
            await _connection.CloseAsync();
            return ent;
        }

        public async Task<Friendship?> Create(string fromUserId, string toUserId, bool b)
        {
            string uuid = Guid.NewGuid().ToString();
            Friendship? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO `{DBStrings.FriendshipTable}`" +
                $"VALUES ('{uuid}','{fromUserId}','{toUserId}','{Convert.ToInt32(b)}'); " +
                $"SELECT {DBStrings.FriendshipTable}.{DBStrings.Id}, {DBStrings.FriendshipTable}.{DBStrings.Accepted}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic}, " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM {DBStrings.FriendshipTable} " +
                $"JOIN {DBStrings.UserTable} As U1 ON U1.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId1} " +
                $"JOIN {DBStrings.UserTable} As U2 ON U2.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId2} " +
                $"WHERE {DBStrings.FriendshipTable}.{DBStrings.Id} = '{uuid}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
                
                ent = new Friendship(reader.GetValue(0).ToString(),u1, u2, Convert.ToBoolean(reader.GetValue(1).ToString()));
            }
            await _connection.CloseAsync();
            return ent;
        }

        public async Task<List<Friendship>> FindFriendRequests_ByUserId(string userId)
        {
            var list = new List<Friendship>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.FriendshipTable}.{DBStrings.Id}, {DBStrings.FriendshipTable}.{DBStrings.Accepted}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic}, " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM {DBStrings.FriendshipTable} " +
                $"JOIN {DBStrings.UserTable} As U1 ON U1.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId1} " +
                $"JOIN {DBStrings.UserTable} As U2 ON U2.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId2} " +
                $"WHERE {DBStrings.FriendshipTable}.{DBStrings.FriendId2} = '{userId}' AND {DBStrings.FriendshipTable}.{DBStrings.Accepted} = '0'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new UserSimple(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
                
                var ent = new Friendship(reader.GetValue(0).ToString(),u1, u2, Convert.ToBoolean(reader.GetValue(1).ToString()));
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
                $"SELECT {DBStrings.FriendshipTable}.{DBStrings.Id}, {DBStrings.FriendshipTable}.{DBStrings.Accepted}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic}, " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM {DBStrings.FriendshipTable} " +
                $"JOIN {DBStrings.UserTable} As U1 ON U1.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId1} " +
                $"JOIN {DBStrings.UserTable} As U2 ON U2.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId2} " +
                $"WHERE {DBStrings.FriendshipTable}.{DBStrings.Id} = '{id}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
                
                ent = new Friendship(reader.GetValue(0).ToString(),u1, u2, Convert.ToBoolean(reader.GetValue(1).ToString()));
            }
            await _connection.CloseAsync();
            return ent;
        }

        public async Task<Friendship?> AcceptFriendship(string friendshipId)
        {
            Friendship? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"UPDATE {DBStrings.FriendshipTable} " +
                $"SET {DBStrings.FriendshipTable}.{DBStrings.Accepted} ='1' " +
                $"WHERE {DBStrings.FriendshipTable}.{DBStrings.Id} = '{friendshipId}'; " +
                $"SELECT {DBStrings.FriendshipTable}.{DBStrings.Id}, {DBStrings.FriendshipTable}.{DBStrings.Accepted}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic}, " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM {DBStrings.FriendshipTable} " +
                $"JOIN {DBStrings.UserTable} As U1 ON U1.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId1} " +
                $"JOIN {DBStrings.UserTable} As U2 ON U2.{DBStrings.Id} = {DBStrings.FriendshipTable}.{DBStrings.FriendId2} " +
                $"WHERE {DBStrings.FriendshipTable}.{DBStrings.Id} = '{friendshipId}'",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserSimple u1 = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
                UserSimple u2 = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                    reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
                
                ent = new Friendship(reader.GetValue(0).ToString(),u1, u2, Convert.ToBoolean(reader.GetValue(1).ToString()));
            }
            await _connection.CloseAsync();
            return ent;
        }
    }
    }