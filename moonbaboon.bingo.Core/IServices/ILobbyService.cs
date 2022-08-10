using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ILobbyService
    {
        public Lobby? GetById(string id);
        
        public Lobby? Create(Lobby lobbyToCreate);
    }
}