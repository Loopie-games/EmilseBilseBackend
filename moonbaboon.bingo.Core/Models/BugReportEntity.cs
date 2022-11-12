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
        public BugReport(string? id, UserSimple? user, string title, string? description)
        {
            Id = id;
            User = user;
            Title = title;
            Description = description;
        }

        public BugReport(MySqlDataReader reader)
        {
            Id = reader.GetString("BugReport_Id");
            Title = reader.GetString("BugReport_Id");
            Description = reader.GetString("BugReport_Id");
            User = new UserSimple(reader);
        }

        public string? Id { get; set; }
        public UserSimple? User { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}