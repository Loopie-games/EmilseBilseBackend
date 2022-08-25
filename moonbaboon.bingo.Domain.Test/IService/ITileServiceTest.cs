using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace moonbaboon.bingo.Domain.Test.IService
{
    public class ITileServiceTest
    {
        private readonly Mock<IUserTileService> _service = new Mock<IUserTileService>();

        [Fact]
        public void ITileService_IsAvailable()
        {
            Assert.NotNull(_service.Object);
        }

        [Fact]
        public void GetAllTiles()
        {
            List<UserTile> fakeTiles = new List<UserTile>();
            _service.Setup(service => service.GetAll())
                .Returns(fakeTiles);
            Assert.Equal(fakeTiles, _service.Object.GetAll());
        }
        
        /*

        [Fact]
        public void GetTileById_NotNullIfFound()
        {
            string tileId = Guid.NewGuid().ToString();
            Tile t = new Tile(Guid.NewGuid().ToString(), "test");
            _service.Setup(service => service.GetById(tileId))
                .Returns(t);
            Assert.NotNull(_service.Object.GetById(tileId));
        }

        [Fact]
        public void GetTileById_NullIfNotFound()
        {
            string tileId = Guid.NewGuid().ToString();

            string wrongTileId = Guid.NewGuid().ToString();

            Tile t = new Tile(Guid.NewGuid().ToString(), "Test");
            _service.Setup(service => service.GetById(tileId))
                .Returns(t);
            Assert.Null(_service.Object.GetById(wrongTileId));
        }

        [Fact]
        public void GetTileById_WithId()
        {
            string tileId = Guid.NewGuid().ToString();
            Tile testTile = new Tile(Guid.NewGuid().ToString(), "Test");
            _service.Setup(service => service.GetById(tileId))
                .Returns(testTile);
            Assert.Equal(testTile, _service.Object.GetById(tileId));
        }
        */
    }
}
