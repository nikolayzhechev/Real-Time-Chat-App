using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.Data;
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.Models;
using RealTimeChatApp.Models.DTOs;

namespace RealTimeChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly AppDbContext _dBcontext;
        private readonly IChatService _chatService;

        public ChatsController(AppDbContext dBcontext, IChatService chatService)
        {
            _dBcontext = dBcontext;
            _chatService = chatService;
        }

        // GET: api/chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            var chats = await _dBcontext.Chats.AsNoTracking().ToListAsync();

            return Ok(chats);
        }

        // GET: api/chat/5
        [HttpGet("chat/{id}")]
        public async Task<ActionResult<ChatDTO>> GetChat(int id)
        {
            ChatDTO chatDto = await _chatService.GetChat(id);

            if (chatDto == null)
            {
                return NotFound(new { message = "Chat is not found", status = 404 });
            }

            return Ok(chatDto);
        }

        // PATCH: api/chat/2
        [HttpPatch("chat/{id}")]
        public async Task<IActionResult> PatchChatTitle(int id, [FromBody] string newTitle)
        {
            var chat = await _dBcontext.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound(new { message = "Chat is not found", status = 404 });
            }

            chat.Name = newTitle;
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

        // PATH: api/chats/5/users
        [HttpPatch("chat/{chatId}/users")]
        public async Task<ActionResult<ChatDTO>> UpdateChatUsers(
            int chatId,
            [FromQuery] string action,
            [FromBody] List<int> userIds)
        {
            var chat = await _dBcontext.Chats
                .Include(c => c.ChatUsers)
                    .ThenInclude(c => c.User)
                .Include(chat => chat.Messages)
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
            {
                return NotFound(new { message = "Chat is not found", status = 404 });
            }

            var existingUserIds = chat.ChatUsers.Select(cu => cu.UserId).ToHashSet();

            if (action.ToLower() == "add")
            {
                var newUserIds = userIds.Except(existingUserIds).ToList();

                foreach (var userId in newUserIds)
                {
                    var user = await _dBcontext.AppUsers.FindAsync(userId);
                    chat.ChatUsers.Add(new ChatUser
                    {
                        ChatId = chatId,
                        UserId = userId,
                        User = user,
                        JoinedAt = DateTime.UtcNow
                    });
                }
            }
            else if (action.ToLower() == "remove")
            {
                foreach (var user in existingUserIds.Intersect(userIds).ToList())
                {
                    var chatUser = chat.ChatUsers.FirstOrDefault(cu => cu.UserId == user);
                    if (chatUser != null)
                    {
                        chat.ChatUsers.Remove(chatUser);
                    }
                }
            }
            else
            {
                return BadRequest("Invalid action. Use 'add' or 'remove'.");
            }

            if (chat.ChatUsers.Count > 2)
            {
                chat.IsGroup = true;
            }
            else if (chat.ChatUsers.Count() == 2)
            {
                chat.IsGroup = false;
            }

            try
            {
                await _dBcontext.SaveChangesAsync();

                ChatDTO chatDto = _chatService.CreateChatDTO(chat);

                return Ok(chatDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding users to chat. {ex.Message + ex.StackTrace + ex.Source}");
            }
        }

        // GET: api/1
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetAllUserChats(int userId)
        {
            if (!_dBcontext.AppUsers.Any(u => u.Id == userId))
                return NotFound(userId);

            try
            {
                var chatDtos = await _chatService.GetMyUserChats(userId);

                return Ok(chatDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/chats/5
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

        // POST: api/chat
        [HttpPost("chat")]
        public async Task<ActionResult<ChatDTO>> CreateChat([FromBody] CreateChatRequestDTO chatRequest)
        {
            if (chatRequest.ParticipantIds == null || chatRequest.ParticipantIds.Count < 2)
            {
                return BadRequest("At least two participants are required.");
            }

            var chatDto = await _chatService.CreateChat(chatRequest);

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
