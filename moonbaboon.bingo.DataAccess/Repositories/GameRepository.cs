using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly MySqlConnection _connection;
        private readonly IDbConnectionFactory _connectionFactory;

        public GameRepository(MySqlConnection connection, IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _connection = connection.Clone();
        }

        public async Task<Game> FindById(string id)
        {
            await using var con = _connection.Clone();
            await con.OpenAsync();

            await using MySqlCommand command = new(@"
SELECT * 
FROM Game 
    JOIN User on User.User_id = Game.Game_HostId
    LEFT OUTER JOIN Board B on Game.Game_WinnerId = B.Board_Id
    WHERE Game.Game_Id = @GameId",
                con);
            command.Parameters.Add("@GameId", MySqlDbType.VarChar).Value = id;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return new Game(reader);

            throw new Exception($"no {nameof(Game)} with id: " + id);
        }

        public async Task<string> Create(GameEntity toCreate)
        {
            await using var con = _connection.Clone();
            {
                toCreate.Id = Guid.NewGuid().ToString();
                con.Open();
                await using MySqlCommand command =
                    new(
                        "INSERT INTO Game(Game_Id, Game_Name, Game_HostId, Game_WinnerId, Game_State) VALUES (@Id,@Name, @HostId, @WinnerId, @State);",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = toCreate.Id;
                    command.Parameters.Add("@Name", MySqlDbType.VarChar).Value = toCreate.Name;
                    command.Parameters.Add("@HostId", MySqlDbType.VarChar).Value = toCreate.HostId;
                    command.Parameters.Add("@WinnerId", MySqlDbType.VarChar).Value = toCreate.WinnerId;
                    command.Parameters.Add("@State", MySqlDbType.Int32).Value = toCreate.State;
                }
                command.ExecuteNonQuery();
            }
            return toCreate.Id;
        }

        public async Task Delete(string gameId)
        {
            await using var con = _connection.Clone();
            con.Open();

            await using var command = new MySqlCommand(
                @"DELETE FROM Game WHERE Game_Id = @Id; ",
                con);
            {
                command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = gameId;
            }
            command.ExecuteNonQuery();
        }

        public async Task Update(GameEntity entity)
        {
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "UPDATE Game SET Game_Name = @Game_Name, `Game_HostId`= @HostId,`Game_WinnerId`= @WinnerId,`Game_State`=@GameState WHERE Game_Id = @Id",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = entity.Id;
                    command.Parameters.Add("@Game_Name", MySqlDbType.VarChar).Value = entity.Name;
                    command.Parameters.Add("@HostId", MySqlDbType.VarChar).Value = entity.HostId;
                    command.Parameters.Add("@WinnerId", MySqlDbType.VarChar).Value = entity.WinnerId;
                    command.Parameters.Add("@GameState", MySqlDbType.Int32).Value = entity.State;
                }
                command.ExecuteNonQuery();
            }
        }

        public List<Game> GetByHostId(string userId)
        {
            var games = new List<Game>();

            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();

            command.CommandText = @"
SELECT  *
FROM Game 
    JOIN User AS HOST ON HOST.User_id = Game.Game_HostId
    LEFT OUTER JOIN Board B on Game.Game_WinnerId = B.Board_Id
WHERE Game_HostId = @HostId";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@HostId";
            parameter.Value = userId;
            command.Parameters.Add(parameter);

            con.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                games.Add(new Game(reader));
            }

            return games;
        }
    }
}