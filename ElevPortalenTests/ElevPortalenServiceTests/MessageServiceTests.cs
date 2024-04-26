using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevPortalen.Data;
using ElevPortalen.Models;
using ElevPortalen.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ElevPortalenTests.ElevPortalenServiceTests {
    public class MessageServiceTests {

        private readonly DbContextOptions<ElevPortalenDataDbContext> _options;
        private readonly ElevPortalenDataDbContext _context;
        private readonly MessageService _messageService;

        public MessageServiceTests() {
            _options = new DbContextOptionsBuilder<ElevPortalenDataDbContext>()
                .UseInMemoryDatabase(databaseName: "MessageServiceTests")
                .Options;

            _context = new ElevPortalenDataDbContext(_options);

            // Mock dependencies
            var dataProtectionProviderMock = new Mock<IDataProtectionProvider>();

            // Create MessageService instance with mocked dependencies
            _messageService = new MessageService(_context, dataProtectionProviderMock.Object);
        }

        #region SendMessage test1 - Create Message (send) - Function should return success
        [Fact]
        public async void SendMessage_ShouldReturnSuccess_WhenMessageModelIsCorrect() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            var message = new MessageModel {
                ReceiverId = 1,
                SenderName = "Sender",
                Subject = "Test Subject",
                Content = "Test Content",
                Timestamp = DateTime.Now,
                IsRead = false
            };

            // Act
            var (resultMessage, isSuccess) = await _messageService.SendMessage(message);

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("Message sent.", resultMessage);
        }
        #endregion

        #region SendMessage test2 - Create Message (send) - throw exception on null message
        [Fact]
        public async Task SendMessage_ShouldThrowException_WhenMessageModelIsNull() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            MessageModel invalidMessage = null; // Passing a null message model intentionally

            // Act and assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _messageService.SendMessage(invalidMessage));
        }
        #endregion

        #region Delete Message with the messageId - Function should delete message
        [Fact]
        public async Task Delete_ShouldDeleteMessage_WhenMessageExists() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var message = new MessageModel {
                MessageId = 1,
                ReceiverId = 2,
                SenderName = "Sender",
                Subject = "Test Subject",
                Content = "Test Content",
                Timestamp = DateTime.Now,
                IsRead = false
            };
            //_context.Messages.Add(message);
            //await _context.SaveChangesAsync();

            // Act
            //var result = await _messageService.Delete(message.MessageId);
            var (resultMessage, isSuccess) = await _messageService.SendMessage(message);

            // Assert

            Assert.True(isSuccess);
            Assert.Equal("Message sent.", resultMessage);
            // Check if message with ID 1 is created
            var createdMessage = await _context.Messages.FindAsync(1);
            Assert.NotNull(createdMessage); // Assert that a message with ID 1 is found
            Assert.Equal(1, createdMessage.MessageId); // Assert that the message has ID equal to 1

            //there was an issue getting this method to confirm that it deletes, hence above cluster
            //continuing, we want to revisiting the act and assert part to confirm deletion works

            // Act
            var result = await _messageService.Delete(1);

            // Assert
            Assert.Equal("Message Deleted.", result);
            Assert.Null(await _context.Messages.FindAsync(1)); // Check if message with ID 1 is deleted



        }

        #endregion


        #region Delete Message with the messageId - Function should return message not found
        [Fact]
        public async Task Delete_ShouldReturnMessageNotFound_WhenMessageDoesNotExist() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear

            // Act
            var result = await _messageService.Delete(1); // Assuming message with ID 1 does not exist

            // Assert
            Assert.Equal("Messasge not found with MessageId - 1.", result);
        }

        #endregion





        #region
        #endregion

        #region TEMPLATE
        [Fact]
        public async void Method() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            //_context.Database.CloseConnection();

            //ACT


            //ASSERT

        }
        #endregion



    } //MessageServiceTests class bracket end
}
