using System;
using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connection)
        {
            _connectionFactory = connection;
        }

        public List<User> GetPlayers(string gameId)
        {
            var list = new List<User>();
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText =
                @"SELECT * FROM `User` 
JOIN BoardMember On User_id = BoardMember.BoardMember_UserId
JOIN Board ON BoardMember.BoardMember_BoardId = Board.Board_Id AND Board.Board_GameId = @gameId ";

            var param = command.CreateParameter();
            param.ParameterName = "@gameId";
            param.Value = gameId;
            command.Parameters.Add(param);
            con.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read()) list.Add(new User(reader));

            return list;
        }

        public List<User> GetBoardMembers(string boardId)
        {
            var list = new List<User>();
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText =
                @"SELECT * FROM `User` 
JOIN BoardMember On User_id = BoardMember.BoardMember_UserId And BoardMember_BoardId = @Board_Id ";

            var param = command.CreateParameter();
            param.ParameterName = "@Board_Id";
            param.Value = boardId;
            command.Parameters.Add(param);
            con.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read()) list.Add(new User(reader));

            return list;
        }

        public List<User> Search(string searchString)
        {
            var list = new List<User>();
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText = @"Select * from User WHERE User.User_Username LIKE @searchString";
            var param = command.CreateParameter();
            param.ParameterName = "@searchString";
            param.Value = searchString;
            command.Parameters.Add(param);
            using var reader = command.ExecuteReader();
            while (reader.Read()) list.Add(new User(reader));

            return list;
        }

        public User Login(string username, string password)
        {
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText = @"Select * from User 
JOIN Auth A on User.User_id = A.Auth_UserId
WHERE User_Username = @username AND  Auth_Password = @password";

            var param1 = command.CreateParameter();
            param1.ParameterName = "@username";
            param1.Value = username;
            command.Parameters.Add(param1);

            var param2 = command.CreateParameter();
            param2.ParameterName = "@password";
            param2.Value = password;
            command.Parameters.Add(param2);
            con.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) return new User(reader);

            throw new InvalidOperationException("Invalid Login");
        }

        /// <summary>
        ///     Finds User with specified id
        /// </summary>
        /// <param name="id">unique user Identification</param>
        /// <returns>A Task containing the user</returns>
        /// <exception cref="Exception">No user with given id</exception>
        public User ReadById(string id)
        {
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText = @"select * from User WHERE User_id = @id;";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            command.Parameters.Add(parameter);

            con.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new User(reader);
            }

            throw new Exception("No user found with id: " + id);
        }

        public string Insert(User entity)
        {
            entity.Id = Guid.NewGuid().ToString();

            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText = @"Insert into User VALUES (@id, @username, @nickname, @profilePic)";

            con.Open();

            var param1 = command.CreateParameter();
            param1.ParameterName = "@id";
            param1.Value = entity.Id;
            command.Parameters.Add(param1);

            var param2 = command.CreateParameter();
            param2.ParameterName = "@username";
            param2.Value = entity.Username;
            command.Parameters.Add(param2);

            var param3 = command.CreateParameter();
            param3.ParameterName = "@nickname";
            param3.Value = entity.Nickname;
            command.Parameters.Add(param3);

            var param4 = command.CreateParameter();
            param4.ParameterName = "@profilePic";
            param4.Value = entity.ProfilePicUrl;
            command.Parameters.Add(param4);

            command.ExecuteNonQuery();
            return entity.Id;
        }

        public string? GetUserIdByUsername(string username)
        {
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText =
                @"SELECT User_id FROM User WHERE Lower(User_Username) = @username";

            var paramUsername = command.CreateParameter();
            paramUsername.ParameterName = "@username";
            paramUsername.Value = username;
            command.Parameters.Add(paramUsername);

            con.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) return reader.GetString(0);

            return null;
        }

        public string GetSalt(string userId)
        {
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText =
                @"SELECT Auth_Salt FROM Auth WHERE Auth_UserId = @UserId";

            var paramUsername = command.CreateParameter();
            paramUsername.ParameterName = "@UserId";
            paramUsername.Value = userId;
            command.Parameters.Add(paramUsername);

            con.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) return reader.GetString(0);

            throw new Exception("no user with given userId");
        }

        public void UpdateUser(User entity)
        {
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText =
                @"UPDATE `User` SET `User_Username`=@Username,`User_Nickname`=@Password,`User_ProfilePicURL`= @ProfilePic WHERE User_id = @UserId";

            var param1 = command.CreateParameter();
            param1.ParameterName = "@id";
            param1.Value = entity.Id;
            command.Parameters.Add(param1);

            var param2 = command.CreateParameter();
            param2.ParameterName = "@username";
            param2.Value = entity.Username;
            command.Parameters.Add(param2);

            var param3 = command.CreateParameter();
            param3.ParameterName = "@nickname";
            param3.Value = entity.Nickname;
            command.Parameters.Add(param3);

            var param4 = command.CreateParameter();
            param4.ParameterName = "@profilePic";
            param4.Value = entity.ProfilePicUrl;
            command.Parameters.Add(param4);

            con.Open();
            command.ExecuteNonQuery();
        }
    }
}