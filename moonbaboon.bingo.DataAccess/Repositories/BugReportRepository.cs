using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BugReportRepository :IBugReportRepository
    {
        public Task<List<BugReport>> FindAll()
        {
            throw new System.NotImplementedException();
        }

        public Task<BugReportEntity> Create(BugReportEntity pt)
        {
            throw new System.NotImplementedException();
        }

        public Task<BugReport> ReadById(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}