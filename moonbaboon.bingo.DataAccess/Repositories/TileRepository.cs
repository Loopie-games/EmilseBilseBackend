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
        private readonly MySqlConnection _connection = new(DatabaseStrings.SqLconnection);

        public async Task<Tile?> Create(Tile tileToCreate)
        {
            tileToCreate.Id = Guid.NewGuid().ToString();
            bool success = true;

            var sqlCommand =
                $"INSERT INTO `{DatabaseStrings.TileTable}` VALUES ('{tileToCreate.Id}', '{tileToCreate.UserId}', '{tileToCreate.Action}'";

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

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM `{DatabaseStrings.TileTable}` ORDER BY `{DatabaseStrings.Id}`;", _connection);
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

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM `{DatabaseStrings.TileTable}` WHERE `{DatabaseStrings.Id}`='{id}';", _connection);
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

            await using MySqlCommand command = new MySqlCommand($"DELETE FROM `{DatabaseStrings.TileTable}` WHERE `{DatabaseStrings.Id}`='{id}'", _connection);
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

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM `{DatabaseStrings.TileTable}` WHERE `{DatabaseStrings.UserId}` = '{id}';", _connection);
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
                $"SELECT {DatabaseStrings.TileTable}.{DatabaseStrings.Id}, u1.{DatabaseStrings.Nickname}, {DatabaseStrings.TileTable}.{DatabaseStrings.Action}, u2.{DatabaseStrings.Nickname} " +
                $"FROM `{DatabaseStrings.TileTable}` " +
                $"JOIN {DatabaseStrings.UserTable} as u1 on u1.{DatabaseStrings.Id} = {DatabaseStrings.TileTable}.{DatabaseStrings.UserId} " +
                $"JOIN {DatabaseStrings.UserTable} as u2 on u2.{DatabaseStrings.Id} = {DatabaseStrings.TileTable}.{DatabaseStrings.AddedById} " +
                $"WHERE {DatabaseStrings.TileTable}.{DatabaseStrings.UserId} = '{id}'", _connection);
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

        public TileForUser CreateTile_TileForUser(TileNewFromUser tileToCreate)
        {
            throw new NotImplementedException();
        }
    }
}
