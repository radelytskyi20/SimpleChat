using Microsoft.EntityFrameworkCore;
using SimpleChat.Library.Data;
using SimpleChat.Library.Models;

namespace SimpleChat.Library.Repos
{
    public class UsersRepo : BaseRepo<User>
    {
        public UsersRepo(ApplicationDbContext context) : base(context) { Table = Context.Users; }

        public override async Task<User?> GetOneAsync(Guid id)
        {
            return await Table
                .Include(u => u.Chats)
                .Include(u => u.ChatsCreated)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await Table
                .Include(u => u.Chats)
                .Include(u => u.ChatsCreated)
                .ToListAsync();
        }
    }
}
