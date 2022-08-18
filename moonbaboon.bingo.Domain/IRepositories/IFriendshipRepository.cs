using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IFriendshipRepository
    {
        public Task<List<Friendship>> FindAll();
        public Task<List<Friendship>> FindAcceptedFriendshipsByUserId(string userId);
        public Task<bool> ValidateFriendship(string userId1, string userId2);
        public Task<Friendship?> Create(string fromUserId, string toUserId, bool b);
        public Task<List<Friendship>> FindFriendRequests_ByUserId(string userId);
        public Task<Friendship?> FindById(string id);
        public Task<Friendship?> AcceptFriendship(string friendshipId);
    }
}