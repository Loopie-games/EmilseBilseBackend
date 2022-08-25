using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IGameRepository
    {
        public Task<Game?> FindById(string id);

        public Task<Game?> Create(string hostId);
        public Task<List<UserSimple>> GetPlayers(string gameId);
    }
}