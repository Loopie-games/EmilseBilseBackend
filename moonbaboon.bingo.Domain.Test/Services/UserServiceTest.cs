using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Domain.IRepositories;
using moonbaboon.bingo.Domain.Services;
using Moq;
using Xunit;

namespace moonbaboon.bingo.Domain.Test.Services
{
    public class UserServiceTest
    {
        private UserService GetMock()
        {
            var mock = new Mock<IUserRepository>();
            return new UserService(mock.Object);
        }

        [Fact]
        public void IsIService()
        {
            var userService = GetMock();
            
            Assert.True(userService is IUserService service);
        }
    }
}