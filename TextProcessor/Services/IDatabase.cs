using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextProcessor.Services
{
    public interface IDatabase<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetOnceAsync(string input);
        Task<int> WriteAsync(T input);
        Task<int> UpdateAsync(T input);
        Task<int> DeleteAsync();
        Task<bool> IsExistAsync(T input);
    }
}
