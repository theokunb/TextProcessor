using System.Threading.Tasks;

namespace TextProcessor.Services
{
    public interface IDictionatyManager
    {
        Task Create(string path);
        Task Update(string path);
        Task Clear();
    }
}
