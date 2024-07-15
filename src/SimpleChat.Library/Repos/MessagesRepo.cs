using Microsoft.EntityFrameworkCore;
using SimpleChat.Library.Data;
using SimpleChat.Library.Models;

namespace SimpleChat.Library.Repos
{
    public class MessagesRepo : BaseRepo<Message>
    {
        public MessagesRepo(ApplicationDbContext context) : base(context) { Table = Context.Messages; }

        public override async Task<Message?> GetOneAsync(Guid id)
        {
            return await Table
                .Include(m => m.User)
                .Include(m => m.Chat)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await Table
                .Include(m => m.User)
                .Include(m => m.Chat)
                .ToListAsync();
        }
    }
}
