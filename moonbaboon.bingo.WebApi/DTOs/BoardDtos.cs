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
}