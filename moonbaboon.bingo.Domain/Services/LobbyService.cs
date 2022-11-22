using System;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IPendingPlayerRepository _pendingPlayerRepository;
        private readonly IUserRepository _userRepository;

        public LobbyService(ILobbyRepository lobbyRepository, IPendingPlayerRepository pendingPlayerRepository,
            IUserRepository userRepository)
        {
            _lobbyRepository = lobbyRepository;
            _pendingPlayerRepository = pendingPlayerRepository;
            _userRepository = userRepository;
        }

        public Lobby GetById(string id)
        {
            return _lobbyRepository.FindById(id).Result;
        }

        public Lobby? Create(string hostId)
        {
            var lobby = GetByHostId(hostId);

            //if user is already host for a lobby, close the old one
            if (lobby?.Id is not null) CloseLobby(lobby.Id, hostId);
            var lobbyNew = _lobbyRepository.Create(new Lobby(null, hostId, null)).Result;
            return _lobbyRepository.FindById(lobbyNew).Result;
        }


        public PendingPlayer JoinLobby(string userId, string pin)
        {
            try
            {
                Lobby lobby = _lobbyRepository.FindByPin(pin).Result;
                var pp = _pendingPlayerRepository.IsPlayerInLobby(userId).Result;
                if (pp is not null && pp.Lobby.Id == lobby.Id) return pp;
                if (pp is not null && pp.Lobby.Id != lobby.Id)
                {
                    _pendingPlayerRepository.Update(pp).Wait();
                    return _pendingPlayerRepository.ReadById(pp.Id).Result;
                }

                //if user already is in the lobby the PendingPlayer is returned, else a new is created
                var newP = _pendingPlayerRepository.Create(new PendingPlayerEntity(null, userId, lobby.Id)).Result;
                return _pendingPlayerRepository.ReadById(newP).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void CloseLobby(string lobbyId, string hostId)
        {
            var lobby = _lobbyRepository.FindById(lobbyId).Result;
            if (lobby.Host != hostId) throw new Exception("you Cannot Delete a lobby you are not host for");
            _pendingPlayerRepository.DeleteWithLobbyId(lobbyId).Wait();
            _lobbyRepository.DeleteLobby(lobbyId).Wait();
        }

        public void LeaveLobby(string userId)
        {
            var pp = _pendingPlayerRepository.IsPlayerInLobby(userId).Result;
            if (pp is not null) _pendingPlayerRepository.Delete(pp.Id).Wait();
        }

        public Lobby? GetByHostId(string hostId)
        {
            return _lobbyRepository.FindByHostId(hostId).Result;
        }
    }
}