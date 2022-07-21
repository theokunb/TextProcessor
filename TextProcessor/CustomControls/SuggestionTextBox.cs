using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TextProcessor.Models;

namespace TextProcessor.CustomControls
{
    [TemplatePart(Name = "editor", Type = typeof(TextBox))]
    public class SuggestionTextBox : Control
    {
        public ObservableCollection<Word> Suggestions
        {
            get { return (ObservableCollection<Word>)GetValue(SuggestionsProperty); }
            set { SetValue(SuggestionsProperty, value); }
        }


        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.Register(nameof(Suggestions), typeof(ObservableCollection<Word>), typeof(SuggestionTextBox), new PropertyMetadata(null));


        private TextAnalizer analizer;



        public TextBox TextBox;
        public Suggestion Suggestion;



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template != null)
            {
                TextBox = Template.FindName("editor", this) as TextBox;
                Suggestion = Template.FindName("suggestion", this) as Suggestion;
                Suggestion.OnSuggestionSelected += Suggestion_OnSuggestionSelected;
                analizer = new TextAnalizer();
                if (TextBox != null)
                {
                    TextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                    TextBox.KeyDown += TextBox_KeyDown;
                    TextBox.TextChanged += TextBox_TextChanged;
                }
            }
        }


        private void Suggestion_OnSuggestionSelected(string word)
        {
            int currentPos = TextBox.CaretIndex;
            string firstPart = analizer.GetFirstPart(TextBox.Text, currentPos);
            string secondPart = analizer.GetSecondPart(word, firstPart);
            TextBox.Text = TextBox.Text.Insert(currentPos, secondPart) + " ";
            TextBox.Focus();
            TextBox.CaretIndex = currentPos + secondPart.Length + 1;
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

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Suggestion.SuggestionPopup.IsOpen = false;
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Down && Suggestion.ListSuggestions.Items.Count > 0 && !(e.OriginalSource is ListBoxItem))
            {
                Suggestion.ListSuggestions.Focus();
                Suggestion.ListSuggestions.SelectedIndex = 0;
                ListBoxItem item = Suggestion.ListSuggestions.ItemContainerGenerator.ContainerFromIndex(Suggestion.ListSuggestions.SelectedIndex) as ListBoxItem;
                item.Focus();
                e.Handled = true;
            }
        }
    }
}
