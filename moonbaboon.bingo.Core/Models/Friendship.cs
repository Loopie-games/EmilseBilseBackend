namespace moonbaboon.bingo.Core.Models
{
    public class Friendship
    {
        public Friendship(string? id, UserSimple friendId1, UserSimple friendId2, bool accepted)
        {
            Id = id;
            FriendId1 = friendId1;
            FriendId2 = friendId2;
            Accepted = accepted;
        }

        public string? Id { get; set; }
        public UserSimple FriendId1 { get; set; }
        public UserSimple FriendId2 { get; set; }
        public bool Accepted { get; set; } 

    }
    
}