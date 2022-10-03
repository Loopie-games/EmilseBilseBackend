using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class UserTile : ByTile
    {
        public UserTile(string id, Tile tile, UserSimple user, UserSimple addedByUser) : base(id, tile,
            Models.TileType.UserTile)
        {
            Id = id;
            Tile = tile;
            User = user;
            AddedByUser = addedByUser;
        }

        public UserTile(MySqlDataReader reader) : base(reader.GetString("UserTile_Id"), new Tile(reader),
            Models.TileType.UserTile)
        {
            Id = reader.GetString("UserTile_Id");
            Tile = new Tile(reader);
            User = new UserSimple(reader.GetString("About_Id"), reader.GetString("About_Username"),
                reader.GetString("About_Nickname"), reader.GetValue("About_ProfilePicUrl").ToString());
            AddedByUser = new UserSimple(reader.GetString("AddedBy_Id"), reader.GetString("AddedBy_Username"),
                reader.GetString("AddedBy_Nickname"), reader.GetValue("AddedBy_ProfilePicUrl").ToString());
        }

        public UserSimple User { get; set; }
        public UserSimple AddedByUser { get; set; }
    }

    public class UserTileEntity
    {
        public UserTileEntity(string? id, string tileId, string aboutUserId, string addedByUserId)
        {
            Id = id;
            TileId = tileId;
            AboutUserId = aboutUserId;
            AddedByUserId = addedByUserId;
        }

        public UserTileEntity(UserTile userTile)
        {
            Id = userTile.Id;
            TileId = userTile.Tile.Id;
            AboutUserId = userTile.User.Id;
            AddedByUserId = userTile.AddedByUser.Id;
        }

        public string? Id { get; set; }
        public string TileId { get; set; }
        public string AboutUserId { get; set; }
        public string AddedByUserId { get; set; }
    }
}