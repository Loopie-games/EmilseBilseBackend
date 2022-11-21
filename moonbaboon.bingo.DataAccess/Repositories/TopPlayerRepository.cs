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
    public class TopPlayerRepository : ITopPlayerRepository
    {
        private const string Table = DbStrings.TopPlayerTable;
        private readonly MySqlConnection _connection;

        public TopPlayerRepository(MySqlConnection connection)
        {
            _connection = connection.Clone();
        }

        public async Task<TopPlayer> Create(TopPlayer toCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            var ent = await Ent(
                $"INSERT INTO `{Table}` " +
                $"VALUES ('{uuid}','{toCreate.GameId}','{toCreate.User.Id}','{toCreate.TurnedTiles}'); " +
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} = '{uuid}'", _connection.Clone());
            return ent ??
                   throw new InvalidDataException($"ERROR in creating {Table} with User: " + toCreate.User.Username);
        }

        public async Task<List<TopPlayer>> FindTop(string gameId, int limit)
        {
            List<TopPlayer> list = new();
            await using var con = _connection.Clone();
            con.Open();

            string sqlCommand =
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.GameId} = '{gameId}' " +
                $"ORDER BY {DbStrings.TurnedTiles} DESC " +
                $"Limit {limit};";

            await using var command = new MySqlCommand(sqlCommand, con);
            await using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var ent = ReaderToEnt(reader);
                list.Add(ent);
            }

            return list;
        }

        private static string sql_select(string from)
        {
            return
                $"SELECT {Table}.{DbStrings.Id}, {Table}.{DbStrings.GameId}, {Table}.{DbStrings.TurnedTiles}, " +
                $"U.{DbStrings.Id}, U.{DbStrings.Username}, U.{DbStrings.Nickname}, U.{DbStrings.ProfilePic} " +
                $"FROM {from} " +
                $"JOIN {DbStrings.UserTable} AS U ON U.{DbStrings.Id} = {Table}.{DbStrings.UserId} ";
        }

        private static TopPlayer ReaderToEnt(IDataRecord reader)
        {
            var user = new User(reader.GetString(3), reader.GetString(4),
                reader.GetString(5), reader.GetValue(6).ToString());
            TopPlayer ent = new(reader.GetString(0), reader.GetString(1), user, reader.GetInt32(2));

            return ent;
        }

        private static async Task<TopPlayer?> Ent(string sqlCommand, MySqlConnection connection)
        {
            TopPlayer? ent = null;
            await using var con = connection;
                con.Open();

            await using var command = new MySqlCommand(sqlCommand, con);
            await using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) ent = ReaderToEnt(reader);

            return ent;
        }
    }
}