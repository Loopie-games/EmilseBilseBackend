using System.ComponentModel.DataAnnotations;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class NewTileDto
    {
        public NewTileDto(string action, string aboutUserId)
        {
            Action = action;
            AboutUserId = aboutUserId;
        }
        [Required]
        public string Action { get; set; }
        
        [Required]
        public string AboutUserId { get; set; }
    }
}