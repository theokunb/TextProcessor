using SQLite;

namespace TextProcessor.Models
{
    public class Word
    {
        public Word()
        {
            Frequency = 0;
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Content { get; set; }
        public int Frequency { get; set; }

        public void IncreaseFrequency()
        {
            Frequency++;
        }
    }
}
