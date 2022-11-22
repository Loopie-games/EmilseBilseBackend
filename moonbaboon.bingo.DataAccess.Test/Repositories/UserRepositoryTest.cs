using System.Data;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.DataAccess.Repositories;
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
        public void Insert()
        {
            /*
            //Arrange
            var commandMock = new Mock<MySqlCommand>();
            commandMock
                .Setup(m => m.ExecuteNonQuery())
                .Verifiable();

            var connectionMock = new Mock<MySqlConnection>();
            connectionMock
                .Setup(m => m.CreateCommand())
                .Returns(commandMock.Object);

            var repo = new UserRepository(connectionMock.Object);
            var ent = new User("81d7f01e-57c9-4151-b31b-86881192dbf0","Demetra","dpawlicki0", null);
            
            //Act
            repo.Insert(ent);
            
            //Assert
            commandMock.Verify();
            */
        }
    }
}