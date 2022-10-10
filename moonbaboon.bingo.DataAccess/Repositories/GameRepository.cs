using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class GameRepository : IGameRepository
    {
        private const string Table = DbStrings.GameTable;
        private readonly MySqlConnection _connection;

        public GameRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<Game> FindById(string id)
        {
            await using var con = _connection.Clone();
            await con.OpenAsync();

            await using MySqlCommand command = new(@"
SELECT Game.Id AS Game_Id, Game.State AS Game_State, 
       HOST.id AS Host_Id, HOST.username AS Host_Username, HOST.nickname AS Host_Nickname, HOST.ProfilePicURL AS Host_ProfilePic, 
       Winner.id AS Winner_Id, Winner.username AS Winner_Username, Winner.nickname AS Winner_Nickname, Winner.ProfilePicURL AS Winner_ProfilePic 
FROM Game 
    JOIN User AS HOST ON HOST.id = Game.HostId 
    LEFT OUTER JOIN User AS Winner ON Winner.id = Game.WinnerId
    WHERE Game.Id = @GameId",
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
            toCreate.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new("INSERT INTO Game(Id, HostId, WinnerId, State) VALUES (@Id,@HostId, @WinnerId, @State);", con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = toCreate.Id;
                    command.Parameters.Add("@HostId", MySqlDbType.VarChar).Value = toCreate.HostId;
                    command.Parameters.Add("@WinnerId", MySqlDbType.VarChar).Value = toCreate.WinnerId;
                    command.Parameters.AddWithValue("@State", toCreate.State.ToString());
                }
                command.ExecuteNonQuery();
            }
            return toCreate.Id;
        }

        //Todo move to other repo?
        public async Task<List<UserSimple>> GetPlayers(string gameId)
        {
            var list = new List<UserSimple>();
            await using var con = new MySqlConnection(DbStrings.SqlConnection);
            con.Open();

            await using var command = new MySqlCommand(
                "SELECT User.id, User.username, User.nickname, User.ProfilePicURL " +
                "From (SELECT Board.UserId as u1 FROM `Game` " +
                "JOIN Board ON Board.GameId = Game.Id " +
                $"WHERE Game.Id = '{gameId}') As b " +
                "JOIN User ON b.u1 = User.id",
                con);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = new UserSimple(reader.GetValue(0).ToString(), reader.GetString(1),
                    reader.GetString(2), reader.GetValue(3).ToString());
                list.Add(ent);
            }

            return list;
        }

        public async Task<bool> Delete(string gameId)
        {
            var b = false;
            await using var con = new MySqlConnection(DbStrings.SqlConnection);

            con.Open();

            await using var command = new MySqlCommand(
                $"DELETE FROM `{DbStrings.GameTable}` " +
                $"WHERE `{DbStrings.Id}`='{gameId}'; " +
                "SELECT ROW_COUNT()",
                con);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) b = Convert.ToInt16(reader.GetValue(0).ToString()) > 0;

            return b;
        }

        public async Task<Game> Update(Game game)
        {
            var update =
                $"UPDATE {Table} SET {Table}.{DbStrings.HostId}='{game.Host.Id}',";

            if (game.Winner?.Id is not null) update += $"{Table}.{DbStrings.WinnerId}='{game.Winner?.Id}',";

            update += $"{Table}.{DbStrings.State}='{game.State}' " +
                      $"WHERE {Table}.{DbStrings.Id} = '{game.Id}'; ";

            var ent = await GameEnt(update +
                                    sql_select(Table) +
                                    $"WHERE {Table}.{DbStrings.Id} = '{game.Id}'");
            return ent ?? throw new InvalidDataException("ERROR in updating game with id: " + game.Id);
        }

        private static string sql_select(string from)
        {
            return
                $"SELECT {Table}.{DbStrings.Id}, " +
                $"Host.{DbStrings.Id}, Host.{DbStrings.Username}, Host.{DbStrings.Nickname}, Host.{DbStrings.ProfilePic}, " +
                $"Winner.{DbStrings.Id}, Winner.{DbStrings.Username}, Winner.{DbStrings.Nickname}, Winner.{DbStrings.ProfilePic} " +
                $", {Table}.{DbStrings.State} " +
                $"FROM `{from}` " +
                $"JOIN {DbStrings.UserTable} AS Host ON Host.{DbStrings.Id} = {Table}.{DbStrings.HostId} " +
                $"LEFT JOIN {DbStrings.UserTable} AS Winner ON Winner.{DbStrings.Id} = {Table}.{DbStrings.WinnerId} ";
        }

        private static Game ReaderToGame(IDataRecord reader)
        {
            var host = new UserSimple(reader.GetString(1), reader.GetString(2),
                reader.GetString(3), reader.GetValue(4).ToString());
            UserSimple? winner = null;
            if (!string.IsNullOrEmpty(reader.GetValue(5).ToString()))
                winner = new UserSimple(reader.GetValue(5).ToString(), reader.GetString(6),
                    reader.GetString(7), reader.GetValue(8).ToString());

            Game ent = new(reader.GetValue(0).ToString(), host, winner,
                Enum.Parse<State>(reader.GetValue(9).ToString()));

            return ent;
        }

        private static async Task<Game?> GameEnt(string sqlCommand)
        {
            Game? ent = null;
            await using var con = new MySqlConnection(DbStrings.SqlConnection);
            con.Open();

            await using var command = new MySqlCommand(sqlCommand, con);
            await using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) ent = ReaderToGame(reader);

            return ent;
        }
    }
}