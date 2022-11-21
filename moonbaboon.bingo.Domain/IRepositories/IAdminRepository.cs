using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IAdminRepository
    {
        public Task<Admin?> FindByUserId(string userId);
    }
}