using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IGameService
    {
        public Game? GetById(string id);
    }
}