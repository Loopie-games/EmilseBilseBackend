using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Core.Models
{
    public class Tile
    {
        public Tile(string userId, string action)
        {
            UserId = userId;
            Action = action;
        }

        public string? Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        
        public string? AddedById { get; set; }
    }

    public class TileForUser
    {
        public TileForUser(string userNickname, string action)
        {
            UserNickname = userNickname;
            Action = action;
        }

        public string? Id { get; set; }
        public string UserNickname { get; set; }
        public string Action { get; set; }
        
        public string? AddedByNickname { get; set; }
    }

    public class TileNewFromUser
    {
        public TileNewFromUser(string addedByUserId, string action, string aboutUserName)
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
