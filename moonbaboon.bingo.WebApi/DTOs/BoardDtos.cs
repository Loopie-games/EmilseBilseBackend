using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class BoardTileDto
    {
        public BoardTileDto(string id, string boardId, UserTile userTile, int position, bool isActivated)
        {
            Id = id;
            BoardId = boardId;
            UserTile = userTile;
            Position = position;
            IsActivated = isActivated;
        }

        public string Id { get; set; }
        public string BoardId { get; set; }
        public UserTile UserTile { get; set; }
        public int Position { get; set; }
        public bool IsActivated { get; set; }
    }
}