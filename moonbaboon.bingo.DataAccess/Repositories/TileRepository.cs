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

        public async Task<Tile?> Create(Tile tileToCreate)
        {
            tileToCreate.Id = Guid.NewGuid().ToString();
            bool success = true;

            var sqlCommand =
                $"INSERT INTO `{DBStrings.TileTable}` VALUES ('{tileToCreate.Id}', '{tileToCreate.UserId}', '{tileToCreate.Action}'";

            if (tileToCreate.AddedById is not null)
            {
                sqlCommand += $",'{tileToCreate.AddedById}'";
            }

            sqlCommand += ");";

            await _connection.OpenAsync();

            await using MySqlCommand command = new(sqlCommand, _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                success = reader.GetBoolean(0); //INSERT statement only returns a boolean, so assumption can be made about return-type
            }

            await _connection.CloseAsync();
            return (success) ? tileToCreate : null;
        }

        public async Task<List<Tile>> FindAll()
        {
            List<Tile> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM `{DBStrings.TileTable}` ORDER BY `{DBStrings.Id}`;", _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                Tile tile = new Tile(reader.GetValue(1).ToString() ?? "[ERR]", reader.GetValue(2).ToString() ?? "[ERR]")
                {
                    Id = reader.GetValue(0).ToString() ?? "[ERR]",
                    AddedById = reader.GetValue(3).ToString() ?? "[ERR]",
                };
                tiles.Add(tile);
            }
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<Tile?> FindById(string id)
        {
            Tile? tile = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM `{DBStrings.TileTable}` WHERE `{DBStrings.Id}`='{id}';", _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tile = new Tile(reader.GetValue(1).ToString() ?? "[ERR]", reader.GetValue(2).ToString() ?? "[ERR]")
                {
                    Id = reader.GetValue(0).ToString() ?? "[ERR]",
                    AddedById = reader.GetValue(3).ToString() ?? "[ERR]",
                };
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

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM `{DBStrings.TileTable}` WHERE `{DBStrings.UserId}` = '{id}';", _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                Tile tile = new Tile(reader.GetValue(1).ToString() ?? "[ERR]", reader.GetValue(2).ToString() ?? "[ERR]")
                {
                    Id = reader.GetValue(0).ToString() ?? "[ERR]",
                    AddedById = reader.GetValue(3).ToString() ?? "[ERR]",
                };
                tiles.Add(tile);
            }
            await _connection.CloseAsync();
            return tiles;
        }
        
        public async Task<List<TileForUser>> GetAboutUserById_TileForUser(string id)
        {
            List<TileForUser> tiles = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT {DBStrings.TileTable}.{DBStrings.Id}, u1.{DBStrings.Nickname}, {DBStrings.TileTable}.{DBStrings.Action}, u2.{DBStrings.Nickname} " +
                $"FROM `{DBStrings.TileTable}` " +
                $"JOIN {DBStrings.UserTable} as u1 on u1.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.UserId} " +
                $"JOIN {DBStrings.UserTable} as u2 on u2.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.AddedById} " +
                $"WHERE {DBStrings.TileTable}.{DBStrings.UserId} = '{id}'", _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                TileForUser tile = new(reader.GetValue(1).ToString() ?? "[ERR]", reader.GetValue(2).ToString() ?? "[ERR]")
                {
                    Id = reader.GetValue(0).ToString() ?? "[ERR]",
                    AddedByNickname = reader.GetValue(3).ToString() ?? "[ERR]",
                };
                tiles.Add(tile);
            }
            await _connection.CloseAsync();
            return tiles;
        }

        public async Task<TileForUser?> CreateTile_TileForUser(Tile tileToCreate)
        {
            TileForUser? ent = null;
            tileToCreate.Id = Guid.NewGuid().ToString();
            var sqlCommand =
                $"INSERT INTO {DBStrings.TileTable} " +
                $"VALUES ('{tileToCreate.Id}','{tileToCreate.UserId}', '{tileToCreate.Action}'";

            if (tileToCreate.AddedById is not null)
            {
                sqlCommand += $",'{tileToCreate.AddedById}'";
            }

            sqlCommand += "); " +
                          $"SELECT {DBStrings.TileTable}.{DBStrings.Id}, u1.{DBStrings.Nickname}, {DBStrings.TileTable}.{DBStrings.Action}, u2.{DBStrings.Nickname} " +
                          $"FROM `{DBStrings.TileTable}` " +
                          $"JOIN {DBStrings.UserTable} as u1 on u1.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.UserId} " +
                          $"JOIN {DBStrings.UserTable} as u2 on u2.{DBStrings.Id} = {DBStrings.TileTable}.{DBStrings.AddedById} " +
                          $"WHERE {DBStrings.TileTable}.{DBStrings.Id} = '{tileToCreate.Id}'";;

            await _connection.OpenAsync();

            await using MySqlCommand command = new(sqlCommand, _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                if (!reader.HasRows) continue;
                ent = new TileForUser(reader.GetValue(1).ToString(), reader.GetValue(2).ToString())
                {
                    Id = reader.GetValue(0).ToString()
                };
                var addedBy = reader.GetValue(3).ToString();
                if (!string.IsNullOrEmpty(addedBy))
                {
                    ent.AddedByNickname = addedBy;
                }
            }

            await _connection.CloseAsync();
            return ent;
        }
    }
}
