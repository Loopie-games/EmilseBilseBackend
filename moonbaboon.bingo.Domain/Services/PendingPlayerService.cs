using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class PendingPlayerService : IPendingPlayerService
    {
        private readonly IPendingPlayerRepository _pendingPlayerRepository;

        public PendingPlayerService(IPendingPlayerRepository pendingPlayerRepository)
        {
            _pendingPlayerRepository = pendingPlayerRepository;
        }

        
    }
}