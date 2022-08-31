namespace moonbaboon.bingo.Core.Models
{
    public class Admin
    {
        public Admin(string? id, UserSimple user)
        {
            Id = id;
            User = user;
        }

        public string? Id { get; set; }
        public UserSimple User { get; set; }
    }
}