namespace moonbaboon.bingo.Core.Models
{
    public class Game{
        public Game(UserSimple host)
        {
            Host = host;
        }

        public string? Id { get; set; }
        public UserSimple Host { get; set; }
        public string? WinnerId { get; set; }
    }
}