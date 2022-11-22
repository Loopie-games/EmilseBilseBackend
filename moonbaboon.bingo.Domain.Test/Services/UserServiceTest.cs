using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
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
        
        private List<User> GetSampleUsers()
        {
            return new List<User>()
            {
                new User("f0b4943f-c452-43fb-a6ec-9989f9b94b81", "Merci", "mcelez3", null),
                new User("181756a3-12ee-4a26-901c-a620dad2a158", "Darnell", "dtackett0", null),
                new User("9a54d77b-6b5e-4f4b-9e3b-42e21facb6ea", "Dix", "dstuchburie0", null)
            };
        }

        [Fact]
        public void IsIService()
        {
            var userService = GetMock();
            
            Assert.True(userService is IUserService service);
        }
    }
}