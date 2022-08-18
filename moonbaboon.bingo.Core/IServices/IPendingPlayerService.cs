using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IPendingPlayerService
    {
        public List<PendingPlayer> GetByLobbyId(string lobbyId);

    }
}