using System.ComponentModel.DataAnnotations;

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
}