namespace NucleusWPF.MVVM
{
    public sealed class AsyncRelayCommand : IRelayCommand
    {
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public AsyncRelayCommand(Func<Task> execute) : this(execute, () => true)
        {

        }

        private int _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) =>
            _isExecuting == 0 && _canExecute();

        public async Task ExecuteAsync()
        {
            if (Interlocked.CompareExchange(ref _isExecuting, 1, 0) != 0) return;
            RaiseCanExecuteChanged();
            try
            {
                await _execute();
            }
            finally
            {
                Interlocked.Exchange(ref _isExecuting, 0);
                RaiseCanExecuteChanged();
            }
        }

        public void Execute(object? parameter)
        {
            ExecuteAsync().ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    var messageService = DependencyInjection.Instance.Resolve<IMessageService>();
                    messageService.Show(t.Exception.InnerException ?? t.Exception);
                }
            },
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }

    public sealed class AsyncRelayCommand<T> : IRelayCommand
    {
        public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            _canExecuteParameterRequired = true;
        }

        public AsyncRelayCommand(Func<T, Task> execute, Func<bool> canExecute) : this(execute, (param) => canExecute())
        {
            _canExecuteParameterRequired = false;
        }

        public AsyncRelayCommand(Func<T, Task> execute) : this(execute, (param) => true)
        {
            _canExecuteParameterRequired = false;
        }

        private readonly bool _canExecuteParameterRequired;
        private int _isExecuting;
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (_isExecuting != 0) return false;
            
            if (_canExecuteParameterRequired)
                return parameter is T param && _canExecute(param);

            return _canExecute(default!);
        }

        public async Task ExecuteAsync(T t)
        {
            if (Interlocked.CompareExchange(ref _isExecuting, 1, 0) != 0) return;
            RaiseCanExecuteChanged();
            try
            {
                await _execute(t);
            }
            finally
            {
                Interlocked.Exchange(ref _isExecuting, 0);
                RaiseCanExecuteChanged();
            }
        }

        public void Execute(object? parameter)
        {
            Exception? ex = null;

            if(parameter is T value)
            {
                ExecuteAsync(value).ContinueWith(t =>
                {
                    if (t.Exception != null)
                        ex = t.Exception.InnerException ?? t.Exception;
                },
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                ex = new ArgumentException($"Pmeter must be of type {typeof(T).Name}", nameof(parameter));
            }

            if (ex != null)
            {
                var messageService = DependencyInjection.Instance.Resolve<IMessageService>();
                messageService.Show(ex);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
