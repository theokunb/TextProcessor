using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TextProcessor.Models;

namespace TextProcessor.CustomControls
{
    [TemplatePart(Name = "listSuggestions", Type = typeof(ListBox))]
    [TemplatePart(Name = "suggestionPopup", Type = typeof(Popup))]
    public class Suggestion : Control
    {
        private const int MAX_SUGGESTIONS = 5;
        private const int MIN_LENGHT = 1;

        public ListBox ListSuggestions;
        public Popup SuggestionPopup;
        public delegate void SuggestionSelected(string text);
        public event SuggestionSelected OnSuggestionSelected;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                ListSuggestions = Template.FindName("listSuggestions", this) as ListBox;
                SuggestionPopup = Template.FindName("suggestionPopup", this) as Popup;
                ListSuggestions.PreviewMouseDown += ListSuggestions_PreviewMouseDown;
                ListSuggestions.KeyDown += ListSuggestions_KeyDown;
            }
        }

        private void ListSuggestions_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item == null)
                return;
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string selectedText = item.Content as string;
                if (selectedText == null)
                    return;
                OnSuggestionSelected?.Invoke(selectedText);
                SuggestionPopup.IsOpen = false;
            }
        }

        private void ListSuggestions_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                TextBlock item = e.OriginalSource as TextBlock;
                if (item == null)
                    return;
                OnSuggestionSelected?.Invoke(item.Text);
                SuggestionPopup.IsOpen = false;
            }
        }

        public void ShowSuggestions(ObservableCollection<Word> suggestions, string text)
        {
            if (text.Length < MIN_LENGHT)
            {
                SuggestionPopup.IsOpen = false;
                return;
            }
            List<Word> collection = suggestions.Where(element =>
            {
                return element.Content.StartsWith(text, System.StringComparison.CurrentCultureIgnoreCase);
            }).
            OrderByDescending(word => word.Frequency).
            ToList();

            ListSuggestions.Items.Clear();
            for (int i = 0; i < MAX_SUGGESTIONS; i++)
            {
                if (i == collection.Count)
                    break;
                ListSuggestions.Items.Add(collection[i].Content);
            }

            SuggestionPopup.IsOpen = ListSuggestions.Items.Count > 0;
        }
    }
}
