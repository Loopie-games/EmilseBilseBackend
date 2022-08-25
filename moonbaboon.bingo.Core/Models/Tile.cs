namespace moonbaboon.bingo.Core.Models
{
    public class Tile : ITile
    {
        public Tile(string? id, string action)
        {
            Id = id;
            Action = action;
        }

        public string? Id { get; set; }
        public string Action { get; set; }
    }
}