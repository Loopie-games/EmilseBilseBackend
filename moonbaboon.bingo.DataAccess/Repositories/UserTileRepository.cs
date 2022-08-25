using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class UserTileRepository : IUserTileRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);

        private static string SqlSelect(string from)
        {
            return $"SELECT {DBStrings.TileTable}.{DBStrings.Id}, {DBStrings.TileTable}.{DBStrings.Action}, " +
                   $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic},  " +
                   $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                   $"FROM {from} " +
                   $"JOIN {DBStrings.TileTable} ON {DBStrings.TileTable}.{DBStrings.Id} = {DBStrings.UserTileTable}.{DBStrings.TileId} " +
                   $"JOIN {DBStrings.UserTable} AS U1 ON U1.{DBStrings.Id} = {DBStrings.UserTileTable}.{DBStrings.UserId} " +
                   $"JOIN {DBStrings.UserTable} AS U2 ON U2.{DBStrings.Id} = {DBStrings.UserTileTable}.{DBStrings.AddedById} ";
        }

        private static UserTile ReaderToUserTile(MySqlDataReader reader)
        {
            UserSimple about = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
            UserSimple addedBy = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
            UserTile userTile = new(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), about, addedBy);

            return userTile;
        }

        public async Task<List<UserTile>> FindAll()
        {
            List<UserTile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(SqlSelect($"{DBStrings.UserTileTable}"), _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                tiles.Add(ReaderToUserTile(reader));
            }
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<UserTile?> FindById(string id)
        {
            UserTile? tile = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                SqlSelect($"{DBStrings.UserTileTable}") + 
                $"WHERE {DBStrings.UserTileTable}.{DBStrings.TileId} ='{id}';"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tile = ReaderToUserTile(reader);
            }

            await _connection.CloseAsync();
            return tile;
        }

        public async Task<UserTile?> FindFiller(string userId)
        {
            UserTile? tile = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                SqlSelect(DBStrings.UserTileTable) + 
                $"WHERE ({DBStrings.TileTable}.{DBStrings.Action} ='filler' AND {DBStrings.UserTileTable}.{DBStrings.UserId} ='{userId}');"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tile = ReaderToUserTile(reader);
            }

            await _connection.CloseAsync();
            return tile;
        }

        public async Task<List<UserTile>> GetAboutUserById(string id)
        {
            List<UserTile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                SqlSelect(DBStrings.UserTileTable) +
                $"WHERE {DBStrings.UserTileTable}.{DBStrings.UserId} = '{id}';"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                tiles.Add(ReaderToUserTile(reader));
            }
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<UserTile?> Create(string userId, string action, string addedById)
        {
            UserTile? ent = null;
            var uuid = Guid.NewGuid().ToString();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"INSERT INTO {DBStrings.TileTable} " +
                $"VALUES ('{uuid}', '{action}');" +
                $"INSERT INTO {DBStrings.UserTileTable} " +
                $"VALUES ('{uuid}','{userId}', '{addedById}'); " +
                SqlSelect(DBStrings.UserTileTable) + 
                $"WHERE {DBStrings.UserTileTable}.{DBStrings.TileId} = '{uuid}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                ent = ReaderToUserTile(reader);
            }

            await _connection.CloseAsync();
            return ent;
        }

        public async Task<List<UserTile>> GetTilesForBoard(string lobbyId, string userId)
        {
            var sqlcommand =
                $"SELECT {DBStrings.UserTileTable}.{DBStrings.Id}, {DBStrings.UserTileTable}.{DBStrings.Action}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic},  " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"JOIN {DBStrings.UserTable} As U1 ON {DBStrings.UserTileTable}.{DBStrings.UserId} = U1.id " +
                $"JOIN {DBStrings.UserTable} As U2 ON {DBStrings.UserTileTable}.{DBStrings.AddedById} = U2.id";
            
            List<UserTile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(SqlSelect($"(SELECT {DBStrings.PendingPlayerTable}.{DBStrings.UserId} as uId " +
                                                             $"FROM {DBStrings.PendingPlayerTable} " +
                                                             $"WHERE {DBStrings.PendingPlayerTable}.LobbyId = '{lobbyId}' " +
                                                             $"and {DBStrings.PendingPlayerTable}.{DBStrings.UserId} != '{userId}') As pp " +
                                                             $"JOIN {DBStrings.UserTileTable} on pp.uId = {DBStrings.UserTileTable}.{DBStrings.UserId}" )
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                tiles.Add(ReaderToUserTile(reader));
            }
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<List<UserTile>> FindMadeByUserId(string userId)
        {
            List<UserTile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                SqlSelect(DBStrings.UserTileTable) +
                $"WHERE {DBStrings.UserTileTable}.{DBStrings.AddedById} = '{userId}';"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                tiles.Add(ReaderToUserTile(reader));
            }
            await _connection.CloseAsync();
            return tiles;
        }
    }
}
