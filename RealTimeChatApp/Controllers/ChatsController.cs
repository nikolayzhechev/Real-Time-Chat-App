using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.Data;
using RealTimeChatApp.Models;
using RealTimeChatApp.Models.DTOs;

namespace RealTimeChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly AppDbContext _dBcontext;

        public ChatsController(AppDbContext dBcontext)
        {
            _dBcontext = dBcontext;
        }

        // GET: api/Chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            return await _dBcontext.Chats.ToListAsync();
        }

        // GET: api/Chats/5
        [HttpGet("chat/{id}")]
        public async Task<ActionResult<ChatDTO>> GetChat(int id)
        {
            var chat = await _dBcontext.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chat == null)
            {
                return NotFound();
            }

            var chatDto = new ChatDTO
            {
                Id = chat.Id,
                Name = chat.Name,
                Messages = chat.Messages
                    .OrderBy(m => m.SentAt)
                    .Select(m => new MessageDTO
                    {
                        SenderId = m.SenderId,
                        Content = m.Content,
                        SentAt = m.SentAt
                    })
                    .ToList(),
                CreatedAt = chat.CreatedAt
            };

            return Ok(chatDto);
        }

        [HttpGet("chats/{userId}")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetAllUserChats(int userId)
        {
            if (!_dBcontext.AppUsers.Any(u => u.Id == userId))
                return NotFound(userId);

            try
            {
                var chats = await _dBcontext.Chats
                   .Where(chat => chat.ChatUsers.Any(user => user.UserId == userId))
                   .Include(chat => chat.ChatUsers)
                       .ThenInclude(cu => cu.User)
                   .Include(chat => chat.Messages) // preload messages
                   .ToListAsync();

                var chatDtos = chats.Select(chat => new ChatDTO
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    CreatedAt = chat.CreatedAt,
                    ParticipantUsernames = chat.ChatUsers.Select(u => u.User.Username).ToList(),
                    Messages = chat.Messages.Select(m => new MessageDTO
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        Content = m.Content,
                        SentAt = m.SentAt
                    }).ToList(),
                }).ToList();

                return Ok(chatDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Chats/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChat(int id, Chat chat)
        {
            if (id != chat.Id)
            {
                return BadRequest();
            }

            _dBcontext.Entry(chat).State = EntityState.Modified;

            try
            {
                await _dBcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/createChat
        [HttpPost("chat")]
        public async Task<ActionResult<ChatDTO>> CreateChat([FromBody] CreateChatRequestDTO chatRequest)
        {
            if (chatRequest.ParticipantIds == null || chatRequest.ParticipantIds.Count < 2)
            {
                return BadRequest("At least two participants are required.");
            }

            var chatUser = _dBcontext.AppUsers.FirstOrDefault(u => u.Id == chatRequest.ParticipantIds[0]);

            var chat = new Chat
            {
                Name = chatRequest.Title ?? $"Chat with {chatUser.Username}",
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
                Name = chat.Name,
                CreatedAt = chat.CreatedAt,
                Messages = new List<MessageDTO>()
            };

            return Ok(chatDto);
        }

        // DELETE: api/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var chat = await _dBcontext.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound(new { message = "Chat is not found", status = 404 });
            }

            _dBcontext.Chats.Remove(chat);
            await _dBcontext.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatExists(int id)
        {
            return _dBcontext.Chats.Any(e => e.Id == id);
        }
    }
}
