using System.Collections;
using System.ComponentModel;

namespace OrderingViewModel.Validators
{
    public class ObservableValidators : BaseViewModel, INotifyDataErrorInfo
    {

        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public event Action<string>? ErrorUpdated;

        public bool HasErrors => _errors.Count > 0;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return Array.Empty<string>();

            return _errors.TryGetValue(propertyName, out var errors)
                ? errors
                : Array.Empty<string>();
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errors.TryGetValue(propertyName, out var errors))
            {
                errors = new List<string>();
                _errors[propertyName] = errors;
            }

            if (!errors.Contains(error))
            {
                errors.Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                ErrorUpdated?.Invoke(GetConcatenatedMessage());
                OnPropertyChanged(nameof(HasErrors));
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                ErrorUpdated?.Invoke(GetConcatenatedMessage());
                OnPropertyChanged(nameof(HasErrors));

            }
        }

        private string GetConcatenatedMessage()
        {
            return string.Join("\n", _errors.Values.SelectMany(x => x));
        }

    }
}
