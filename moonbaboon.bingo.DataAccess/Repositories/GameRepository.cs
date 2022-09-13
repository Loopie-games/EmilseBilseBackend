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
            {
                winner = new UserSimple(reader.GetValue(5).ToString(), reader.GetString(6),
                    reader.GetString(7), reader.GetValue(8).ToString());
            }

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
            while (reader.Read())
            {
                ent = ReaderToGame(reader);
            }

            return ent;
        }

        public async Task<Game> FindById(string id)
        {
            var ent =
                await GameEnt(sql_select(Table) +
                              $"WHERE {Table}.{DbStrings.Id} = '{id}'");
            return ent ?? throw new Exception("No game found with id: " + id);
        }

        public async Task<Game?> FindByHostId(string userId)
        {
            var ent =
                await GameEnt(
                    sql_select(Table) +
                    $"WHERE {Table}.{DbStrings.HostId} = '{userId}'");
            return ent;
        }

        public async Task<Game> Create(string hostId)
        {
            string uuid = Guid.NewGuid().ToString();
            var ent = await GameEnt(
                $"INSERT INTO `{Table}`(`{DbStrings.Id}`, `{DbStrings.HostId}`) " +
                $"VALUES ('{uuid}','{hostId}'); " +
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} = '{uuid}'");
            return ent ?? throw new InvalidDataException($"ERROR in creating game with host: " + hostId);
        }

        //Todo move to other repo?
        public async Task<List<UserSimple>> GetPlayers(string gameId)
        {
            var list = new List<UserSimple>();
            await using var con = new MySqlConnection(DbStrings.SqlConnection);
            con.Open();

            await using var command = new MySqlCommand(
                $"SELECT User.id, User.username, User.nickname, User.ProfilePicURL " +
                $"From (SELECT Board.UserId as u1 FROM `Game` " +
                $"JOIN Board ON Board.GameId = Game.Id " +
                $"WHERE Game.Id = '{gameId}') As b " +
                $"JOIN User ON b.u1 = User.id",
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
                $"SELECT ROW_COUNT()",
                con);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                b = (Convert.ToInt16(reader.GetValue(0).ToString()) > 0);
            }

            return b;
        }

        public async Task<Game> Update(Game game)
        {
            var update =
                $"UPDATE {Table} SET {Table}.{DbStrings.HostId}='{game.Host.Id}',";

            if (game.Winner?.Id is not null)
            {
                update += $"{Table}.{DbStrings.WinnerId}='{game.Winner?.Id}',";
            }

            update += $"{Table}.{DbStrings.State}='{game.State}' " +
                      $"WHERE {Table}.{DbStrings.Id} = '{game.Id}'; ";

            var ent = await GameEnt(update +
                                    sql_select(Table) +
                                    $"WHERE {Table}.{DbStrings.Id} = '{game.Id}'");
            return ent ?? throw new InvalidDataException($"ERROR in updating game with id: " + game.Id);
        }

        public async Task<List<Game>> GetEnded(string userId)
        {
            var list = new List<Game>();
            await using var con = new MySqlConnection(DbStrings.SqlConnection);
            con.Open();

            await using var command = new MySqlCommand(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.HostId} = '{userId}' && {Table}.{DbStrings.State} = '{Enum.GetName(State.Ended)}'",
                con);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = ReaderToGame(reader);
                list.Add(ent);
            }

            return list;
        }
    }
}