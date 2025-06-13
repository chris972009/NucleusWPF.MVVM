using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NucleusWPF.MVVM
{
    public abstract class ValidViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected void RaiseErrorsChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName != null)
                ErrorsChanged?.Invoke(this, new(propertyName));
        }

        public IEnumerable GetErrors([CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null || !_errors.ContainsKey(propertyName))
                return Enumerable.Empty<string>();
            return new List<string>();
        }

        protected void AddError(string error, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null) return;
            if (_errors.ContainsKey(propertyName)) _errors.Add(propertyName, []);
            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                RaiseErrorsChanged(propertyName);
                RaisePropertyChanged(nameof(HasErrors));
            }
        }

        protected void ClearErrors([CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null) return;
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
                RaisePropertyChanged(nameof(HasErrors));
            }
        }
    }
}
