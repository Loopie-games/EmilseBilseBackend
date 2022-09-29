using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class PackTile : Tile
    {
        public PackTile(string? packTileId, Tile tile, TilePack pack) : base(tile.Id, tile.Action, pack.Name,
            Models.TileType.PackTile)
        {
            PackTileId = packTileId;
            Tile = tile;
            Pack = pack;
        }

        public PackTile(MySqlDataReader reader) : base(reader)
        {
            PackTileId = reader.GetString("PackTile_Id");
            Pack = new TilePack(reader);
            Tile = new Tile(reader)
            {
                AddedBy = Pack.Name,
                TileType = Models.TileType.PackTile
            };
        }

        public string? PackTileId { get; set; }
        public Tile Tile { get; set; }
        public TilePack Pack { get; set; }
    }

    public class PackTileEntity
    {
        public PackTileEntity(string tileId, string packId)
        {
            TileId = tileId;
            PackId = packId;
        }

        public string? Id { get; set; }
        public string TileId { get; set; }
        public string PackId { get; set; }
    }
}