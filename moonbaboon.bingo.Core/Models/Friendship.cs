namespace moonbaboon.bingo.Core.Models
{
    public class Friendship
    {
        public Friendship(string friendId1, string friendId2)
        {
            FriendId1 = friendId1;
            FriendId2 = friendId2;
        }

        public string? Id { get; set; }
        public string FriendId1 { get; set; }
        public string FriendId2 { get; set; }
        public bool Accepted { get; set; } = false;

    }
}