namespace moonbaboon.bingo.Core.Models
{
    public class PackTile : Tile
    {
        public PackTile(string? id, string action, TilePack pack) : base(id, action, pack.Name, TileType.PackTile)
        {
            Id = id;
            Action = action;
            Pack = pack;
            AddedBy = Pack.Name;
        }

        public string? Id { get; set; }
        public string Action { get; set; }
        public string AddedBy { get; }
        public TileType TileType => TileType.PackTile;
        public TilePack Pack { get; set; }
    }
}