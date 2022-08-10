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

            await using var command = new MySqlCommand($"SELECT * FROM `{DBStrings.FriendshipTable}` ORDER BY `{DBStrings.Id}`;", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = new Friendship(reader.GetValue(1).ToString(),reader.GetValue(2).ToString())
                {
                    Id = reader.GetValue(0).ToString(),
                    Accepted = Convert.ToBoolean(reader.GetValue(3).ToString())
                    
                };
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
    }
    }