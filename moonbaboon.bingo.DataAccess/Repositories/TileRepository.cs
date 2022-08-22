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

        public async Task<Tile?> FindFiller(string userId)
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
                $"WHERE {DBStrings.TileTable}.{DBStrings.Action} ='filler' AND {DBStrings.TileTable}.{DBStrings.UserId} ='{userId}';"
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

        public async Task<List<Tile>> GetTilesForBoard(string lobbyId, string userId)
        {
            var sqlcommand =
                $"SELECT {DBStrings.TileTable}.{DBStrings.Id}, {DBStrings.TileTable}.{DBStrings.Action}, " +
                $"U1.{DBStrings.Id}, U1.{DBStrings.Username}, U1.{DBStrings.Nickname}, U1.{DBStrings.ProfilePic},  " +
                $"U2.{DBStrings.Id}, U2.{DBStrings.Username}, U2.{DBStrings.Nickname}, U2.{DBStrings.ProfilePic} " +
                $"FROM (SELECT PendingPlayer.UserId as uId " +
                $"FROM PendingPlayer " +
                $"WHERE PendingPlayer.LobbyId = '{lobbyId}' " +
                $"and PendingPlayer.UserId != '{userId}') As pp " +
                $"JOIN BingoTile on pp.uId = BingoTile.UserId " +
                $"JOIN User As U1 ON BingoTile.UserId = U1.id " +
                $"JOIN User As U2 ON BingoTile.AddedById = U2.id";
            
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
