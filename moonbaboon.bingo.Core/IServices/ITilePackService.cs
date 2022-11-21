using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITilePackService
    {
        public List<TilePack> GetAll(string? userId);
        public List<TilePack> GetOwned(string userId);
        public TilePack GetById(string id);
        public TilePack GetDefault();
        public string Create(TilePack toCreate);
        public void Update(TilePack toUpdate);
        public void Delete(string packId);
    }
}