using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class BoardTileDto
    {
        public BoardTileDto(string id, string boardId, Tile tile, int position, bool isActivated)
        {
            Id = id;
            BoardId = boardId;
            Tile = tile;
            Position = position;
            IsActivated = isActivated;
        }

        public string Id { get; set; }
        public string BoardId { get; set; }
        public Tile Tile { get; set; }
        public int Position { get; set; }
        public bool IsActivated { get; set; }
    }
}