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
            Id = reader.GetString("TilePack_Id");
            Name = reader.GetString("TilePack_Name");
            PicUrl = reader.GetValue("TilePack_PicUrl").ToString();
            PriceStripe = reader.GetValue("TilePack_Stripe_PRICE").ToString();
        }

        public TilePack(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("TilePack_Id"));
            Name = reader.GetString(reader.GetOrdinal("TilePack_Name"));
            PicUrl = reader.GetValue(reader.GetOrdinal("TilePack_PicUrl")).ToString();
            PriceStripe = reader.GetValue(reader.GetOrdinal("TilePack_Stripe_PRICE")).ToString();
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public string? PicUrl { get; set; }
        public bool? IsOwned { get; set; }
        public string? PriceStripe { get; set; }
    }
}