using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextProcessor.Services;

namespace TextProcessor.Models
{
    public class SqliteDatabase : IDatabase<Word>
    {
        public SqliteDatabase(string path)
        {
            connection = new SQLiteAsyncConnection(path);
            connection.CreateTableAsync<Word>().Wait();
        }


        private readonly SQLiteAsyncConnection connection;



        public async Task<IEnumerable<Word>> GetAllAsync()
        {
            return await connection.QueryAsync<Word>($"select * from Word");
        }
        public async Task<int> WriteAsync(Word input)
        {
            if (await IsExistAsync(input))
                return 0;
            return await connection.InsertAsync(input);
        }
        public async Task<int> UpdateAsync(Word input)
        {
            return await connection.UpdateAsync(input);
        }
        public async Task<int> DeleteAsync()
        {
            return await connection.DeleteAllAsync<Word>();
        }
        public async Task<bool> IsExistAsync(Word input)
        {
            List<Word> collection = await connection.Table<Word>().Where(item => item.Content == input.Content).ToListAsync();
            return collection.Count > 0;
        }
    }
}
