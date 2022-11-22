using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IFriendshipRepository
    {
        public Task<List<Friend>> FindFriendsByUserId(string userId);
        public Task<bool> ValidateFriendship(string userId1, string userId2);
        public Task<string> Create(FriendshipEntity entity);
        public Task<List<Friend>> FindFriendRequests_ByUserId(string userId);
        public Task AcceptFriendship(string friendshipId, string acceptingUserId);
        public Task<List<Friend>> SearchUsers(string searchStr, string? loggedUserId);
    }
}