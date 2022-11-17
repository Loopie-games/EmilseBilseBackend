using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class BugReportEntity
    {
        public BugReportEntity(string? id, string? userId, string title, string? description)
        {
            Id = id;
            UserId = userId;
            Title = title;
            Description = description;
        }

        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
    }

    public class BugReport
    {
        public BugReport(MySqlDataReader reader)
        {
            Id = reader.GetString("BugReport_Id");
            Title = reader.GetString("BugReport_Title");
            Description = reader.GetValue("BugReport_Description").ToString();
            ReportingUser = new UserSimple(reader);
            StarId = reader.GetValue("StarredBugReport_Id").ToString();
        }

        public string? Id { get; set; }
        public UserSimple? ReportingUser { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? StarId { get; set; }
    }

    public class StarredBugReportEntity
    {
        public StarredBugReportEntity(string? id, string adminId, string bugReportId)
        {
            Id = id;
            AdminId = adminId;
            BugReportId = bugReportId;
        }

        public string? Id { get; set; }
        public string AdminId { get; set; }
        public string BugReportId { get; set; }
    }
}