using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IPendingPlayerRepository
    {
        public Task<PendingPlayer?> Create(PendingPlayer toCreate);
        
        public Task<PendingPlayer?> GetByUserId(string userId);

        public Task<List<PendingPlayerForUser>> GetByLobbyId(string lobbyId);

        public Task<PendingPlayer?> IsPlayerInLobby(string userId, string lobbyId);
    }
}