using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class UserTileRepository : IUserTileRepository
    {
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);

        /*
        public async Task<List<UserTile>> FindAll()
        {
            List<UserTile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(SqlSelect($"{DbStrings.UserTileTable}"), _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) tiles.Add(ReaderToUserTile(reader));
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<UserTile?> FindById(string id)
        {
            UserTile? tile = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                SqlSelect($"{DbStrings.UserTileTable}") +
                $"WHERE {DbStrings.UserTileTable}.{DbStrings.TileId} ='{id}';"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) tile = ReaderToUserTile(reader);

            await _connection.CloseAsync();
            return tile;
        }

        public async Task<UserTile?> FindFiller(string userId)
        {
            UserTile? tile = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                SqlSelect(DbStrings.UserTileTable) +
                $"WHERE ({DbStrings.TileTable}.{DbStrings.Action} ='filler' AND {DbStrings.UserTileTable}.{DbStrings.UserId} ='{userId}');"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) tile = ReaderToUserTile(reader);

            await _connection.CloseAsync();
            return tile;
        }

        public async Task<List<UserTile>> GetAboutUserById(string id)
        {
            List<UserTile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                SqlSelect(DbStrings.UserTileTable) +
                $"WHERE {DbStrings.UserTileTable}.{DbStrings.UserId} = '{id}';"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) tiles.Add(ReaderToUserTile(reader));
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<UserTile?> Create(string userId, string action, string addedById)
        {
            UserTile? ent = null;
            var uuid = Guid.NewGuid().ToString();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"INSERT INTO {DbStrings.TileTable} " +
                $"VALUES ('{uuid}', '{action}');" +
                $"INSERT INTO {DbStrings.UserTileTable} " +
                $"VALUES ('{uuid}','{userId}', '{addedById}'); " +
                SqlSelect(DbStrings.UserTileTable) +
                $"WHERE {DbStrings.UserTileTable}.{DbStrings.TileId} = '{uuid}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToUserTile(reader);

            await _connection.CloseAsync();
            return ent;
        }

        public async Task<List<UserTile>> GetTilesForBoard(string lobbyId, string userId)
        {
            var sqlcommand =
                $"SELECT {DbStrings.UserTileTable}.{DbStrings.Id}, {DbStrings.UserTileTable}.{DbStrings.Action}, " +
                $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic},  " +
                $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                $"JOIN {DbStrings.UserTable} As U1 ON {DbStrings.UserTileTable}.{DbStrings.UserId} = U1.id " +
                $"JOIN {DbStrings.UserTable} As U2 ON {DbStrings.UserTileTable}.{DbStrings.AddedById} = U2.id";

            List<UserTile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(SqlSelect(
                    $"(SELECT {DbStrings.PendingPlayerTable}.{DbStrings.UserId} as uId " +
                    $"FROM {DbStrings.PendingPlayerTable} " +
                    $"WHERE {DbStrings.PendingPlayerTable}.LobbyId = '{lobbyId}' " +
                    $"and {DbStrings.PendingPlayerTable}.{DbStrings.UserId} != '{userId}') As pp " +
                    $"JOIN {DbStrings.UserTileTable} on pp.uId = {DbStrings.UserTileTable}.{DbStrings.UserId}")
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) tiles.Add(ReaderToUserTile(reader));
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<List<UserTile>> FindMadeByUserId(string userId)
        {
            List<UserTile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                SqlSelect(DbStrings.UserTileTable) +
                $"WHERE {DbStrings.UserTileTable}.{DbStrings.AddedById} = '{userId}';"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) tiles.Add(ReaderToUserTile(reader));
            await _connection.CloseAsync();
            return tiles;
        }

        private static string SqlSelect(string from)
        {
            return $"SELECT {DbStrings.TileTable}.{DbStrings.Id}, {DbStrings.TileTable}.{DbStrings.Action}, " +
                   $"U1.{DbStrings.Id}, U1.{DbStrings.Username}, U1.{DbStrings.Nickname}, U1.{DbStrings.ProfilePic},  " +
                   $"U2.{DbStrings.Id}, U2.{DbStrings.Username}, U2.{DbStrings.Nickname}, U2.{DbStrings.ProfilePic} " +
                   $"FROM {from} " +
                   $"JOIN {DbStrings.TileTable} ON {DbStrings.TileTable}.{DbStrings.Id} = {DbStrings.UserTileTable}.{DbStrings.TileId} " +
                   $"JOIN {DbStrings.UserTable} AS U1 ON U1.{DbStrings.Id} = {DbStrings.UserTileTable}.{DbStrings.UserId} " +
                   $"JOIN {DbStrings.UserTable} AS U2 ON U2.{DbStrings.Id} = {DbStrings.UserTileTable}.{DbStrings.AddedById} ";
        }

        private static UserTile ReaderToUserTile(MySqlDataReader reader)
        {
            UserSimple about = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
            UserSimple addedBy = new(reader.GetValue(6).ToString(), reader.GetValue(7).ToString(),
                reader.GetValue(8).ToString(), reader.GetValue(9).ToString());
            //Needs own id
            UserTile userTile = new(new Tile(reader.GetValue(0).ToString(), reader.GetValue(1).ToString()), about, addedBy);

            return userTile;
        }
    
    */
        public Task<UserTile?> Create(string userId, string action, string addedById)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserTile>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<UserTile?> FindById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<UserTile?> FindFiller(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserTile>> GetAboutUserById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserTile>> GetTilesForBoard(string lobbyId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserTile>> FindMadeByUserId(string userId)
        {
            throw new NotImplementedException();
        }
    }
}