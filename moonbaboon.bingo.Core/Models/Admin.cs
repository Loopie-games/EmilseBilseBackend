using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class Admin
    {
        public Admin(MySqlDataReader reader)
        {
            Id = reader.GetString("Admin_Id");
            User = new User(reader);
        }

        public string? Id { get; set; }
        public User User { get; set; }
    }
}