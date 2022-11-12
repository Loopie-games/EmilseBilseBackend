using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IBugReportService
    {
        public List<BugReport> GetAll();

        public BugReportEntity Create(BugReportEntity bugReportEntity);
        public BugReport GetById(string id);
    }
}