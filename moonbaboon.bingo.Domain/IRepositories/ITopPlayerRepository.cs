using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ITopPlayerRepository
    {
        public Task<TopPlayer> Create(TopPlayer toCreate);

        public Task<List<TopPlayer>> FindTop(string gameId, int limit);
    }
}