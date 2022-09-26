using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class PackTile : Tile
    {
        public PackTile(string? packTileId, Tile tile, TilePack pack) : base(tile.Id, tile.Action, pack.Name, Models.TileType.PackTile)
        {
            PackTileId = packTileId;
            Tile = tile;
            Pack = pack;
        }

        public PackTile(MySqlDataReader reader) : base(reader.GetString("TileId"), reader.GetString("TileAction"), reader.GetString("TilePackName"), Models.TileType.PackTile)
        {
            PackTileId = reader.GetString("PackTileId");
            Pack = new TilePack(reader.GetString("TilePackId"), reader.GetString("TilePackName"),
                reader.GetValue("TilePackPic").ToString(), reader.GetValue("TilePackPrice").ToString());
            Tile = new Tile(reader.GetString("TileID"), reader.GetString("TileAction"), Pack.Name, Models.TileType.PackTile);
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