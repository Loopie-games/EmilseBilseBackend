using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITilePackService
    {
        public List<TilePack> GetAll();

        public TilePack GetDefault();
    }
}