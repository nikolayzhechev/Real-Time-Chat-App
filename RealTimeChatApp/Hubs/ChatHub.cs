using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace RealTimeChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        // broadcasts received messages to all connected users once the server receives them
        // method to invoke from the client
        public async Task NewMessage(string username, string message)
        {
            _logger.LogInformation("New message received from {User}: {Message}", username, message);

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new HubException("Message cannot be empty.");
            }

            if (username.Length > 20)
            {
                throw new HubException("Username is too long.");
            }

            try
            {
            await Clients.All.SendAsync("messageReceived", username, message);
                
            }
            catch (Exception ex)
            {
                throw new HubException("An unexpected error occurred while sending your message.", ex);
            }
        }
    }
}
