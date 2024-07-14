namespace SimpleChat.Library.Interfaces
{
    public interface IRepo<T>
    {
        Task<Guid> AddAsync(T entity);
        Task<int> DeleteAsync(Guid id);
        Task<int> UpdateAsync(T entity);
        Task<T?> GetOneAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
