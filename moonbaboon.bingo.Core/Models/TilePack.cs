using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class TilePack
    {
        public TilePack(string? id, string name, string? picUrl, string? priceStripe)
        {
            Id = id;
            Name = name;
            PicUrl = picUrl;
            PriceStripe = priceStripe;
        }

        public TilePack(MySqlDataReader reader)
        {
            Id = reader.GetString("TilePackId");
            Name = reader.GetString("TilePackName");
            PicUrl = reader.GetValue("TilePackPic").ToString();
            PriceStripe = reader.GetValue("TilePackStripe").ToString();
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public string? PicUrl { get; set; }
        public bool? IsOwned { get; set; }
        public string? PriceStripe { get; set; }
    }
}