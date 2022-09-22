using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class PackTile : Tile
    {
        public PackTile(Tile tile, TilePack pack) : base(tile.Id, tile.Action, pack.Name, Models.TileType.PackTile)
        {
            Tile = tile;
            Pack = pack;
        }

        public PackTile(MySqlDataReader reader) : base(reader.GetString("TileId"), reader.GetString("TileAction"), reader.GetString("TilePackName"), Models.TileType.PackTile)
        {
            Pack = new TilePack(reader.GetString("TilePackId"), reader.GetString("TilePackName"),
                reader.GetString("TilePackPic"), reader.GetString("TilePackPrice"));
            Tile = new Tile(reader.GetString("TileID"), reader.GetString("TileAction"), Pack.Name, Models.TileType.PackTile);
        }

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

        public string TileId { get; set; }
        public string PackId { get; set; }
        
    }
}