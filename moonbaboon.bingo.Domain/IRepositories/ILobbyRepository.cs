using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ILobbyRepository
    {
        public Task<Lobby?> Create(Lobby lobbyToCreate);
        
        public Task<LobbyForUser?> FindById(string id);
    }
}