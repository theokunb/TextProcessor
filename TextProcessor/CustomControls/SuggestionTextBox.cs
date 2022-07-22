using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TextProcessor.Models;

namespace TextProcessor.CustomControls
{
    [TemplatePart(Name = "editor", Type = typeof(TextBox))]
    public class SuggestionTextBox : Control
    {


        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.Register(nameof(Suggestions), typeof(ObservableCollection<Word>), typeof(SuggestionTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty SuggestionSelectedCommandProperty =
            DependencyProperty.Register(nameof(SuggestionSelectedCommand),typeof(ICommand),typeof(SuggestionTextBox),new PropertyMetadata(null));
        public static readonly DependencyProperty SuggestionWordAddedCommandProperty =
            DependencyProperty.Register(nameof(SuggestionWordAddedCommand), typeof(ICommand), typeof(SuggestionTextBox), new PropertyMetadata(null));



        private TextAnalizer analizer;
        private Dictionary<string, int> sessionWords;
        private const int MIN_FOR_INCLUDE_DICTIONARY = 5;



        public TextBox TextBox;
        public Suggestion Suggestion;

        public ObservableCollection<Word> Suggestions
        {
            get { return (ObservableCollection<Word>)GetValue(SuggestionsProperty); }
            set { SetValue(SuggestionsProperty, value); }
        }
        public ICommand SuggestionSelectedCommand
        {
            get { return GetValue(SuggestionSelectedCommandProperty) as ICommand; }
            set { SetValue(SuggestionSelectedCommandProperty, value); }
        }
        public ICommand SuggestionWordAddedCommand
        {
            get { return GetValue(SuggestionWordAddedCommandProperty) as ICommand; }
            set { SetValue(SuggestionWordAddedCommandProperty, value); }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template != null)
            {
                TextBox = Template.FindName("editor", this) as TextBox;
                Suggestion = Template.FindName("suggestion", this) as Suggestion;
                Suggestion.OnSuggestionSelected += Suggestion_OnSuggestionSelected;
                analizer = new TextAnalizer();
                sessionWords = new Dictionary<string, int>();
                if (TextBox != null)
                {
                    TextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                    TextBox.TextChanged += TextBox_TextChanged;
                }
            }
        }


        private async void Suggestion_OnSuggestionSelected(string word)
        {
            int currentPos = TextBox.CaretIndex;
            string firstPart = analizer.GetFirstPart(TextBox.Text, currentPos);
            string secondPart = analizer.GetSecondPart(word, firstPart);
            TextBox.Text = TextBox.Text.Insert(currentPos, secondPart) + " ";
            TextBox.Focus();
            TextBox.CaretIndex = currentPos + secondPart.Length + 1;

            var entityWord = await App.SqliteDatabase.GetOnceAsync(word);
            if (entityWord == null)
                return;
            
            SuggestionSelectedCommand?.Execute(entityWord);
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string firstPart = analizer.GetFirstPart(TextBox.Text, TextBox.CaretIndex);
            Rect rect = TextBox.GetRectFromCharacterIndex(TextBox.CaretIndex);
            Geometry geometry = analizer.GetGeometry(firstPart, TextBox);
            Suggestion.SuggestionPopup.HorizontalOffset = rect.Left - geometry.Bounds.Width;
            Suggestion.SuggestionPopup.VerticalOffset = rect.Top + (2 * geometry.Bounds.Height);
            Suggestion.ShowSuggestions(Suggestions, firstPart);
        }


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && Suggestion.ListSuggestions.Items.Count > 0 && !(e.OriginalSource is ListBoxItem))
            {
                Suggestion.ListSuggestions.Focus();
                Suggestion.ListSuggestions.SelectedIndex = 0;
                ListBoxItem item = Suggestion.ListSuggestions.ItemContainerGenerator.ContainerFromIndex(Suggestion.ListSuggestions.SelectedIndex) as ListBoxItem;
                item.Focus();
                e.Handled = true;
            }
            else if(e.Key == Key.OemComma || e.Key == Key.OemPeriod)
            {
                var currentIndex = TextBox.CaretIndex;
                TextBox.Text = analizer.RemoveSpace(TextBox.Text, currentIndex);
                TextBox.CaretIndex = currentIndex;
            }
            else if(e.Key == Key.Space || e.Key == Key.Enter)
            {
                var word = analizer.GetFirstPart(TextBox.Text, TextBox.CaretIndex);
                if (sessionWords.Keys.Contains(word))
                {
                    sessionWords[word]++;
                    if(sessionWords[word] == MIN_FOR_INCLUDE_DICTIONARY)
                    {
                        var newWord = new Word 
                        {
                            Content = word
                        };
                        SuggestionWordAddedCommand?.Execute(newWord);
                    }
                }
                else
                    sessionWords.Add(word, 1);
            }
        }
    }
}
