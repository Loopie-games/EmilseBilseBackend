using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IFriendshipRepository
    {
        public Task<List<Friendship>> FindAll();
        public Task<List<Friend>> FindAcceptedFriendshipsByUserId(string userId);
        public Task<bool> ValidateFriendship(string userId1, string userId2);
    }
}