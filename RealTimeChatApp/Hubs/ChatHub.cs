using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RealTimeChatApp.Data;
using RealTimeChatApp.Models;

namespace RealTimeChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly AppDbContext _dbContext;

        public ChatHub(ILogger<ChatHub> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task SendMessage(int chatId, string username, string messageContent)
        {
            if (string.IsNullOrWhiteSpace(messageContent))
            {
                throw new HubException("Message cannot be empty.");
            }

            var senderId = Context.UserIdentifier;

            var message = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = messageContent,
                SentAt = DateTime.UtcNow
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            try
            {
                // Broadcast to clients in the chat
                await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", new
                {
                    ChatId = chatId,
                    SenderId = senderId,
                    Content = messageContent,
                    SentAt = DateTime.UtcNow
                });
                _logger.LogInformation("New message received from {User}: {Message}", username, message);
            }
            catch (Exception ex)
            {
                throw new HubException("An unexpected error occurred while sending your message.", ex);
            }
        }
    }
}
