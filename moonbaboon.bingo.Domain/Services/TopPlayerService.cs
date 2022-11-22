using System;
using System.Collections.Generic;
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

        public List<TopPlayer> FindTop(string gameId, int limit)
        {
            try
            {
                return _topPlayerRepository.FindTop(gameId, limit).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            ;
        }
    }
}