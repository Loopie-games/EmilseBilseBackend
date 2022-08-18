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
}