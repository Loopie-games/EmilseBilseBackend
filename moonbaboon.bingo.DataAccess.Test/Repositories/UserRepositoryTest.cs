using System;
using System.Collections.Generic;
using System.Data;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.DataAccess.Repositories;
using moonbaboon.bingo.Domain;
using moonbaboon.bingo.Domain.Services;
using Moq;
using MySqlConnector;
using Xunit;

namespace moonbaboon.bingo.DataAccess.Test.Repositories
{
    public class UserRepositoryTest
    {
        /*
        private readonly UserRepository _userRepository = new();

        [Fact]
        public void UserRepository_IsIUserRepository()
        {
            Assert.IsAssignableFrom<IUserRepository>(_userRepository);
        }
        */
        
        [Fact]
        public void TestInsert()
        {
            //Arrange
            var commandMock = new Mock<IDbCommand>();
            commandMock
                .Setup(m => (m.ExecuteNonQuery()))
                .Verifiable();

            var paramMock = new Mock<IDbDataParameter>();
            
            commandMock
                .Setup(m => m.CreateParameter())
                .Returns(paramMock.Object);

            commandMock.Setup(m => m.Parameters.Add(paramMock));

            var connectionMock = new Mock<IDbConnection>();
            connectionMock
                .Setup(m => m.CreateCommand())
                .Returns(commandMock.Object);
            
            var connectionFactoryMock = new Mock<IDbConnectionFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateConnection())
                .Returns(connectionMock.Object);

            var repo = new UserRepository(connectionFactoryMock.Object);
            var ent = new User("81d7f01e-57c9-4151-b31b-86881192dbf0","Demetra","dpawlicki0", null);
            
            //Act
            repo.Insert(ent);
            
            //Assert
            commandMock.Verify();
            
        }
        
        [Fact]
        public void TestReadById()
        {
            //Arrange
            var id = "81d7f01e-57c9-4151-b31b-86881192dbf0";
            var username = "Demetra";
            var nickname = "dpawlicki0";
            string? profilePic = null;
            var expected = new User("81d7f01e-57c9-4151-b31b-86881192dbf0","Demetra","dpawlicki0", null);

            
            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(_ => _.Read())
                .Returns(true)
                .Returns(false);
            
            readerMock.Setup(reader => reader.GetOrdinal("User_Id")).Returns(0);
            readerMock.Setup(reader => reader.GetOrdinal("User_Username")).Returns(1);
            readerMock.Setup(reader => reader.GetOrdinal("User_Nickname")).Returns(2);
            readerMock.Setup(reader => reader.GetOrdinal("User_ProfilePicUrl")).Returns(3);
            
            readerMock.Setup(reader => reader.GetString(0)).Returns(id);
            readerMock.Setup(reader => reader.GetString(1)).Returns(username);
            readerMock.Setup(reader => reader.GetString(2)).Returns(nickname);
            readerMock.Setup(reader => reader.GetValue(3).ToString()).Returns(profilePic);

            
            var commandMock = new Mock<IDbCommand>();
            

            var paramMock = new Mock<IDbDataParameter>();
            
            commandMock
                .Setup(m => m.CreateParameter())
                .Returns(paramMock.Object).Verifiable();

            commandMock.Setup(m => m.Parameters.Add(paramMock));

            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object);

            var connectionMock = new Mock<IDbConnection>();
            connectionMock
                .Setup(m => m.CreateCommand())
                .Returns(commandMock.Object);
            
            var connectionFactoryMock = new Mock<IDbConnectionFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateConnection())
                .Returns(connectionMock.Object);

            var repo = new UserRepository(connectionFactoryMock.Object);
            
            
            //Act
            var result = repo.ReadById(id);
            
            //Assert
            commandMock.Verify();
            Assert.Equal(expected, result, new UserComparer());
        }
    }

    public class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id && x.Username == y.Username && x.Nickname == y.Nickname && x.ProfilePicUrl == y.ProfilePicUrl;
        }

        public int GetHashCode(User obj)
        {
            return HashCode.Combine(obj.Id, obj.Username, obj.Nickname, obj.ProfilePicUrl);
        }
    }
}