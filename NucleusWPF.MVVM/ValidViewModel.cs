using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NucleusWPF.MVVM
{
    /// <summary>
    /// Implementation of INotifyDataErrorInfo and INotifyPropertyChanged.
    /// </summary>
    public abstract class ValidViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _errors.Count > 0;

        /// <inheritdoc/>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of changed property.</param>
        protected void RaiseErrorsChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName != null)
                ErrorsChanged?.Invoke(this, new(propertyName));
        }

        /// <summary>
        /// Gets a list of errors attached to property.
        /// </summary>
        /// <param name="propertyName">Name of property</param>
        /// <returns>An <see cref="IEnumerable"/> of errors messages for specified property, or empty collection if there are no errors.</returns>
        public IEnumerable GetErrors([CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null || !_errors.ContainsKey(propertyName))
                return Enumerable.Empty<string>();
            return new List<string>();
        }

        /// <summary>
        /// Attach an error to a property.
        /// </summary>
        /// <param name="error">Error message to attach.</param>
        /// <param name="propertyName">Name of property.</param>
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

        /// <summary>
        /// Remove all errors attached to a property.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
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

        /// <summary>
        /// Updates the target property if the specified value is different, raises a property change notification,  and
        /// performs validation on the new value.
        /// </summary>
        /// <typeparam name="T">The type of the property being updated.</typeparam>
        /// <param name="targetProperty">A reference to the property to be updated.</param>
        /// <param name="value">The new value to assign to the property.</param>
        /// <param name="validate">An action to validate the new value. The action receives the new value and the property name as parameters.</param>
        /// <param name="propertyName">The name of the property being updated. This is automatically supplied by the caller if not explicitly
        /// provided.</param>
        protected void RaseAndValidateIfChanged<T>(ref T targetProperty, T value, Action<T, string?> validate, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(targetProperty, value))
            {
                targetProperty = value;
                RaisePropertyChanged(propertyName);
                validate(value, propertyName);
            }
        }
    }
}
