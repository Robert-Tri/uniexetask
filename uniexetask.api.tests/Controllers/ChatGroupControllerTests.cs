using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using uniexetask.api.Controllers;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;
using FluentAssertions;
using uniexetask.api.Models.Request;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using uniexetask.api.Hubs;
using Task = System.Threading.Tasks.Task;

namespace uniexetask.api.tests.Controllers
{
    public class ChatGroupControllerTests
    {
        private readonly Mock<IChatGroupService> _chatGroupServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IStudentService> _studentServiceMock;
        private readonly Mock<IMentorService> _mentorServiceMock;
        private readonly ChatGroupController _controller;
        private readonly IFixture _fixture;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHubContext<ChatHub>> _hubContextMock;

        public ChatGroupControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customize<ChatGroup>(composer => composer
            .Without(x => x.Group)
            .Without(x => x.Users)
            .Without(x => x.ChatMessages)
            .Without(x => x.Owner)
            .Without(x => x.CreatedByNavigation));
            _fixture.Customize<ChatMessage>(composer => composer
            .Without(x => x.User)
            .Without(x => x.ChatGroup));
            _fixture.Customize<User>(composer => composer
            .Without(x => x.Campus)
            .Without(x => x.ChatGroupCreatedByNavigations)
            .Without(x => x.ChatGroupOwners)
            .Without(x => x.ChatMessages)
            .Without(x => x.DocumentModifiedByNavigations)
            .Without(x => x.DocumentUploadByNavigations)
            .Without(x => x.Mentors)
            .Without(x => x.NotificationReceivers)
            .Without(x => x.NotificationSenders)
            .Without(x => x.ProjectScores)
            .Without(x => x.RefreshTokens)
            .Without(x => x.Role)
            .Without(x => x.Students)
            .Without(x => x.ChatGroups));
            _fixture.Customize<Student>(composer => composer
            .Without(x => x.GroupMembers)
            .Without(x => x.Lecturer)
            .Without(x => x.MemberScores)
            .Without(x => x.Subject)
            .Without(x => x.TaskAssigns)
            .Without(x => x.User));
            _mapperMock = new Mock<IMapper>();
            _fixture.Inject(_mapperMock);

            _studentServiceMock = new Mock<IStudentService>();
            _fixture.Inject(_studentServiceMock);

            _userServiceMock = new Mock<IUserService>();
            _fixture.Inject(_userServiceMock);

            _chatGroupServiceMock = new Mock<IChatGroupService>();
            _fixture.Inject(_chatGroupServiceMock);

            _mentorServiceMock = new Mock<IMentorService>();
            _fixture.Inject(_mentorServiceMock);

            _hubContextMock = new Mock<IHubContext<ChatHub>>();
            _fixture.Inject(_hubContextMock);
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            // Thiết lập Clients.All trả về mock của IClientProxy
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            _hubContextMock.Setup(context => context.Clients).Returns(mockClients.Object);

            // Thiết lập SendAsync trả về Task.CompletedTask để mô phỏng hành vi thành công
            mockClientProxy
                .Setup(proxy => proxy.SendCoreAsync(
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _controller = new ChatGroupController(_chatGroupServiceMock.Object, _userServiceMock.Object, _studentServiceMock.Object, _mentorServiceMock.Object, _mapperMock.Object, _hubContextMock.Object);
            // Set User Claims (mock authenticated user)
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, "1") // UserId = 1
                    }))
                }
            };
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupByUser_ShouldReturnOkResult_WithChatGroups()
        {
            // Arrange
            var chatGroupsMock = _fixture.Create<IEnumerable<ChatGroup>>();
            var userId = 1;
            _chatGroupServiceMock
                .Setup(s => s.GetChatGroupByUserId(userId, 0, 5, ""))
                .ReturnsAsync(chatGroupsMock);

            foreach (var chatGroup in chatGroupsMock)
            {
                var latestMessage = _fixture.Create<ChatMessage>();

                _chatGroupServiceMock
                    .Setup(s => s.GetLatestMessageInChatGroup(chatGroup.ChatGroupId))
                    .ReturnsAsync(latestMessage);
            }

            // Act
            var result = await _controller.GetChatGroupByUser();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<ChatGroupResponse>>;
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            _chatGroupServiceMock.Verify(x => x.GetChatGroupByUserId(userId, 0, 5, ""), Times.Once());

            foreach (var chatGroup in chatGroupsMock)
            {
                _chatGroupServiceMock.Verify(
                    x => x.GetLatestMessageInChatGroup(chatGroup.ChatGroupId),
                    Times.Once(),
                    $"GetLatestMessageInChatGroup should be called for chat group {chatGroup.ChatGroupId}"
                );
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupByUser_ShouldReturnBadRequest_WhenChatGroupsNotFound()
        {
            // Arrange
            IEnumerable<ChatGroup>? chatGroupsMock = null;
            var userId = 1;
            _chatGroupServiceMock
                .Setup(s => s.GetChatGroupByUserId(userId, 0, 5, ""))
                .ReturnsAsync(chatGroupsMock);

            // Act
            var result = await _controller.GetChatGroupByUser();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var okResult = result as BadRequestObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<ChatGroupResponse>>;
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Data.Should().BeNull();
            response.ErrorMessage.Should().NotBeNull();
            response.ErrorMessage.Should().Be("Chat group not found");
            _chatGroupServiceMock.Verify(x => x.GetChatGroupByUserId(userId, 0, 5, ""), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupByUser_ShouldReturnOkResult_WhenKeywordProvided()
        {
            // Arrange
            var chatGroupsMock = _fixture.Create<IEnumerable<ChatGroup>>();
            var userId = 1;

            var keyword = chatGroupsMock.First().ChatGroupName;

            _chatGroupServiceMock
                .Setup(s => s.GetChatGroupByUserId(userId, 0, 5, keyword))
                .ReturnsAsync(chatGroupsMock.Where(cg => cg.ChatGroupName.Contains(keyword)).ToList());

            // Act
            var result = await _controller.GetChatGroupByUser(0, 5, keyword);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<ChatGroupResponse>>;

            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data.Should().NotBeEmpty();

            foreach (var chatGroup in response.Data)
            {
                chatGroup.ChatGroup.ChatGroupName.Should().Contain(keyword);
            }

            _chatGroupServiceMock.Verify(x => x.GetChatGroupByUserId(userId, 0, 5, keyword), Times.Once());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("a")]
        public async System.Threading.Tasks.Task GetChatGroupByUser_ShouldReturnBadRequest_WhenInvalidUserId(string userId)
        {
            // Arrange
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(userId))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }
            else
            {
                claims.Clear();
            }

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            // Act
            var result = await _controller.GetChatGroupByUser();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var okResult = result as BadRequestObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<ChatGroupResponse>>;
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Data.Should().BeNull();
            response.ErrorMessage.Should().NotBeNull();
            response.ErrorMessage.Should().Be("Invalid User Id");
            _chatGroupServiceMock.Verify(x => x.GetChatGroupByUserId(It.IsAny<int>(), 0, 5, ""), Times.Never());
            _chatGroupServiceMock.Verify(x => x.GetLatestMessageInChatGroup(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetMessagesChatGroup_ShouldReturnOkResult_WhenValidData()
        {
            // Arrange
            var chatMessagesMock = _fixture.Create<IEnumerable<ChatMessage>>();
            var chatGroupId = _fixture.Create<int>();
            _chatGroupServiceMock
                .Setup(s => s.GetMessagesInChatGroup(chatGroupId, 0, 5))
                .ReturnsAsync(chatMessagesMock);

            // Act
            var result = await _controller.GetMessagesChatGroup(chatGroupId.ToString());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<ChatMessageResponse>>;
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            _chatGroupServiceMock.Verify(x => x.GetMessagesInChatGroup(chatGroupId, 0, 5), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetMessagesChatGroup_ShouldReturnBadRequest_WhenMessageNotFound()
        {
            // Arrange
            IEnumerable<ChatMessage>? chatMessagesMock = null;
            var chatGroupId = _fixture.Create<int>();
            _chatGroupServiceMock
                .Setup(s => s.GetMessagesInChatGroup(chatGroupId, 0, 5))
                .ReturnsAsync(chatMessagesMock);

            // Act
            var result = await _controller.GetMessagesChatGroup(chatGroupId.ToString());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var okResult = result as BadRequestObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<ChatMessageResponse>>;
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Data.Should().BeNull();
            response.ErrorMessage.Should().NotBeNull();
            response.ErrorMessage.Should().Be("Chat message not found");
            _chatGroupServiceMock.Verify(x => x.GetMessagesInChatGroup(chatGroupId, 0, 5), Times.Once());
        }

        [Theory]
        [InlineData(null, null, "Invalid Usser Id")]
        [InlineData("a", "a", "Invalid Usser Id")]
        [InlineData("1", null, "Invalid Usser Id")]
        [InlineData(null, "1", "Invalid Chat Group Id")]
        [InlineData("a", "1", "Invalid Chat Group Id")]
        [InlineData("1", "a", "Invalid Usser Id")]
        public async System.Threading.Tasks.Task GetMessagesChatGroup_ShouldReturnBadRequest_WithAppropriateErrorMessages(string chatGroupId, string userId, string expectedErrorMessage)
        {
            // Arrange
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(userId))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }
            else
            {
                claims.Clear();
            }

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            // Act
            var result = await _controller.GetMessagesChatGroup(chatGroupId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var okResult = result as BadRequestObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<ChatMessageResponse>>;
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Data.Should().BeNull();
            response.ErrorMessage.Should().NotBeNull();
            response.ErrorMessage.Should().Be(expectedErrorMessage);
            _chatGroupServiceMock.Verify(x => x.GetMessagesInChatGroup(It.IsAny<int>(), 0, 5), Times.Never());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetMembersInChatGroup_ShouldReturnOkResult_WhenValidData()
        {
            // Arrange
            var chatGroupsMock = _fixture.Create<ChatGroup>();
            _chatGroupServiceMock
                .Setup(s => s.GetChatGroupWithUsersByChatGroupId(chatGroupsMock.ChatGroupId))
                .ReturnsAsync(chatGroupsMock);

            // Act
            var result = await _controller.GetMembersInChatGroup(chatGroupsMock.ChatGroupId.ToString());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<MemberInChatGroupResponse>>;
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            _chatGroupServiceMock.Verify(x => x.GetChatGroupWithUsersByChatGroupId(chatGroupsMock.ChatGroupId), Times.Once());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("a")]
        public async System.Threading.Tasks.Task GetMembersInChatGroup_ShouldReturnBadRequest_WhenInvalidChatGroupId(string chatGroupId)
        {
            // Arrange

            // Act
            var result = await _controller.GetMembersInChatGroup(chatGroupId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var okResult = result as BadRequestObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<MemberInChatGroupResponse>>;
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Data.Should().BeNull();
            response.ErrorMessage.Should().NotBeNull();
            response.ErrorMessage.Should().Be("Invalid Chat Group Id");
        }

        [Fact]
        public async System.Threading.Tasks.Task GetMembersInChatGroup_ShouldReturnBadRequest_WhenChatGroupNotFound()
        {
            // Arrange
            var chatGroupId = _fixture.Create<int>();
            ChatGroup? chatGroupMock = null;
            _chatGroupServiceMock
                .Setup(s => s.GetChatGroupWithUsersByChatGroupId(chatGroupId))
                .ReturnsAsync(chatGroupMock);

            // Act
            var result = await _controller.GetMembersInChatGroup(chatGroupId.ToString());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var okResult = result as BadRequestObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<IEnumerable<MemberInChatGroupResponse>>;
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Data.Should().BeNull();
            response.ErrorMessage.Should().NotBeNull();
            response.ErrorMessage.Should().Be("Chat group not found");
            _chatGroupServiceMock.Verify(x => x.GetChatGroupWithUsersByChatGroupId(chatGroupId), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task CreatePersonalChatGroup_ShouldReturnOkResult_WhenValidData()
        {
            // Arrange
            var modal = new CreatePersonalChatGroupModal 
            {
                ContactedUserId = "1",
                ContactUserId = "2",
                Message = "Sample message"
            };
            _chatGroupServiceMock
                .Setup(s => s.CreatePersonalChatGroup(int.Parse(modal.ContactedUserId), int.Parse(modal.ContactUserId), modal.Message))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreatePersonalChatGroup(modal);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<string>;
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data.Should().Be("Message has been sent, please go to Chat session to check..");
            _chatGroupServiceMock.Verify(x => x.CreatePersonalChatGroup(int.Parse(modal.ContactedUserId), int.Parse(modal.ContactUserId), modal.Message), Times.Once());
        }

        [Theory]
        [InlineData(null, "1", "Sample Message")]
        [InlineData("1", null, "Sample Message")]
        [InlineData("1", "1", null)]
        [InlineData("1", "1", "")]
        [InlineData("a", "1", "Sample Message")]
        [InlineData("1", "a", "Sample Message")]
        [InlineData("a", "a", "Sample Message")]
        [InlineData("a", "a", "")]
        [InlineData("a", "a", null)]
        [InlineData(null, null, null)]
        public async System.Threading.Tasks.Task CreatePersonalChatGroup_ShouldReturnBadRequest_WhenInvalidData(string leaderId, string userId, string message)
        {
            // Arrange
            var modal = new CreatePersonalChatGroupModal
            {
                ContactedUserId = leaderId,
                ContactUserId = userId,
                Message = message
            };

            // Act
            var result = await _controller.CreatePersonalChatGroup(modal);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<string>;
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Data.Should().BeNull();
            response.ErrorMessage.Should().NotBeNull();
            response.ErrorMessage.Should().Be("Invalid data.");
            _chatGroupServiceMock.Verify(x => x.CreatePersonalChatGroup(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async System.Threading.Tasks.Task CreatePersonalChatGroup_ShouldReturnBadRequest_WhenServiceReturnsFalse()
        {
            // Arrange
            var modal = new CreatePersonalChatGroupModal
            {
                ContactedUserId = "1",
                ContactUserId = "2",
                Message = "Sample message"
            };
            _chatGroupServiceMock
                .Setup(s => s.CreatePersonalChatGroup(int.Parse(modal.ContactedUserId), int.Parse(modal.ContactUserId), modal.Message))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CreatePersonalChatGroup(modal);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ApiResponse<string>;
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.Data.Should().BeNull();
            response.ErrorMessage.Should().NotBeNull();
            response.ErrorMessage.Should().Be("Failed to send message.");
            _chatGroupServiceMock.Verify(x => x.CreatePersonalChatGroup(int.Parse(modal.ContactedUserId), int.Parse(modal.ContactUserId), modal.Message), Times.Once());
        }
    }
}
