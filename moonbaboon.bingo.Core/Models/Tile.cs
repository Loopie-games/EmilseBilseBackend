namespace moonbaboon.bingo.Core.Models
{
    public class Tile : ITile
    {
        public Tile(string? id, string action, string addedBy)
        {
            Id = id;
            Action = action;
            AddedBy = addedBy;
        }

        public string? Id { get; set; }
        public string Action { get; set; }
        public string AddedBy { get; }
    }
}