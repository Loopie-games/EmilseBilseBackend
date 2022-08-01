using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Core.Models
{
    public class Tile
    {
        public Tile(string? id, string userId, string action)
        {
            Id = id;
            UserId = userId;
            Action = action;
        }

        public string? Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
    }
}
