namespace NucleusWPF.MVVM
{
    public class AsyncRelayCommand : IRelayCommand
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

        public delegate Task ExecuteDelegate();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) =>
            Interlocked.CompareExchange(ref _isExecuting, 0, 0) == 0 && _canExecute();

        public async Task ExecuteAsync()
        {
            if (Interlocked.Exchange(ref _isExecuting, 1) == 1) return;
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
                    var messageService = new MessageService();
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

    //add AsyncRelayCommand<T> here
}
