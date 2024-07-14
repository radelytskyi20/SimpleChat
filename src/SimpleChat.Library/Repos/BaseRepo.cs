using Microsoft.EntityFrameworkCore;
using SimpleChat.Library.Data;
using SimpleChat.Library.Interfaces;

namespace SimpleChat.Library.Repos
{
    public abstract class BaseRepo<T> : IRepo<T> where T : class, IIdentifiable
    {
        public ApplicationDbContext Context { get; init; }
        protected DbSet<T> Table { get; init; } = null!;

        public BaseRepo(ApplicationDbContext context) { Context = context; }

        public async Task<Guid> AddAsync(T entity)
        {
            await Table.AddAsync(entity);
            await SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var entity = await GetOneAsync(id);
            if (entity != null)
            {
                Table.Remove(entity);
                return await SaveChangesAsync();
            }
            
            return 0;
        }

        public async Task<int> UpdateAsync(T entity)
        {
            Table.Update(entity);
            return await SaveChangesAsync();
        }

        public virtual async Task<T?> GetOneAsync(Guid id) => await Table.FindAsync(id);
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await Table.ToListAsync();

        public async Task<int> SaveChangesAsync() //int => number of affected rows (added, updated, removed, etc.)
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                throw;
            }
            catch (DbUpdateException ex)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
