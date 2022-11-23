using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IBoardMemberRepository
    {
        public string Insert(BoardMemberEntity entity);
    }
}