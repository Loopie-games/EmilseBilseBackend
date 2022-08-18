namespace moonbaboon.bingo.Core.Models
{
    public class BoardTile
    {
        public BoardTile(Board board, string tileId, int position, bool isActivated)
        {
            Board = board;
            TileId = tileId;
            Position = position;
            IsActivated = isActivated;
        }

        public string? Id { get; set; }
        public Board Board { get; set; }
        public string TileId { get; set; }
        public int Position { get; set; }
        public bool IsActivated { get; set; }
    }
}