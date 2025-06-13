using System.Windows.Input;

namespace NucleusWPF.MVVM
{
    public interface IRelayCommand : ICommand
    {
        public void RaiseCanExecuteChanged();
    }
}
