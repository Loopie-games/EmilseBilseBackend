using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IGameService
    {
        public Game? GetById(string id);
        
        public Game? Create(string hostId);
        public Game? NewGame(Lobby lobby);
        public List<UserSimple> GetPlayers(string gameId);
    }
}