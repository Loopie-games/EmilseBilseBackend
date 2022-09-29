using System;

namespace moonbaboon.bingo.Core.Models
{
    public class UserTile : ByTile
    {
        public UserTile(string id, Tile tile, UserSimple user, UserSimple addedByUser) : base(id, tile, Models.TileType.UserTile)
        {
            Id = id;
            Tile = tile;
            User = user;
            AddedByUser = addedByUser;
        }
        public UserSimple User { get; set; }
        public UserSimple AddedByUser { get; set; }
    }
}