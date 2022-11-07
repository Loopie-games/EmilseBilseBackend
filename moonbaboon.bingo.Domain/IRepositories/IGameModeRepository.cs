using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IGameModeRepository
    {
        public Task<List<GameMode>> FindAll();
    }
}