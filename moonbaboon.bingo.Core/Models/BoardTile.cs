namespace moonbaboon.bingo.Core.Models
{
    public class BoardTile
    {
        public BoardTile(string? id, Board board, Tile tile, UserSimple aboutUser, int position, bool isActivated)
        {
            Id = id;
            Board = board;
            Tile = tile;
            AboutUser = aboutUser;
            Position = position;
            IsActivated = isActivated;
        }

        public string? Id { get; set; }
        public Board Board { get; set; }
        public Tile Tile { get; set; }
        public UserSimple AboutUser { get; set; }
        public int Position { get; set; }
        public bool IsActivated { get; set; }
    }
}