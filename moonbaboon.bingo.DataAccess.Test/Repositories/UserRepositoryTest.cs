using moonbaboon.bingo.DataAccess.Repositories;
using moonbaboon.bingo.Domain.IRepositories;
using Xunit;

namespace moonbaboon.bingo.DataAccess.Test.Repositories
{
    public class UserRepositoryTest
    {
        private readonly UserRepository _userRepository = new();

        [Fact]
        public void UserRepository_IsIUserRepository()
        {
            Assert.IsAssignableFrom<IUserRepository>(_userRepository);
        }
    }
}