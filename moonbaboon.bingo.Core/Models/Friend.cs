using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class Friend
    {
        public Friend(string? friendshipId, User user, bool isAccepted)
        {
            FriendshipId = friendshipId;
            User = user;
            IsAccepted = isAccepted;
        }

        public Friend(MySqlDataReader reader)
        {
            FriendshipId = reader.GetString("Friendship_Id");
            User = new User(reader);
            IsAccepted = bool.TryParse(reader.GetString("Friendship_IsAccepted"), out _);
        }

        public string? FriendshipId { get; set; }
        public User User { get; set; }
        public bool IsAccepted { get; set; }
    }
}