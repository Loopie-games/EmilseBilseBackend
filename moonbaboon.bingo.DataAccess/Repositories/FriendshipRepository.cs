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
    }
}