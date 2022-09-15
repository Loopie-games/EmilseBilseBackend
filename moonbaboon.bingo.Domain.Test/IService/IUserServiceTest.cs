using moonbaboon.bingo.Core.IServices;
using Moq;
using Xunit;

namespace moonbaboon.bingo.Domain.Test.IService
{
    public class IUserServiceTest
    {
        private readonly Mock<IUserService> _service = new();

        [Fact]
        public void IUserService_IsAvailable()
        {
            Assert.NotNull(_service.Object);
        }
    }
}