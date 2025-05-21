using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApp.Data;
using RealTimeChatApp.Models;

namespace RealTimeChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _dbContext;
        public UsersController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: Users
        [HttpGet("getUsers")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(string username)
        {
            try
            {
                var users = await _dbContext.AppUsers.Where(u => u.UserName != username).ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }
        // GET user by id
        [HttpGet("by-id/{id}")]
        public async Task<ActionResult<AppUser>> GetUserById(int id)
        {
            var user = await _dbContext.AppUsers.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(new AppUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            });
        }


        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
