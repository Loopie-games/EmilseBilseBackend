using System.ComponentModel.DataAnnotations;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class CreateResponse
    {
        public string Id;
        public string AddedByUsername;
        public string Action;
        public string AboutUsername;

        public CreateResponse(string addedByUsername, string action, string aboutUsername, string id)
        {
            Action = action;
            AboutUsername = aboutUsername;
            Id = id;
            AddedByUsername = addedByUsername;
        }
    }

    public class TileDto
    {
        public TileDto(string id, UserDtos.UserSimpleDto user, string action)
        {
            Id = id;
            User = user;
            Action = action;
        }

        public TileDto(Tile tile)
        {
            Id = tile.Id;
            User = new UserDtos.UserSimpleDto(tile.User);
            Action = tile.Action;
            AddedBy = new UserDtos.UserSimpleDto(tile.AddedBy);
        }


        public string Id { get; set; }
        public UserDtos.UserSimpleDto User { get; set; }
        public string Action { get; set; }
        
        public UserDtos.UserSimpleDto? AddedBy { get; set; }
    }
    
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