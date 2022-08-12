﻿using System.ComponentModel.DataAnnotations;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class CreateLobbyDto
    {
        public CreateLobbyDto(string hostId)
        {
            HostId = hostId;
        }
    
        [Required]
        public string HostId { get; set; }
        
    }

    public class CloseLobbyDto
    {
        public CloseLobbyDto(string lobbyId, string hostId)
        {
            LobbyId = lobbyId;
            HostId = hostId;
        }

        [Required]
        public string LobbyId { get; set; }
        [Required]
        public string HostId { get; set; }

        
    }
    
    public class LeaveLobbyDto
    {
        public LeaveLobbyDto(string lobbyId, string userId)
        {
            LobbyId = lobbyId;
            UserId = userId;
        }

        [Required]
        public string LobbyId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}