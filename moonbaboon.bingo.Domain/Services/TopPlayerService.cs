using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class TopPlayerService : ITopPlayerService
    {
        private readonly ITopPlayerRepository _topPlayerRepository;

        public TopPlayerService(ITopPlayerRepository topPlayerRepository)
        {
            _topPlayerRepository = topPlayerRepository;
        }

        public TopPlayer Create(TopPlayer toCreate)
        {
            return _topPlayerRepository.Create(toCreate).Result;
        }
    }
}