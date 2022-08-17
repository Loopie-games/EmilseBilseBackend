using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Core.Models
{
    public class Tile
    {
        public Tile(UserSimple user, string action, UserSimple? addedBy)
        {
            User = user;
            Action = action;
            AddedBy = addedBy;
        }

        public string? Id { get; set; }
        public UserSimple User { get; set; }
        public string Action { get; set; }
        
        public UserSimple? AddedBy { get; set; }
    }
}
