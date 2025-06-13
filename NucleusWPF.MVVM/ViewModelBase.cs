using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NucleusWPF.MVVM
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaiseAndSetIfChanged<T>(ref T targetProperty, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(targetProperty, value))
            {
                targetProperty = value;
                RaisePropertyChanged(propertyName);
            }
        }
    }
}
