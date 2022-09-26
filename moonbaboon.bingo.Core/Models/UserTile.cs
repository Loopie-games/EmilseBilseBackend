namespace moonbaboon.bingo.Core.Models
{
    public class UserTile : Tile
    {
        public UserTile(string? id, string action, UserSimple user, UserSimple addedByUser) : base(id, action,
            addedByUser.Username, TileType.UserTile)
        {
            Id = id;
            Action = action;
            User = user;
            AddedByUser = addedByUser;
            AddedBy = addedByUser.Username;
        }

        public string? Id { get; set; }
        public string Action { get; set; }
        public string AddedBy { get; }
        public TileType TileType => TileType.UserTile;

        public UserSimple User { get; set; }

        public UserSimple AddedByUser { get; set; }
    }
}