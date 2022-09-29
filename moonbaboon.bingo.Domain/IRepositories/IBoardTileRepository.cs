using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IBoardTileRepository
    {
        /// <summary>
        ///     Searches the database for a boardtile with given id
        /// </summary>
        /// <param name="id">BoardTile Id</param>
        /// <returns>BoardTile</returns>
        public Task<BoardTile> ReadById(string id);

        public Task<BoardTileEntity> Create(BoardTileEntity toCreate);

        public Task<List<BoardTile>> FindByBoardId(string id);
        public Task<BoardTileEntity> Update(BoardTileEntity toUpdate);
        
    }
}