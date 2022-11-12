using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IBugReportRepository
    {
        public Task<List<BugReport>> FindAll();
        public Task<BugReportEntity> Create(BugReportEntity entity);
        Task<BugReport> ReadById(string id);
    }
}