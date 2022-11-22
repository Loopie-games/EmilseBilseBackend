using System;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MySqlConnection _connection;


        public AuthRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<string> Create(AuthEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "INSERT INTO Auth VALUES (@Id,@UserId,@Password,@salt);",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = entity.Id;
                    command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = entity.UserId;
                    command.Parameters.Add("@Password", MySqlDbType.VarChar).Value = entity.Password;
                    command.Parameters.Add("@salt", MySqlDbType.VarChar).Value = entity.Salt;
                }
                command.ExecuteNonQuery();
            }
            return entity.Id;
        }
    }
}