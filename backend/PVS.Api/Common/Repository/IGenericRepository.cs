namespace PVS.Api.Common.Repository;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
    Task<int> AddAsync(T entity);
    Task<int> AddRangeAsync(IEnumerable<T> entities);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(T entity);
    Task<int> DeleteRangeAsync(IEnumerable<T> entities);
    Task<int> SaveChangesAsync();
}
