using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.Data;
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.Models;
using RealTimeChatApp.Models.DTOs;

namespace RealTimeChatApp.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _dBcontext;

        public ChatService(AppDbContext dbContext)
        {
            _dBcontext = dbContext;
        }

        public async Task<ChatDTO> CreateChat(CreateChatRequestDTO chatRequest)
        {
            var directKey = SetDirectKey(chatRequest);

            var currentUsers = _dBcontext.AppUsers.Where(u => chatRequest.ParticipantIds.Contains(u.Id)).ToList();

            var chat = new Chat
            {
                Name = chatRequest.Title,
                ChatUsers = currentUsers.Select(cu => new ChatUser
                {
                    UserId = cu.Id,
                    JoinedAt = DateTime.UtcNow
                }).ToList(),
                IsGroup = chatRequest.IsGroup,
                CreatedAt = DateTime.UtcNow
            };

            _dBcontext.Chats.Add(chat);
            await _dBcontext.SaveChangesAsync();

            var chatDto = new ChatDTO
            {
                Id = chat.Id,
                IsGroup = chat.IsGroup,
                Participants = currentUsers
                        .Select(cu => new ChatUser
                        {
                            UserId = cu.Id,
                            User = new AppUser
                            {
                                Id = cu.Id,
                                Username = cu.Username,
                                Email = cu.Email
                            }
                        })
                        .ToList(),
                DirectKey = directKey,
                Name = chatRequest.Title,
                CreatedAt = chat.CreatedAt,
                Messages = new List<MessageDTO>()
            };

            return chatDto;
        }

        public async Task<List<ChatDTO>> GetMyUserChats(int userId)
        {
            var chats = await _dBcontext.Chats
                //.AsNoTracking()
                .Where(chat => chat.ChatUsers.Any(user => user.UserId == userId))
                .Include(chat => chat.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .Include(chat => chat.Messages) // preload messages
                .ToListAsync();

            var chatDto = chats.Select(chat => new ChatDTO
            {
                Id = chat.Id,
                Name = chat.Name,
                IsGroup = chat.IsGroup,
                CreatedAt = chat.CreatedAt,
                Participants =
                    chat.ChatUsers
                        .Select(cu => new ChatUser
                        {
                            UserId = cu.UserId,
                            User = new AppUser
                            {
                                Id = cu.User.Id,
                                Username = cu.User.Username,
                                Email = cu.User.Email
                            },
                            JoinedAt = cu.JoinedAt
                        })
                        .ToList(),
                Messages = chat.Messages.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SentAt = m.SentAt
                }).ToList(),
            }).ToList();

            return chatDto;
        }

        public async Task<ChatDTO> GetChat(int chatId)
        {
            var chat = await _dBcontext.Chats
                .Include(c => c.ChatUsers)
                    .ThenInclude(c => c.User)
                .Include(chat => chat.Messages)
                //.Include(c => c.Messages)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
            {
                return null;
            }

            var chatDto = CreateChatDTO(chat);

            return chatDto;
        }

        public string SetDirectKey(CreateChatRequestDTO chatRequest)
        {
            var chatUser = _dBcontext.AppUsers.FirstOrDefault(u => u.Id == chatRequest.ParticipantIds[0]);
            var userToChatWith = _dBcontext.AppUsers.FirstOrDefault(u => u.Id == chatRequest.ParticipantIds[1]);

            var currentUserId = chatUser.Id;
            var otherUserId = userToChatWith.Id;

            var a = string.CompareOrdinal(currentUserId.ToString(), otherUserId.ToString()) < 0
                 ? currentUserId : otherUserId;
            var b = ReferenceEquals(a, currentUserId) ? otherUserId : currentUserId;
            var directKey = $"{a}|{b}";

            return directKey;
        }

        public ChatDTO CreateChatDTO(Chat chat)
        {
            var chatDto = new ChatDTO
            {
                Id = chat.Id,
                Name = chat.Name,
                IsGroup = chat.IsGroup,
                Messages = chat.Messages
                   .OrderBy(m => m.SentAt)
                   .Select(m => new MessageDTO
                   {
                       SenderId = m.SenderId,
                       Username = _dBcontext.AppUsers.Where(u => u.Id == m.SenderId).Select(u => u.Username).FirstOrDefault(),
                       Content = m.Content,
                       SentAt = m.SentAt
                   })
                   .ToList(),
                CreatedAt = chat.CreatedAt,
                Participants = chat.ChatUsers.Select(cu => new ChatUser
                {
                    UserId = cu.UserId,
                    User = new AppUser
                    {
                        Id = cu.User.Id,
                        Username = cu.User.Username,
                        Email = cu.User.Email
                    },
                    JoinedAt = cu.JoinedAt
                }).ToList()
            };

            return chatDto;
        }
    }
}
