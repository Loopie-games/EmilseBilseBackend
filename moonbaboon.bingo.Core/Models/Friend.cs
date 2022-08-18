namespace moonbaboon.bingo.Core.Models
{
    public class Friend
    {
        public Friend(string? id, UserSimple user, bool accepted)
        {
            Id = id;
            User = user;
            Accepted = accepted;
        }

        public string? Id { get; set; }
        public UserSimple User { get; set; }
        public bool Accepted { get; set; }
    }
}