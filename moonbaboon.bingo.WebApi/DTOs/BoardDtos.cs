namespace moonbaboon.bingo.WebApi.DTOs
{
    public class GetBoardDto
    {
        public GetBoardDto(string userId, string gameId)
        {
            this.userId = userId;
            this.gameId = gameId;
        }

        public string userId { get; set; }
        public string gameId { get; set; }
    }

    public class BoardTileDto
    {
        public BoardTileDto(string id, string boardId, TileDto tile, int position, bool isActivated)
        {
            Id = id;
            BoardId = boardId;
            Tile = tile;
            Position = position;
            IsActivated = isActivated;
        }

        public string Id { get; set; }
        public string BoardId { get; set; }
        public TileDto Tile { get; set; }
        public int Position { get; set; }
        public bool IsActivated { get; set; }
    }
}