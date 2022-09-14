﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.SignalR
{
    [Authorize]
    public class LobbyHub: Hub
    {
        private readonly IPendingPlayerService _pendingPlayerService;
        private readonly ILobbyService _lobbyService;

        public LobbyHub(ILobbyService lobbyService, IPendingPlayerService pendingPlayerService)
        {
            _lobbyService = lobbyService;
            _pendingPlayerService = pendingPlayerService;
        }
        
        /// <summary>
        /// Send en Error message to the client
        /// </summary>
        /// <param name="message">Error Message</param>
        private async Task SendError(string message)
        {
            await Clients.Caller.SendAsync("PopUpError", message);
        }

        private static string GetUserId(HubCallerContext context)
        {
            if (context.User == null) throw new Exception("Could not get user from Context");
            return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   throw new InvalidOperationException("Could not get userId from Context");
        }
        
        /// <summary>
        /// Adds Authorized user to lobby and sends updates to clients
        /// </summary>
        /// <param name="pin">Pin for lobby</param>
        /// <exception cref="Exception">If User cant be added.</exception>
        public async Task JoinLobby(string pin)
        {
            try
            {
                var pp = _lobbyService.JoinLobby(GetUserId(Context), pin);
                await Groups.AddToGroupAsync(Context.ConnectionId, pp.Lobby.Id!);
                await Clients.Caller.SendAsync("receiveLobby", pp.Lobby);
                await UpdatePlayerList(pp.Lobby.Id);
            }
            catch (Exception e)
            {
                await SendError(e.Message);
                throw;
            }
        }

        public async Task LeaveLobby()
        {
            try
            {
                var lobbyId = _pendingPlayerService.GetByUserId(GetUserId(Context)).Lobby.Id;
                if (lobbyId is not null && _lobbyService.LeaveLobby(GetUserId(Context)))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
                    await UpdatePlayerList(lobbyId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SendError(e.Message);
            }
        }

        private async Task UpdatePlayerList(string lobbyId)
        {
            List<PendingPlayerDto> playerList = _pendingPlayerService.GetByLobbyId(lobbyId)
                .Select(player => new PendingPlayerDto(player)).ToList();
            await Clients.Group(lobbyId).SendAsync("playerList", playerList);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                LeaveLobby();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}