namespace NucleusWPF.MVVM
{
    public sealed class RelayCommand : IRelayCommand
    {
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public RelayCommand(Action execute) : this(execute, () => true)
        {

        }

        private readonly Action _execute;

        private readonly Func<bool> _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter)
        {
            try
            {
                _execute();
            }
            catch (Exception ex)
            {
                var messageService = DependencyInjection.Instance.Resolve<IMessageService>();
                messageService.Show(ex);
            }
        }

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public sealed class RelayCommand<T> : IRelayCommand
    {
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public RelayCommand(Action<T> execute, Func<bool> canExecute) :
            this(execute, _ => canExecute())
        {
            _parameterRequired = false;
        }

        public RelayCommand(Action<T> execute) : this(execute, _ => true)
        {
            _parameterRequired = false;
        }

        private readonly bool _parameterRequired = true;

        private readonly Action<T> _execute;

        private readonly Func<T, bool> _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (!_parameterRequired) return _canExecute(default!);
            else if (parameter is T t) return _canExecute(t);
            else return false;
        }

        public void Execute(object? parameter)
        {
            try
            {
                if (parameter is not T t)
                    throw new ArgumentException($"Parameter must be of type {typeof(T).Name}", nameof(parameter));

                _execute(t);
            }
            catch (Exception ex)
            {
                var messageService = DependencyInjection.Instance.Resolve<IMessageService>();
                messageService.Show(ex);
            }
        }

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
