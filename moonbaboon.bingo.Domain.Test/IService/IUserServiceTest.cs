using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using Moq;
using Xunit;

namespace moonbaboon.bingo.Domain.Test.IService
{
    public class IUserServiceTest
    {
        private readonly Mock<IUserService> _service = new Mock<IUserService>();

        [Fact]
        public void IUserService_IsAvailable()
        {
            Assert.NotNull(_service.Object);
        }
    }
}