namespace moonbaboon.bingo.Core.Models
{
    public class FriendshipEntity
    {
        public FriendshipEntity(string? id, string friendId1, string friendId2, bool accepted)
        {
            Id = id;
            UserId1 = friendId1;
            UserId2 = friendId2;
            Accepted = accepted;
        }

        public string? Id { get; set; }
        public string UserId1 { get; set; }
        public string UserId2 { get; set; }
        public bool Accepted { get; set; }
    }
}