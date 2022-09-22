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

        public string? Id { get; set; }
        public string Name { get; set; }
        public string? PicUrl { get; set; }
        public bool? IsOwned { get; set; }
        public string? PriceStripe { get; set; }
    }
}