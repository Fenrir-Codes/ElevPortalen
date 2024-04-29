using ElevPortalen.Data;
using Microsoft.AspNetCore.DataProtection;
using ElevPortalen.Models;
using Microsoft.EntityFrameworkCore;

namespace ElevPortalen.Services
{
    public class MessageService
    {
        private readonly ElevPortalenDataDbContext _context;
        private readonly IDataProtector? _dataProtector;

        #region constructor
        public MessageService(ElevPortalenDataDbContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _dataProtector = dataProtectionProvider.CreateProtector("ProtectData");
        }
        #endregion

        #region Create Message (send)
        public async Task<(string, bool)> SendMessage(MessageModel message)
        {
            try
            {
                _context.Messages.Add(message); // Add input to context variables
                await _context.SaveChangesAsync(); // Save

                return ($"Message sent.", true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error har ocurred: {ex.Message}");
            }
        }
        #endregion

        #region Delete Message with the messageId
        public async Task<string> Delete(int Id)
        {
            try
            {
                var message = await _context.Messages.FindAsync(Id);
                if (message != null)
                {

                    //var messageToRemove = _context.Messages.FirstOrDefaultAsync(m => m.MessageId == message.MessageId);
                    var messageToRemove = _context.Messages.Local.FirstOrDefault(s => s.MessageId == message.MessageId);

                    if (messageToRemove != null)
                    {
                        _context.Entry(messageToRemove).State = EntityState.Detached;
                    }

                    _context.Messages.Remove(message);
                    await _context.SaveChangesAsync();

                    return "Message Deleted.";
                }
                else
                {
                    return $"Messasge not found with MessageId - {Id}.";
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error has occurred: {ex.Message}");
            }
        }
        #endregion

        #region Delete Messages with the receiverId
        public async Task<string> DeleteAllWithReceiverId(int receiverId)
        {
            try
            {
                var messages = await _context.Messages.Where(m => m.ReceiverId == receiverId).ToListAsync();

                if (messages.Any())
                {
                    _context.Messages.RemoveRange(messages);
                    await _context.SaveChangesAsync();
                    return "Messages Deleted.";
                }
                else
                {
                    return $"No messages found with ReceiverId - {receiverId}.";
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error has occurred: {ex.Message}");
            }
        }
        #endregion

        #region Mark Message as Read
        public async Task MarkMessageAsRead(int messageId) {
            try {
                var message = await _context.Messages.FirstOrDefaultAsync(m => m.MessageId == messageId) ?? throw new InvalidOperationException($"Message with ID {messageId} does not exist.");
                message.IsRead = true;
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new InvalidOperationException("An error occurred while marking the message as read: " + ex.Message);
            }
        }
        #endregion

        #region Get Message with ReceiverId
        public async Task<List<MessageModel>> GetMessageWithReceiverId(int Id) {
            try {
                var messages = await _context.Messages.Where(m => m.ReceiverId == Id).ToListAsync();

                if (messages == null || messages.Count == 0) // Check if no messages are found
                {
                    // Throw an exception if no messages found
                    throw new InvalidOperationException($"No message found with ReceiverId: {Id}");
                }

                return messages;

            } catch (Exception ex) {
                throw new InvalidOperationException($"An error occurred while fetching messages: {ex.Message}");
            }
        }
        #endregion


        #region Count Unread Messages
        public async Task<int> GetUnredMessageCount(int Id)
        {
            try
            {
                int unreadMessageCount = await _context.Messages.Where(m => m.ReceiverId == Id && !m.IsRead).CountAsync();

                return unreadMessageCount;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while counting unread messages." + ex.Message);
            }
        }
        #endregion
    }

}
