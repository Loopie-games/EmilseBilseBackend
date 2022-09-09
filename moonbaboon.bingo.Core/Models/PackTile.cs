namespace moonbaboon.bingo.Core.Models
{
    public class PackTile: ITile
    {
        public PackTile(string? id, string action, TilePack pack)
        {
            Id = id;
            Action = action;
            Pack = pack;
            AddedBy = Pack.Name;
        }

        public string? Id { get; set; }
        public string Action { get; set; }
        public string AddedBy { get; }
        public TilePack Pack { get; set; }
    }
}