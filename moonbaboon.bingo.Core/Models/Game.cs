namespace moonbaboon.bingo.Core.Models
{
    public class Game{
        public Game(string? id, UserSimple host, UserSimple? winner, State state)
        {
            Id = id;
            Host = host;
            Winner = winner;
            State = state;
        }

        public string? Id { get; set; }
        public UserSimple Host { get; set; }
        public UserSimple? Winner { get; set; }
        
        public State State { get; set; }
    }

    public enum State
    {
        Ongoing, Paused, Ended
    }
}