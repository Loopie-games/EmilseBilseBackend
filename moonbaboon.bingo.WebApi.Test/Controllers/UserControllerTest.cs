using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.WebApi.Controllers;
using Moq;
using Xunit;

namespace moonbaboon.bingo.WebApi.Test.Controllers
{
    public class UserControllerTest
    {
        private readonly UserController _userController;
        private readonly Mock<IUserService> _userService = new();

        public UserControllerTest()
        {
            _userController = new UserController(_userService.Object);
        }

        [Fact]
        public void UserController_IsOfTypeControllerBase()
        {
            Assert.IsAssignableFrom<ControllerBase>(_userController);
        }

        private static Attribute? GetAttributeTypeFromName(string name)
        {
            return typeof(UserController)
                .GetTypeInfo().GetCustomAttributes()
                .FirstOrDefault(a => a.GetType().Name.Equals(name));
        }

        [Fact]
        public void UserController_UsesApiControllerAttribute()
        {
            Assert.NotNull(GetAttributeTypeFromName("ApiControllerAttribute"));
        }

        [Fact]
        public void UserController_UsesRouteAttribute()
        {
            Assert.NotNull(GetAttributeTypeFromName("RouteAttribute"));
        }

        private static MethodInfo? GetMethodInfoFromName(string name)
        {
            return typeof(UserController)
                .GetMethods().FirstOrDefault(m => name.Equals(m.Name));
        }

        [Fact]
        public void UserController_HasGetAllMethod()
        {
            Assert.NotNull(GetMethodInfoFromName("GetAll"));
        }
    }
}