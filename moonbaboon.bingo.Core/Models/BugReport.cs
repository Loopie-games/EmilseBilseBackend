namespace moonbaboon.bingo.Core.Models
{
    public class BugReport
    {
        public BugReport(string? id, string? userId, string title, string? description)
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
}