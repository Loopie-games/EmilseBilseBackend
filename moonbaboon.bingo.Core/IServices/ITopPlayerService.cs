using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITopPlayerService
    {
        List<TopPlayer> FindTop(string gameId, int limit);
    }
}