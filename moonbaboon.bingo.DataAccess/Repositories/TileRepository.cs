using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class TileRepository : ITileRepository
    {
        private readonly MySqlConnection _connection;

        public TileRepository(MySqlConnection connection)
        {
            _connection = connection.Clone();
        }

        public async Task<List<Tile>> GetAll()
        {
            List<Tile> tiles = new();
            await _connection.OpenAsync();

            await using var command =
                new MySqlCommand("SELECT Tile.Id AS Tile_Id, Tile.Action AS Tile_Action FROM Tile", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) tiles.Add(new Tile(reader));
            return tiles;
        }

        public async Task<List<Tile>> GetTilesUsedInPacks()
        {
            var list = new List<Tile>();
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command = new(
                    "SELECT * FROM Tile RIGHT JOIN PackTile on PackTile.TileId = Tile.Id"
                    , con);
                await using MySqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    list.Add(new Tile(reader.GetString(0), reader.GetString(1)));

                return list;
            }
        }
    }
}