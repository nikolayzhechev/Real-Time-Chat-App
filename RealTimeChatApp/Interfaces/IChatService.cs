using RealTimeChatApp.Models.DTOs;

namespace RealTimeChatApp.Interfaces
{
    public interface IChatService
    {
        public Task<ChatDTO> CreateChat(CreateChatRequestDTO chatRequest);

        public Task<List<ChatDTO>> GetMyUserChats(int userId);

        public string SetDirectKey(CreateChatRequestDTO chatRequest);
    }
}
