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
        public NewTileDto(string addedByUserId, string action, string aboutUserName)
        {
            AddedByUserId = addedByUserId;
            Action = action;
            AboutUserName = aboutUserName;
        }

        public string AddedByUserId { get; set; }
        public string Action { get; set; }
        public string AboutUserName { get; set; }
    }
}