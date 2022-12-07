using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class UserTile : ByTile
    {
        public UserTile(MySqlDataReader reader) : base(reader.GetString("UserTile_Id"), new Tile(reader),
            Models.TileType.UserTile)
        {
            Id = reader.GetString("UserTile_Id");
            Tile = new Tile(reader);
            User = new User(reader.GetString("About_Id"), reader.GetString("About_Username"),
                reader.GetString("About_Nickname"), reader.GetValue("About_ProfilePicUrl").ToString(), 
                reader.GetString("About_BannerPicUrl"), reader.GetString("About_Email"), reader.GetDateTime("About_Birthdate"));
            AddedByUser = new User(reader.GetString("AddedBy_Id"), reader.GetString("AddedBy_Username"),
                reader.GetString("AddedBy_Nickname"), reader.GetValue("AddedBy_ProfilePicUrl").ToString(), 
                reader.GetString("AddedBy_BannerPicUrl"), reader.GetString("AddedBy_Email"), reader.GetDateTime("AddedBy_Birthdate"));
        }

        public User User { get; set; }
        public User AddedByUser { get; set; }
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

        public string? Id { get; set; }
        public string TileId { get; set; }
        public string AboutUserId { get; set; }
        public string AddedByUserId { get; set; }
    }
}