using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class TileRepository : ITileRepository
    {
        //Table
        private const string Table = "BingoTile";

        //Rows
        private const string Id = "Id";
        private const string UserId = "UserId";
        private const string Action = "Action";
        private const string AddedById = "AddedById";

        private readonly MySqlConnection _connection = new MySqlConnection("Server=185.51.76.204; Database=emilse_bilse_bingo; Uid=root; PWD=hemmeligt;");

        public async Task<Tile?> Create(Tile tileToCreate)
        {
            tileToCreate.Id = Guid.NewGuid().ToString();
            bool success = true;

            var sqlCommand =
                $"INSERT INTO `{Table}` VALUES ('{tileToCreate.Id}', '{tileToCreate.UserId}', '{tileToCreate.Action}'";

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

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM `{Table}` ORDER BY `{Id}`;", _connection);
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

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM `{Table}` WHERE `{Id}`='{id}';", _connection);
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

            await using MySqlCommand command = new MySqlCommand($"DELETE FROM `{Table}` WHERE `{Id}`='{id}'", _connection);
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

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM `{Table}` WHERE `{UserId}` = '{id}';", _connection);
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
    }
}
