using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly MySqlConnection _connection;

        public AdminRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<Admin?> FindByUserId(string userId)
        {
            Admin? ent = null;
            await using var con = _connection.Clone();
            con.Open();
            await using MySqlCommand command = new(
                @"SELECT * FROM Admin
JOIN User on User_id = Admin.Admin_UserId
WHERE Admin_UserId = @UserId"
                , con);
            {
                command.Parameters.Add("@User_Id", MySqlDbType.VarChar).Value = userId;
            }
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = new Admin(reader);
            return ent;
        }
    }
}