using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class PackTile : Tile
    {
        public PackTile(string? id, string action, TilePack pack) : base(id, action, pack.Name, Models.TileType.PackTile)
        {
            Id = id;
            Action = action;
            Pack = pack;
        }

        public PackTile(MySqlDataReader reader) : base(reader.GetString("TileId"), reader.GetString("TileAction"), reader.GetString("TilePackName"), Models.TileType.PackTile)
        {
            Id = reader.GetString("TileID");
            Action = reader.GetString("TileAction");
            Pack = new TilePack(reader.GetString("TilePackId"), reader.GetString("TilePackName"),
                reader.GetString("TilePackPic"), reader.GetString("TilePackPrice"));

        }

        public string? Id { get; set; }
        public string Action { get; set; }
        public new string AddedBy => Pack.Name;
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