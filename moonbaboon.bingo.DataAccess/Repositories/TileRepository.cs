using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class TileRepository : ITileRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);

        public Tile readerToTile(MySqlDataReader reader)
        {
            UserSimple about = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
            UserSimple addedBy = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
            Tile tile = new Tile(about, reader.GetValue(1).ToString(), addedBy)
            {
                Id = reader.GetValue(0).ToString()
            };

            return tile;
        }

        public async Task<List<Tile>> FindAll()
        {
            List<Tile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT {DBStrings.TileTable}.{DBStrings.Id}, {DBStrings.TileTable}.{DBStrings.Action}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic},  " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM `{DBStrings.TileTable}` " +
                $"JOIN {DBStrings.UserTable} AS U1 ON U1.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.UserId} " +
                $"JOIN {DBStrings.UserTable} AS U2 ON U2.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.AddedById} "
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                tiles.Add(readerToTile(reader));
            }
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<Tile?> FindById(string id)
        {
            Tile? tile = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT {DBStrings.TileTable}.{DBStrings.Id}, {DBStrings.TileTable}.{DBStrings.Action}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic},  " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM `{DBStrings.TileTable}` " +
                $"JOIN {DBStrings.UserTable} AS U1 ON U1.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.UserId} " +
                $"JOIN {DBStrings.UserTable} AS U2 ON U2.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.AddedById} "+ 
                $"WHERE {DBStrings.TileTable}.{DBStrings.Id} ='{id}';"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tile = readerToTile(reader);
            }

            await _connection.CloseAsync();
            return tile;
        }

        public async Task<bool> Delete(string id)
        {
            bool success = false;
            await _connection.OpenAsync();

            await using MySqlCommand command = new MySqlCommand($"DELETE FROM `{DBStrings.TileTable}` WHERE `{DBStrings.Id}`='{id}'", _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                success = reader.GetBoolean(0); //DELETE statement only returns a boolean, so assumption can be made about return-type
            }

            await _connection.CloseAsync();
            return success;
        }

        public async Task<List<Tile>> GetAboutUserById(string id)
        {
            List<Tile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT {DBStrings.TileTable}.{DBStrings.Id}, {DBStrings.TileTable}.{DBStrings.Action}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic},  " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM `{DBStrings.TileTable}` " +
                $"JOIN {DBStrings.UserTable} AS U1 ON U1.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.UserId} " +
                $"JOIN {DBStrings.UserTable} AS U2 ON U2.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.AddedById} "+ 
                $"WHERE {DBStrings.TileTable}.{DBStrings.UserId} = '{id}';"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                tiles.Add(readerToTile(reader));
            }
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<Tile?> Create(string userId, string action, string addedById)
        {
            Tile? ent = null;
            var uuid = Guid.NewGuid().ToString();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"INSERT INTO {DBStrings.TileTable} " +
                $"VALUES ('{uuid}','{userId}', '{action}','{addedById}'); " +
                $"SELECT {DBStrings.TileTable}.{DBStrings.Id}, {DBStrings.TileTable}.{DBStrings.Action}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic},  " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM `{DBStrings.TileTable}` " +
                $"JOIN {DBStrings.UserTable} AS U1 ON U1.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.UserId} " +
                $"JOIN {DBStrings.UserTable} AS U2 ON U2.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.AddedById} "+ 
                $"WHERE {DBStrings.TileTable}.{DBStrings.Id} = '{uuid}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                ent = readerToTile(reader);
            }

            await _connection.CloseAsync();
            return ent;
        }

        public async Task<List<Tile>> GetTilesForBoard(List<PendingPlayer> pendingPlayers, string userId)
        {
            var sqlcommand =
                $"SELECT t1.Id, t1.Action, " +
                $"u1.id, u1.username, u1.nickname, u1.ProfilePicURL, " +
                $"u2.id, u2.username, u2.nickname, u2.ProfilePicURL " +
                $"FROM(";

                int i = 0;
            foreach (var player in pendingPlayers)
            {
                if (i == 0)
                {
                    sqlcommand +=
                        $"SELECT * FROM BingoTile AS b{i} WHERE b{i}.UserId = '{player.User.Id}' ";
                }
                else
                {
                    sqlcommand += $"UNION ALL SELECT * FROM BingoTile AS b{i} WHERE b{i}.UserId = '{player.User.Id}' ";
                }

                i++;
            }
            
            
            sqlcommand += $") AS t1 JOIN (SELECT bt1.Id FROM BingoTile as bt1 WHERE bt1.UserId !='{userId}' ORDER BY RAND() LIMIT 24) As t2 ON t2.id = t1.id JOIN User as u1 on u1.id = t1.UserId JOIN User as u2 on u2.id = t1.AddedById";
            
            List<Tile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(sqlcommand
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                tiles.Add(readerToTile(reader));
            }
            await _connection.CloseAsync();
            return tiles;
        }
    }
}
