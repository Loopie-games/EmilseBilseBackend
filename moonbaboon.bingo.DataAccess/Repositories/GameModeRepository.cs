using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class GameModeRepository : IGameModeRepository
    {
        public Task<List<GameMode>> FindAll()
        {
            throw new System.NotImplementedException();
        }
    }
}