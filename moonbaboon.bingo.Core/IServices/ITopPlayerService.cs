using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITopPlayerService
    {
        public TopPlayer Create(TopPlayer toCreate);
        List<TopPlayer> FindTop(string gameId, int limit);
    }
}