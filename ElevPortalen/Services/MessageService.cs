using ElevPortalen.Data;
using ElevPortalen.Models;
using Microsoft.EntityFrameworkCore;

namespace ElevPortalen.Services
{
    /// <summary>
    ///  Lavet af Jozsef
    /// </summary>
    public class MessageService
    {
        private readonly ElevPortalenDataDbContext _context;

        #region constructor
        public MessageService(ElevPortalenDataDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Create Message (send)
        public async Task<(string?, bool)> SendMessage(MessageModel message)
        {
            try
            {
                _context.Messages.Add(message); // Add input to context variables
                await _context.SaveChangesAsync(); // Save

                return ($"Message sent.", true);
            }
            catch (Exception ex)
            {
                return ($"An error har ocurred: {ex.Message}", false);
            }
        }
        #endregion

        #region Delete Message with the messageId
        public async Task<(bool, string)> Delete(int Id)
        {
            try
            {
                var message = _context.Messages.FirstOrDefault(m => m.MessageId == Id);
                if (message != null)
                {
                    _context.Messages.Remove(message);
                    await _context.SaveChangesAsync();
                    return (true, "Message Deleted.");
                }
                else
                {
                    return (false, $"Error: message could not be deleted - messageId : {Id}.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
        #endregion

        #region Delete multiple Messages with the receiverId
        public async Task<(bool, string)> DeleteAllWithReceiverId(int receiverId)
        {
            try
            {
                var messages = await _context.Messages.Where(m => m.ReceiverId == receiverId).ToListAsync();

                if (messages.Any())
                {
                    _context.Messages.RemoveRange(messages);
                    await _context.SaveChangesAsync();
                    return (true, "Messages Deleted.");
                }
                else
                {
                    return (false, $"No messages found with ReceiverId - {receiverId}.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"An error has occurred: {ex.Message}");
            }
        }
        #endregion

        #region Mark Message as Read
        public async Task<(bool, string?)> MarkMessageAsRead(int messageId)
        {

            var message = await _context.Messages.FirstOrDefaultAsync(m => m.MessageId == messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
                return (true, null);
            }
            else
            {
                return (false, $"An error while marking the message as readed.");
            }
        }
        #endregion

        #region Get Message with ReceiverId
        public async Task<List<MessageModel>> GetMessageWithReceiverId(int Id)
        {
            try
            {
                var message = await _context.Messages.Where(m => m.ReceiverId == Id).ToListAsync();

                if (message != null)
                {
                    return message;
                }
                else
                {
                    // Throw an exception if no student found
                    throw new InvalidOperationException($"No message found with Id: {Id}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"{ex.Message}");
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
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion
    }

}
