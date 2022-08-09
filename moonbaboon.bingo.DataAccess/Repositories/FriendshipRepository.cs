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
        private readonly MySqlConnection _connection = new(DatabaseStrings.SqLconnection);

        public async Task<List<Friendship>> FindAll()
        {
            var list = new List<Friendship>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT * FROM `{DatabaseStrings.FriendshipTable}` ORDER BY `{DatabaseStrings.Id}`;", _connection);
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
                $"SELECT {DatabaseStrings.FriendshipTable}.{DatabaseStrings.Id}, {DatabaseStrings.UserTable}.{DatabaseStrings.Username}, {DatabaseStrings.UserTable}.{DatabaseStrings.Nickname} " +
                $"FROM {DatabaseStrings.FriendshipTable} " +
                $"INNER JOIN {DatabaseStrings.UserTable} on {DatabaseStrings.UserTable}.{DatabaseStrings.Id} = {DatabaseStrings.FriendshipTable}.{DatabaseStrings.FriendId1} " +
                $"WHERE {DatabaseStrings.FriendshipTable}.{DatabaseStrings.Accepted} = {DatabaseStrings.True} " +
                $"AND {DatabaseStrings.FriendshipTable}.{DatabaseStrings.FriendId2}= '{userId}' " +
                $"UNION SELECT {DatabaseStrings.FriendshipTable}.{DatabaseStrings.Id}, {DatabaseStrings.UserTable}.{DatabaseStrings.Username}, {DatabaseStrings.UserTable}.{DatabaseStrings.Nickname} " +
                $"FROM {DatabaseStrings.FriendshipTable} " +
                $"INNER JOIN {DatabaseStrings.UserTable} ON {DatabaseStrings.UserTable}.{DatabaseStrings.Id} = {DatabaseStrings.FriendshipTable}.{DatabaseStrings.FriendId2} " +
                $"WHERE {DatabaseStrings.FriendshipTable}.{DatabaseStrings.Accepted} = 1 " +
                $"AND {DatabaseStrings.FriendshipTable}.{DatabaseStrings.FriendId1}= 'b674aad7-358b-4204-a00f-d30878bc69a5'", 
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
                $"SELECT {DatabaseStrings.FriendshipTable}.{DatabaseStrings.Accepted} " +
                $"FROM `{DatabaseStrings.FriendshipTable}` " +
                $"WHERE ({DatabaseStrings.FriendshipTable}.{DatabaseStrings.FriendId1} = '{userId1}' && {DatabaseStrings.FriendshipTable}.{DatabaseStrings.FriendId2} = '{userId2}') " +
                $"OR ({DatabaseStrings.FriendshipTable}.{DatabaseStrings.FriendId1} = '{userId2}' && {DatabaseStrings.FriendshipTable}.{DatabaseStrings.FriendId2} = '{userId1}')",
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