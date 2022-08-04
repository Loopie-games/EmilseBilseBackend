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
        //Table
        private const string Table = "Friendship";

        //Rows
        private const string Id = "Id";
        private const string FriendId1 = "FriendId1";
        private const string FriendId2 = "FriendId2";
        private const string Accepted = "Accepted";
        
        //booleans
        private const string False = "0";
        private const string True = "1";
        
        //Usertable
        private const string UserTable = "User";
        private const string UserRowUsername = "Username";
        private const string UserRowNickname = "Nickname";
        

        private readonly MySqlConnection _connection = new MySqlConnection("Server=185.51.76.204; Database=emilse_bilse_bingo; Uid=root; PWD=hemmeligt;");
        
        public async Task<List<Friendship>> FindAll()
        {
            var list = new List<Friendship>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT * FROM `{Table}` ORDER BY `{Id}`;", _connection);
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

        public async Task<List<Friendship>> FindFriendshipsByUserId(string userId)
        {
            var list = new List<Friendship>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT * FROM `{Table}` WHERE `{FriendId1}` = '{True}' OR `{FriendId2}`= '{userId}'", _connection);
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
                $"SELECT {Table}.{Id}, {UserTable}.{UserRowUsername}, {UserTable}.{UserRowNickname} " +
                $"FROM {Table} " +
                $"INNER JOIN {UserTable} on {UserTable}.{Id} = {Table}.{FriendId1} " +
                $"WHERE {Table}.{Accepted} = {True} " +
                $"AND {Table}.{FriendId2}= '{userId}' " +
                $"UNION SELECT {Table}.{Id}, {UserTable}.{UserRowUsername}, {UserTable}.{UserRowNickname} " +
                $"FROM {Table} " +
                $"INNER JOIN {UserTable} ON {UserTable}.{Id} = {Table}.{FriendId2} " +
                $"WHERE {Table}.{Accepted} = 1 " +
                $"AND {Table}.{FriendId1}= 'b674aad7-358b-4204-a00f-d30878bc69a5'", 
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
        }
    }