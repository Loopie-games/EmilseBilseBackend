using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IPendingPlayerRepository
    {
        public Task<PendingPlayer?> Create(PendingPlayer toCreate);
    }
}