using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class FriendRequestDto
    {
        public FriendRequestDto(string senderId, string receiverUsername)
        {
            SenderId = senderId;
            ReceiverUsername = receiverUsername;
        }

        public string SenderId { get; set; }
        public string ReceiverUsername { get; set; }
    }

    public class FriendDto
    {
        public FriendDto(string friendshipId, UserSimple friend, bool accepted)
        {
            this.friendshipId = friendshipId;
            this.friend = friend;
            this.accepted = accepted;
        }

       public string friendshipId { get; set; }
       public UserSimple friend { get; set; }
       public bool accepted { get; set; }
    }
}