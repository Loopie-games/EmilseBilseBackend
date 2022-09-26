using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITilePackService
    {
        public List<TilePack> GetAll(string? userId);
        public TilePack GetById(string id);
        public TilePack GetDefault();
        public TilePack Create(TilePack toCreate);
        public void Update(TilePack toUpdate);
        public void Delete(string packId);
    }
}