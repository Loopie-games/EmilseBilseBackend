using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class BugReportService : IBugReportService
    {
        private readonly IBugReportRepository _bugReportRepository;

        public BugReportService(IBugReportRepository bugReportRepository)
        {
            _bugReportRepository = bugReportRepository;
        }

        public List<BugReport> GetAll()
        {
            return _bugReportRepository.FindAll().Result;
        }

        BugReportEntity IBugReportService.Create(BugReportEntity bugReportEntity)
        {
            return _bugReportRepository.Create(bugReportEntity).Result;
        }

        public BugReport GetById(string id)
        {
            return _bugReportRepository.ReadById(id);
        }
    }
}