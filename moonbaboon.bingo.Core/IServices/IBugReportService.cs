using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IBugReportService
    {
        public List<BugReport> GetAll(string userId);

        public BugReportEntity Create(BugReportEntity bugReportEntity);
        public BugReport GetById(string id, string userId);
        public void AddStar(string userId, string bugReportId);
    }
}