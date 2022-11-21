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
            _connection = connection;
        }

        public async Task<List<Tile>> GetAll()
        {
            List<Tile> tiles = new();
            await using var con = _connection.Clone();
            con.Open();
            await using var command =
                new MySqlCommand("SELECT * FROM Tile", con);
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
                    "SELECT * FROM Tile RIGHT JOIN PackTile on PackTile.PackTile_TileId = Tile.Tile_Id"
                    , con);
                await using MySqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    list.Add(new Tile(reader));

                return list;
            }
        }
    }
}