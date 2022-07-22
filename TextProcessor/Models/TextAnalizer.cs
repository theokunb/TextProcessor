using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;

namespace TextProcessor.Models
{
    public class TextAnalizer
    {
        public TextAnalizer()
        {
            regIsChar = new Regex($"[a-zA-Z]");
        }



        private Regex regIsChar;


        public string GetFirstPart(string line, int index)
        {
            if (index > line.Length)
                return string.Empty;
            StringBuilder result = new StringBuilder();

            for (int i = index - 1; i >= 0; i--)
            {
                if (regIsChar.IsMatch(line[i].ToString()))
                    result.Insert(0, line[i]);
                else
                    break;
            }
            return result.ToString();
        }
        public string GetSecondPart(string wholeWord, string firstPart)
        {
            return wholeWord.Substring(firstPart.Length);
        }
        public Geometry GetGeometry(string text, TextBox textbox)
        {
            FormattedText _text = new FormattedText(text,
                System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(textbox.FontFamily, textbox.FontStyle, textbox.FontWeight, textbox.FontStretch),
                textbox.FontSize,
                Brushes.Black);
            return _text.BuildGeometry(new System.Windows.Point(0, 0));
        }
        public string RemoveSpace(string input,int index)
        {
            if (input.Length < index)
                return input;
            int coutSpaces = 0;
            for (int i = index - 1; i >= 0; i--)
            {
                if (input[i] == ' ')
                {
                    ++coutSpaces;
                }
                else
                    break;
            }
            if (coutSpaces == 1)
                return input.Remove(index - 1, 1);
            return input;
        }
    }
}
