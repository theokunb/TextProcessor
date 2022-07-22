using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TextProcessor.Models;
using TextProcessor.Services;

namespace TextProcessor.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            Suggestions = new ObservableCollection<Word>();
            dictionary = new MyDictionaryManager();


            CommandLoaded = new Command(param => OnLoaded(param));
            CommandDictionary = new Command(param => OnDictionatyTapped(param));
            CommandSuggestionSelected = new Command(param => OnSuggestionSelected(param));
        }


        private const string FILE_FILTER = "|*.txt";
        private readonly IDictionatyManager dictionary;


        public ObservableCollection<Word> Suggestions { get; set; }
        public ICommand CommandLoaded { get; }
        public ICommand CommandDictionary { get; }
        public ICommand CommandSuggestionSelected { get; }


        private async void OnLoaded(object param)
        {

            if (Suggestions.Count == 0)
            {
                var collection = await App.SqliteDatabase.GetAllAsync();
                foreach (var element in collection)
                    Suggestions.Add(element);
            }
        }


        private async void OnDictionatyTapped(object param)
        {
            if (param.ToString().Equals("create"))
            {
                OpenFileDialog dialogResult = new OpenFileDialog();
                dialogResult.ShowDialog();
                if (string.IsNullOrEmpty(dialogResult.FileName))
                    return;

                await dictionary.Create(dialogResult.FileName);
                await UpdateCollection();
            }
            else if (param.ToString().Equals("update"))
            {
                OpenFileDialog dialogResult = new OpenFileDialog();
                dialogResult.ShowDialog();
                if (string.IsNullOrEmpty(dialogResult.FileName))
                    return;

                await dictionary.Update(dialogResult.FileName);
                await UpdateCollection();
            }
            else if (param.ToString().Equals("clear"))
            {
                await dictionary.Clear();
                await UpdateCollection();
            }
        }
        private async Task UpdateCollection()
        {
            Suggestions.Clear();
            var collection = await App.SqliteDatabase.GetAllAsync();
            foreach (var element in collection)
            {
                Suggestions.Add(element);
            }
        }
        private async void OnSuggestionSelected(object param)
        {
            var entityWord = param as Word;
            entityWord.IncreaseFrequency();
            await App.SqliteDatabase.UpdateAsync(entityWord);
            Suggestions.Where(element => element.Content == entityWord.Content).First().IncreaseFrequency();
        }
    }
}
