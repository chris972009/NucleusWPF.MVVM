using System.Windows;

namespace NucleusWPF.MVVM
{
    public sealed class RelayCommand : IRelayCommand
    {
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public RelayCommand(Action execute) : this(execute, () => true)
        {

        }

        private readonly Action execute;

        private readonly Func<bool> canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => canExecute();

        public void Execute(object? parameter) => execute();

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }

    public sealed class RelayCommand<T> : IRelayCommand
    {
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public RelayCommand(Action<T> execute, Func<bool> canExecute) :
            this(execute, _ => canExecute())
        {

        }

        public RelayCommand(Action<T> execute) : this(execute, _ => true)
        {

        }

        private readonly Action<T> execute;

        private readonly Func<T, bool> canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (parameter == null && typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null) return false;
            return canExecute(parameter is T t ? t : default!);
        }

        public void Execute(object? parameter)
        {
            throw new NotImplementedException();
        }

        public void RaiseCanExecuteChanged()
        {
            throw new NotImplementedException();
        }
    }
}
