using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITilePackService
    {
        public List<TilePack> GetAll();
        public TilePack GetById(string id);
        public TilePack GetDefault();
        public TilePack Create(TilePack toCreate);
        
    }
}