namespace moonbaboon.bingo.Core.Models
{
    public class TilePack
    {
        public TilePack(string? id, string name, string? picUrl)
        {
            Id = id;
            Name = name;
            PicUrl = picUrl;
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        
        public string? PicUrl { get; set; }
    }
}