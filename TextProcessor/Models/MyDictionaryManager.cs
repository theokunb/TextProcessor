using System.IO;
using System.Threading.Tasks;
using TextProcessor.Services;

namespace TextProcessor.Models
{
    public class MyDictionaryManager : IDictionatyManager
    {
        public MyDictionaryManager()
        {

        }

        private int MIN_WORD_LENGHT = 3;


        public async Task Clear()
        {
            await App.SqliteDatabase.DeleteAsync();
        }

        public async Task Create(string path)
        {
            if (!File.Exists(path))
                return;

            Clear();

            using (StreamReader reader = new StreamReader(path, encoding: System.Text.Encoding.Default))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(' ');
                    foreach (string word in parts)
                    {
                        if (word.Length < MIN_WORD_LENGHT)
                            continue;
                        await App.SqliteDatabase.WriteAsync(new Word
                        {
                            Content = word
                        });
                    }
                }
            }

        }

        public async Task Update(string path)
        {
            if (!File.Exists(path))
                return;

            using (StreamReader reader = new StreamReader(path, System.Text.Encoding.Default))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(' ');
                    foreach (string word in parts)
                    {
                        var newWord = new Word
                        {
                            Content = word
                        };
                        var isWordExist = await App.SqliteDatabase.IsExistAsync(newWord);
                        if (isWordExist)
                            continue;
                        if (word.Length < MIN_WORD_LENGHT)
                            continue;
                        await App.SqliteDatabase.WriteAsync(newWord);
                    }
                }
            }
        }
    }
}
