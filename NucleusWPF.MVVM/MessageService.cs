using System.Windows;

namespace NucleusWPF.MVVM
{
    public class MessageService : IMessageService
    {
        public virtual void Show(string message, string title, MessageBoxButton button, MessageBoxImage icon) =>
            MessageBox.Show(message, title, button, icon);

        public virtual void Show(Exception ex) =>
            MessageBox.Show(ex.GetType().Name, "Unhanlded Exception", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
