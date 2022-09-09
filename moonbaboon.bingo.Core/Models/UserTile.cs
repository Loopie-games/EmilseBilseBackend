using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Core.Models
{
    public class UserTile: ITile
    {
        public UserTile(string? id, string action, UserSimple user, UserSimple addedByUser)
        {
            Id = id;
            Action = action;
            User = user;
            AddedByUser = addedByUser;
            AddedBy = addedByUser.Username;
        }

        public string? Id { get; set; }
        public string Action { get; set; }
        public string AddedBy { get; }

        public UserSimple User { get; set; }

        public UserSimple AddedByUser { get; set; }


        
    }
}
