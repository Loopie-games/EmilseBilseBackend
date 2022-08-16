using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IBoardService
    {
        public Board? GetById(string id);
    }
}