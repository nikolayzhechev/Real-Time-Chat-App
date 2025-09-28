using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.Authentication;
using RealTimeChatApp.Data;
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.Models;
using RealTimeChatApp.Models.DTOs;
using System;
using System.IO;

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

            var chat = new Chat
            {
                Name = chatRequest.Title,
                ChatUsers = chatRequest.ParticipantIds.Select(userId => new ChatUser
                {
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow
                }).ToList(),
                CreatedAt = DateTime.UtcNow
            };

            _dBcontext.Chats.Add(chat);
            await _dBcontext.SaveChangesAsync();

            var chatDto = new ChatDTO
            {
                Id = chat.Id,
                IsGroup = false,
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

            string BuildDisplayName(Chat chat)
            {
                // 1:1 chat: show the other participant's name
                if (!chat.IsGroup)
                {
                    var otherUser = chat.ChatUsers
                        .FirstOrDefault(cu => cu.UserId != userId)?.User;

                    if (otherUser == null)
                        return "Unknown User";

                    return otherUser.Username
                        ?? otherUser.Email
                        ?? "Unknown User";
                }

                // Group chat
                if (!string.IsNullOrWhiteSpace(chat.Name))
                    return chat.Name;

                var others = chat.ChatUsers
                    .Where(cu => cu.UserId != userId)
                    .Select(cu => cu.User.Username ?? cu.User.Email ?? "Unknown")
                    .OrderBy(n => n)
                    .ToList();

                if (others.Count == 0)
                    return "Group";

                var head = string.Join(", ", others.Take(3));
                var extra = others.Count > 3 ? $" +{others.Count - 3}" : "";
                return head + extra;
            }

            var chatDtos = chats.Select(chat => new ChatDTO
            {
                Id = chat.Id,
                Name = BuildDisplayName(chat), 
                IsGroup = chat.IsGroup,
                CreatedAt = chat.CreatedAt,
                ParticipantUsernames =
                    chat.ChatUsers
                        .Select(cu => cu.User.Username)
                        .ToList(),
                // chat.ChatUsers.Where(u => u.ChatId == chat.Id).Select(u => u.User.Username).ToList(),
                Messages = chat.Messages.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SentAt = m.SentAt
                }).ToList(),
            }).ToList();

            return chatDtos;
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
    }
}
