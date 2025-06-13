using System.Windows;

namespace NucleusWPF.MVVM
{
    public interface IMessageService
    {
        void Show(string message, string title, MessageBoxButton button, MessageBoxImage icon);

        void Show(Exception ex);
    }
}
