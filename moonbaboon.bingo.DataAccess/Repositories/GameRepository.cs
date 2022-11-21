using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly MySqlConnection _connection;

        public GameRepository(MySqlConnection connection)
        {
            _connection = connection.Clone();
        }

        public async Task<Game> FindById(string id)
        {
            await using var con = _connection.Clone();
            await con.OpenAsync();

            await using MySqlCommand command = new(@"
SELECT  Game_Id,  Game_State, 
       HOST.User_id AS Host_Id, HOST.User_Username AS Host_Username, HOST.User_Nickname AS Host_Nickname, HOST.User_ProfilePicURL AS Host_ProfilePic, 
       Winner.User_id AS Winner_Id, Winner.User_Username AS Winner_Username, Winner.User_Nickname AS Winner_Nickname, Winner.User_ProfilePicURL AS Winner_ProfilePic 
FROM Game 
    JOIN User AS HOST ON HOST.User_id = Game.Game_HostId
    LEFT OUTER JOIN User AS Winner ON Winner.User_id = Game.Game_WinnerId
    WHERE Game.Game_Id = @GameId",
                con);
            command.Parameters.Add("@GameId", MySqlDbType.VarChar).Value = id;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return new Game(reader);
            }

            await con.CloseAsync();
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
                        "INSERT INTO Game(Game_Id, Game_HostId, Game_WinnerId, Game_State) VALUES (@Id,@HostId, @WinnerId, @State);",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = toCreate.Id;
                    command.Parameters.Add("@HostId", MySqlDbType.VarChar).Value = toCreate.HostId;
                    command.Parameters.Add("@WinnerId", MySqlDbType.VarChar).Value = toCreate.WinnerId;
                    command.Parameters.Add("@State", MySqlDbType.Int32).Value =  toCreate.State;
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
                @"DELETE FROM Game WHERE Game_Id = @Id; " +
                con);
            {
                command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = gameId;
            }
            command.ExecuteNonQuery();
        }

        public async Task Update(Game entity)
        {
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "UPDATE Game SET `Game_HostId`= @HostId,`Game_WinnerId`= @WinnerId,`Game_State`=@GameState WHERE Game_Id = @Id",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = entity.Id;
                    command.Parameters.Add("@HostId", MySqlDbType.VarChar).Value = entity.Host;
                    command.Parameters.Add("@WinnerId", MySqlDbType.VarChar).Value = entity.Winner;
                    command.Parameters.Add("@GameState", MySqlDbType.Int32).Value = entity.State;
                }
                command.ExecuteNonQuery();
            }
        }
    }
}