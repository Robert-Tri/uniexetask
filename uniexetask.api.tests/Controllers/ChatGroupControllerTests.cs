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

namespace uniexetask.api.tests.Controllers
{
    public class ChatGroupControllerTests
    {
        private readonly Mock<IChatGroupService> _chatGroupServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly ChatGroupController _controller;
        private readonly IFixture _fixture;

        public ChatGroupControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customize<ChatGroup>(composer => composer
            .Without(x => x.Users)
            .Without(x => x.ChatMessages)
            .Without(x => x.Owner)
            .Without(x => x.CreatedByNavigation));
            _fixture.Customize<ChatMessage>(composer => composer
            .Without(x => x.User)
            .Without(x => x.ChatGroup));
            _chatGroupServiceMock = _fixture.Freeze<Mock<IChatGroupService>>();
            _userServiceMock = _fixture.Freeze<Mock<IUserService>>();

            _controller = new ChatGroupController(_chatGroupServiceMock.Object, _userServiceMock.Object);
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

        [Fact]
        public async System.Threading.Tasks.Task GetMessagesChatGroup_ShouldReturnOkResult_WhenValidChatGroupId()
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
        public async System.Threading.Tasks.Task GetMessagesChatGroup_ShouldReturnBadRequest_WhenInvalidChatGroupId()
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

        [Fact]
        public async System.Threading.Tasks.Task GetMembersInChatGroup_ShouldReturnOkResult_WhenValidChatGroupId()
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

        [Fact]
        public async System.Threading.Tasks.Task GetMembersInChatGroup_ShouldReturnBadRequest_WhenInvalidChatGroupId()
        {
            // Arrange
            string? chatGroupId = null;

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
    }
}
