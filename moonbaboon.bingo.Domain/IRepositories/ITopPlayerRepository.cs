using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ITopPlayerRepository
    {
        public Task<string> Create(TopPlayerEntity entity);

        public Task<List<TopPlayer>> FindTop(string gameId, int limit);
    }
}