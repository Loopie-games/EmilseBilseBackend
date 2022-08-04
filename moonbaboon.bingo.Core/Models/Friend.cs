namespace moonbaboon.bingo.Core.Models
{
    public class Friend
    {
        public Friend(string username, string nickname)
        {
            Username = username;
            Nickname = nickname;
        }

        public string? Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
    }
}