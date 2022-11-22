using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IFriendshipService
    {
        public List<Friend> SearchUsers(string searchStr, string loggedUserId);
        public List<Friend> GetFriendsByUserId(string userId);
        public string SendFriendRequest(string fromUserId, string toUserId);
        public List<Friend> GetFriendRequestsByUserId(string userId);
        public void AcceptFriendRequest(string friendshipId, string acceptingId);
    }
}