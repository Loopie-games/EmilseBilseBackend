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

        public LobbyService(ILobbyRepository lobbyRepository, IPendingPlayerRepository pendingPlayerRepository, IUserRepository userRepository)
        {
            _lobbyRepository = lobbyRepository;
            _pendingPlayerRepository = pendingPlayerRepository;
            _userRepository = userRepository;
        }

        public Lobby GetById(string id)
        {
            return _lobbyRepository.FindById(id).Result;
        }

        public Lobby? GetByHostId(string hostId)
        {
            return _lobbyRepository.FindByHostId(hostId).Result;
        }

        public Lobby? Create(string hostId)
        {
            var lobby = GetByHostId(hostId);

            //if user is already host for a lobby, close the old one
            if (lobby?.Id is not null)
            {
                CloseLobby(lobby.Id, hostId);
            }
            var lobbyNew = _lobbyRepository.Create(new Lobby(null,hostId, null)).Result;
            return lobbyNew;
        }

        
        public PendingPlayer JoinLobby(string userId, string pin)
        {
            try
            {
                User user = _userRepository.ReadById(userId).Result;
                Lobby lobby = _lobbyRepository.FindByPin(pin).Result;
                var pp = _pendingPlayerRepository.IsPlayerInLobby(userId, lobby.Id!).Result;
                
                //if user already is in the lobby the PendingPlayer is returned, else a new is created
                return pp ?? _pendingPlayerRepository.Create(new PendingPlayer(new UserSimple(user), lobby)).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public bool CloseLobby(string lobbyId, string hostId)
        {
            var lobby = _lobbyRepository.FindById(lobbyId).Result;
            if (lobby.Host != hostId) throw new Exception("you Cannot Delete a lobby you are not host for");
            return _pendingPlayerRepository.DeleteWithLobbyId(lobbyId).Result && _lobbyRepository.DeleteLobby(lobbyId).Result;
        }

        public bool LeaveLobby(string lobbyId, string userId)
        {
            var pp = _pendingPlayerRepository.IsPlayerInLobby(userId, lobbyId).Result;
            if (pp != null)
            {
                if(_lobbyRepository.FindById(lobbyId).Result?.Host == userId)
                {
                    return CloseLobby(lobbyId, userId);
                }
                return _pendingPlayerRepository.Delete(pp.Id).Result;
            }

            return false;
        }
    }
}