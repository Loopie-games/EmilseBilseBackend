﻿using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IBugReportRepository
    {
        public List<BugReport> FindAll(string adminId);
        public Task<BugReportEntity> Create(BugReportEntity entity);
        Task<BugReport> ReadById(string id, string adminId);
        Task AddStar(StarredBugReportEntity entity);
        void RemoveStar(string starId, string? adminId);
    }
}