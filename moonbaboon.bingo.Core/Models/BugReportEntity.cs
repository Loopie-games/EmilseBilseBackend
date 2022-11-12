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
        public BugReport(string? id, UserSimple? reportingUser, string title, string? description)
        {
            Id = id;
            ReportingUser = reportingUser;
            Title = title;
            Description = description;
        }

        public BugReport(MySqlDataReader reader)
        {
            Id = reader.GetString("BugReport_Id");
            Title = reader.GetString("BugReport_Title");
            Description = reader.GetString("BugReport_Description");
            ReportingUser = new UserSimple(reader);
        }

        public string? Id { get; set; }
        public UserSimple? ReportingUser { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}