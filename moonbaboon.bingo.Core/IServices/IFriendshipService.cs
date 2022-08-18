using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IFriendshipService
    {
        public List<Friendship> GetAll();
        public List<Friend> GetFriendsByUserId(string userId);
        public Friendship? SendFriendRequest(string fromUserId, string toUserId);
        public List<Friendship> GetFriendRequestsByUserId(string userId);
        public Friendship? AcceptFriendRequest(string friendshipId, string value);
    }
}