namespace moonbaboon.bingo.Core.Models
{
    public interface ITile
    {

        public string? Id { get; set; }
        
        public string Action { get; set; }
        
        public string AddedBy { get; }
    }
}