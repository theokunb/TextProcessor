using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextProcessor.Services
{
    public interface IDatabase<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> WriteAsync(T input);
        Task<int> UpdateAsync(T input);
        Task<int> DeleteAsync();
        Task<bool> IsExistAsync(T input);
    }
}
