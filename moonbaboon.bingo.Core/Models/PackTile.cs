using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class PackTile : ByTile
    {
        public PackTile(string? id, Tile tile, TilePack pack) : base(id, tile, Models.TileType.PackTile)
        {
            Id = id;
            Pack = pack;
            Tile = tile;
        }

        public PackTile(MySqlDataReader reader) : base(reader.GetString("PackTile_Id"), new Tile(reader),
            Models.TileType.PackTile)
        {
            Id = reader.GetString("PackTile_Id");
            Pack = new TilePack(reader);
            Tile = new Tile(reader);
        }

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