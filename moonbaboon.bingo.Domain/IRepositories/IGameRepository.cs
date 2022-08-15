using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IGameRepository
    {
        public Task<Game?> FindById(string id);
    }
}