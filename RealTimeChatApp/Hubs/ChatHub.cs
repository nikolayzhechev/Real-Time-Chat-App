using Microsoft.AspNetCore.SignalR;

namespace RealTimeChatApp.Hubs
{
    public class ChatHub : Hub
    {
        // broadcasts received messages to all connected users once the server receives them
        public async Task NewMessage(long username, string message) =>
            await Clients.All.SendAsync("messageReceived", username, message);
    }
}
