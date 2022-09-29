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
            await _connection.OpenAsync();

            await using var command = new MySqlCommand("SELECT Tile.Id AS Tile_Id, Tile.Action AS Tile_Action FROM Tile", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) tiles.Add(new Tile(reader));
            return tiles;
        }
    }
}