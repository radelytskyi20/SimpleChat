using Microsoft.EntityFrameworkCore;
using SimpleChat.Library.Data;
using SimpleChat.Library.Models;

namespace SimpleChat.Library.Repos
{
    public class ChatsRepo : BaseRepo<Chat>
    {
        public ChatsRepo(ApplicationDbContext context) : base(context) { Table = Context.Chats; }

        public override async Task<Chat?> GetOneAsync(Guid id)
        {
            return await Table
                .Include(c => c.AdminUser)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<IEnumerable<Chat>> GetAllAsync()
        {
            return await Table
                .Include(c => c.AdminUser)
                .Include(c => c.Users)
                .ToListAsync();
        }
    }
}
