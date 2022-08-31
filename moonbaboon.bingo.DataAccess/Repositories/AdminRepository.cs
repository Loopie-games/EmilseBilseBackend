using System;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class AdminRepository: IAdminRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);

        private const string Table = DBStrings.AdminTable;

        private static string sql_select(string from)
        {
            return
                $"SELECT {Table}.{DBStrings.Id}, " +
                $"U.{DBStrings.Id}, U.{DBStrings.Username}, U.{DBStrings.Nickname}, U.{DBStrings.ProfilePic} " +
                $"FROM {from} " +
                $"JOIN {DBStrings.UserTable} As U ON U.{DBStrings.Id} = {Table}.{DBStrings.UserId} ";
        }
        
        private static Admin ReaderToEnt(MySqlDataReader reader)
        {
            var user = new UserSimple(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
            return new Admin(reader.GetValue(0).ToString(), user);
        }
        
        public async Task<Admin?> IsAdmin(User user)
        {
            Admin? ent = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(Table) + 
                $"WHERE {Table}.{DBStrings.UserId} = '{user.Id}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                ent = ReaderToEnt(reader);
            }

            await _connection.CloseAsync();
            return ent;
        }
    }
}