using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.api.Controllers;
using uniexetask.services.Interfaces;

namespace uniexetask.api.tests.Controllers
{
    class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IRoleService> _roleServiceMock;
        private readonly Mock<IRolePermissionService> _rolePermissionServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _roleServiceMock = new Mock<IRoleService>();
            _rolePermissionServiceMock = new Mock<IRolePermissionService>();
            _configurationMock = new Mock<IConfiguration>();

            _controller = new AuthController(
                _configurationMock.Object,
                _authServiceMock.Object,
                _roleServiceMock.Object,
                _rolePermissionServiceMock.Object
            );
        }
    }
}
