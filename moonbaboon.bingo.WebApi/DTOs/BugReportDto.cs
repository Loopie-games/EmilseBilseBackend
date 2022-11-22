namespace moonbaboon.bingo.WebApi.DTOs
{
    public class UserBugReportDto
    {
        public UserBugReportDto(string title, string? description)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; set; }
        public string? Description { get; set; }
    }
}