namespace moonbaboon.bingo.Core.Models
{
    public class AuthEntity
    {
        public AuthEntity(string? id, string userId, string password, string salt)
        {
            Id = id;
            UserId = userId;
            Password = password;
            Salt = salt;
        }

        public string? Id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}