using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextProcessor.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void Set<T>(ref T oldValue, T newValue, string propName)
        {
            if (Equals(oldValue, newValue))
                return;
            oldValue = newValue;
            OnPropertyChanged(propName);
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
