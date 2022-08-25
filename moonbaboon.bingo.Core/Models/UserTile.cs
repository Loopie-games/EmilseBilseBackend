using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Core.Models
{
    public class UserTile
    {
        public UserTile(Tile tile, UserSimple user, UserSimple? addedBy)
        {
            Tile = tile;
            User = user;
            AddedBy = addedBy;
        }

        public Tile Tile { get; set; }
        public UserSimple User { get; set; }

        public UserSimple? AddedBy { get; set; }
    }
}
