namespace moonbaboon.bingo.Core.Models
{
    public class UserTile : Tile
    {
        public UserTile(Tile tile, UserSimple user, UserSimple addedByUser) : base(tile.Id, tile.Action,
            addedByUser.Username, Models.TileType.UserTile)
        {
            Tile = tile;
            User = user;
            AddedByUser = addedByUser;
        }

        public Tile Tile { get; set; }
        public UserSimple User { get; set; }
        public UserSimple AddedByUser { get; set; }
    }
}