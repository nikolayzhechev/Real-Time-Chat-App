using Microsoft.EntityFrameworkCore;

namespace RealTimeChatApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> oprtions) : base(oprtions)
        {
        }
    }
}
