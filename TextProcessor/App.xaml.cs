using System;
using System.IO;
using System.Windows;
using TextProcessor.Models;
using TextProcessor.Services;

namespace TextProcessor
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IDatabase<Word> database;

        public static IDatabase<Word> SqliteDatabase
        {
            get
            {
                if (database == null)
                    database = new SqliteDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mydb.db"));
                return database;
            }
        }
    }
}
