using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private const string Table = DbStrings.AdminTable;
        private readonly MySqlConnection _connection;

        public AdminRepository(MySqlConnection connection)
        {
            _connection = connection.Clone();
        }

        public async Task<Admin?> IsAdmin(string userId)
        {
            Admin? ent = null;
            await using var con = _connection.Clone();
            con.Open();
            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.UserId} = '{userId}'"
                , con);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);
            return ent;
        }

        private static string sql_select(string from)
        {
            return
                $"SELECT {Table}.{DbStrings.Id}, " +
                $"U.{DbStrings.Id}, U.{DbStrings.Username}, U.{DbStrings.Nickname}, U.{DbStrings.ProfilePic} " +
                $"FROM {from} " +
                $"JOIN {DbStrings.UserTable} As U ON U.{DbStrings.Id} = {Table}.{DbStrings.UserId} ";
        }

        private static Admin ReaderToEnt(MySqlDataReader reader)
        {
            var user = new UserSimple(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
            return new Admin(reader.GetValue(0).ToString(), user);
        }
    }
}