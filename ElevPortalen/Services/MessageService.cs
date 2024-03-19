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

        #region Create Message
        public async Task<string> SaveMessage(MessageModel message)
        {
            try
            {
                _context.Messages.Add(message); // Add input to context variables
                await _context.SaveChangesAsync(); // Save

                return $"Message sent.";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error har ocurred: {ex.Message}");
            }
        }
        #endregion

        #region Delete Message
        public async Task<string> Delete(int Id)
        {
            try
            {
                var message = await _context.Messages.FindAsync(Id);
                if (message != null)
                {

                    var messageToRemove = _context.Messages.FirstOrDefaultAsync(m => m.MessageId == message.MessageId);
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

        #region Get Message with ID
        public async Task<List<MessageModel>> GetMessageById(int Id)
        {
            try
            {
                var message = await _context.Messages.Where(m => m.MessageId == Id).ToListAsync();

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
                throw;
            }
        }
        #endregion
    }

}
