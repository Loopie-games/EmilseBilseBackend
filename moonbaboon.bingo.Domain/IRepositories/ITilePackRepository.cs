using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ITilePackRepository
    {
        public Task<List<TilePack>> FindAll();
        public Task<List<TilePack>> FindAll_LoggedUser(string userId);
        public Task<List<TilePack>> GetOwnedTilePacks(string userId);
        public Task<TilePack> FindDefault();
        public Task<TilePack> FindById(string packId);
        public Task<TilePack> Create(TilePack toCreate);
        public Task Update(TilePack toUpdate);
        public Task Delete(string id);
    }
}