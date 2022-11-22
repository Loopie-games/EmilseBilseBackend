using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class TopPlayerRepository : ITopPlayerRepository
    {
        private readonly MySqlConnection _connection;

        public TopPlayerRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<string> Create(TopPlayerEntity entity)
        {
            string uuid = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            con.Open();
            await using MySqlCommand command = new(
                @"INSERT INTO TopPlayer VALUES (@Id,@GameId,@UserId,@TurnedTiles);", con);
            {
                command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = uuid;
                command.Parameters.Add("@GameId", MySqlDbType.VarChar).Value = entity.GameId;
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = entity.UserId;
                command.Parameters.Add("@TurnedTiles", MySqlDbType.VarChar).Value = entity.TurnedTiles;
            }
            command.ExecuteNonQuery();
            return uuid;
        }

        public async Task<List<TopPlayer>> FindTop(string gameId, int limit)
        {
            List<TopPlayer> list = new();
            await using var con = _connection.Clone();
            con.Open();

            await using var command = new MySqlCommand(
                @"Select * from TopPlayer 
    JOIN User U on U.User_id = TopPlayer.TopPlayer_UserId
    JOIN Game G on G.Game_Id = TopPlayer.TopPlayer_GameId", con);
            await using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) list.Add(new TopPlayer(reader));

            return list;
        }
    }
}