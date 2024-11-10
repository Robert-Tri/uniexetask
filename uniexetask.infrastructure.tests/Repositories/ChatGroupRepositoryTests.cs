using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;
using uniexetask.infrastructure.Repositories;

namespace uniexetask.infrastructure.tests.Repositories
{
    public class ChatGroupRepositoryTests
    {
/*        private readonly ChatGroupRepository _repository;
        private readonly Mock<UniExetaskContext> _mockDbContext;
        private readonly DbSet<ChatGroup> _mockChatGroups;

        public ChatGroupRepositoryTests()
        {
            _mockChatGroups = MockDbSet<ChatGroup>(data);
            _mockDbContext = new Mock<UniExetaskContext>();
            _mockDbContext.Setup(db => db.ChatGroups).Returns(_mockChatGroups);

            _repository = new ChatGroupRepository(_mockDbContext.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupWithUsersByChatGroupIdAsync_ShouldReturnChatGroup_WhenGroupFound()
        {
            // Arrange
            var chatGroupId = 1;

            // Act
            var result = await _repository.GetChatGroupWithUsersByChatGroupIdAsync(chatGroupId);

            // Assert
            result.Should().NotBeNull();
            result.ChatGroupId.Should().Be(chatGroupId);
            result.Users.Should().NotBeEmpty();
            result.Users.Should().ContainSingle()
                  .Which.FullName.Should().Be("User 1");
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupWithUsersByChatGroupIdAsync_ShouldReturnNull_WhenGroupNotFound()
        {
            // Arrange
            var chatGroupId = 999; // Id does not exist

            // Act
            var result = await _repository.GetChatGroupWithUsersByChatGroupIdAsync(chatGroupId);

            // Assert
            result.Should().BeNull();
        }

        private static DbSet<T> MockDbSet<T>(IQueryable<T> sourceList) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(sourceList.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(sourceList.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(sourceList.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(sourceList.GetEnumerator());
            return mockSet.Object;
        }*/
    }
}
