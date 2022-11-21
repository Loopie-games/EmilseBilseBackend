using System;
using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class BugReportService : IBugReportService
    {
        private readonly IBugReportRepository _bugReportRepository;
        private readonly IAdminRepository _adminRepository;

        public BugReportService(IBugReportRepository bugReportRepository, IAdminRepository adminRepository)
        {
            _bugReportRepository = bugReportRepository;
            _adminRepository = adminRepository;
        }

        public List<BugReport> GetAll(string userId)
        {
            var admin = _adminRepository.FindByUserId(userId).Result;

            if (admin?.Id != null) return _bugReportRepository.FindAll(admin.Id).Result;
            throw new Exception("U need to be an Admin to excess this");
        }

        BugReportEntity IBugReportService.Create(BugReportEntity bugReportEntity)
        {
            return _bugReportRepository.Create(bugReportEntity).Result;
        }

        public BugReport GetById(string id, string userId)
        {
            var admin = _adminRepository.FindByUserId(userId).Result;
            if (admin?.Id != null) return _bugReportRepository.ReadById(id, admin.Id).Result;
            throw new Exception("U need to be an Admin to excess this");
        }

        public void AddStar(string userId, string bugReportId)
        {
            
            
            var admin = _adminRepository.FindByUserId(userId).Result;

            _bugReportRepository.AddStar(new StarredBugReportEntity(null, admin.Id, bugReportId)).Wait();
        }
    }
}