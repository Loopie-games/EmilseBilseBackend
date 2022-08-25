using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Core.Models
{
    public class UserTile: ITile
    {
        public UserTile(string? id, string action, UserSimple user, UserSimple? addedBy)
        {
            Id = id;
            Action = action;
            User = user;
            AddedBy = addedBy;
        }

        public string? Id { get; set; }
        public string Action { get; set; }
        
        public UserSimple User { get; set; }

        public UserSimple? AddedBy { get; set; }


        
    }
}
