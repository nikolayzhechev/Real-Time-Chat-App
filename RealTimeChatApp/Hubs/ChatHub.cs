using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RealTimeChatApp.Data;
using RealTimeChatApp.Models;
using RealTimeChatApp.Models.DTOs;

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

            var senderId = int.Parse(Context.UserIdentifier);

            var sender = new AppUser
            {
                Id = senderId,
                Username = username,
            };

            var message = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = messageContent,
                SentAt = DateTime.UtcNow
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            var messageDto = new MessageDTO
            {
                Username = sender.Username,
                Content = message.Content,
                SentAt = message.SentAt,
                Attachments = message.Attachments?.Select(a => new AttachmentDTO
                {
                    FileName = a.FileName,
                    FileType = a.FileType,
                    FileUrl = a.FileUrl
                }).ToList()
            };

            try
            {
                // Broadcast to clients in the chat
                await AddToGroup(chatId.ToString());
                await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", messageDto);

                _logger.LogInformation("New message received from {User}: {Message}", username, messageContent);
            }
            catch (Exception ex)
            {
                throw new HubException("An unexpected error occurred while sending your message.", ex);
            }
        }

        public async Task SendLocationMessage(int chatId, string username, double latitude, double longitude)
        {
            var senderId = int.Parse(Context.UserIdentifier);

            var sender = new AppUser
            {
                Id = senderId,
                Username = username,
            };

            var message = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = $"Latitude: {latitude}, Longitude: {longitude}",
                SentAt = DateTime.UtcNow,
                Latitude = latitude,
                Longitude = longitude
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            var messageDto = new MessageDTO
            {
                Username = sender.Username,
                Content = message.Content,
                SentAt = message.SentAt,
                Attachments = message.Attachments?.Select(a => new AttachmentDTO
                {
                    FileName = a.FileName,
                    FileType = a.FileType,
                    FileUrl = a.FileUrl
                }).ToList(),
                Latitude = latitude,
                Longitude = longitude
            };

            try
            {
                // Broadcast to clients in the chat
                await AddToGroup(chatId.ToString());
                await Clients.Group(chatId.ToString()).SendAsync("ReceiveLocationMessage", messageDto);

                _logger.LogInformation("New location message received from {User}: {Message}", username, message.Content);
            }
            catch (Exception ex)
            {
                throw new HubException("An unexpected error occurred while sending your location message.", ex);
            }
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}
