using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IFriendshipService
    {
        public List<Friendship> GetAll();
        public List<Friend> GetFriendsByUserId(string userId);
        public Friend? SendFriendRequest(string fromUserId, string toUserId);
        public List<Friend> GetFriendRequestsByUserId(string userId);
        public Friend AcceptFriendRequest(string friendshipId, string value);
    }
}