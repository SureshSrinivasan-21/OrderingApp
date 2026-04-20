using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OrderingViewModel.Commands
{
    internal class AsyncRelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Func<Task> _execute;
        private Func<bool>? _canExecute;
        private bool _isExecute;

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => 
            ! _isExecute && (_canExecute?.Invoke() ?? true);
        
        public async void Execute(object? parameter)
        {
            if(!CanExecute(parameter))
            {
                return;
            }
            try
            {
                _isExecute = true;
                RaiseCanExecuteChanged();
                await _execute();
            }
            finally
            {
                _isExecute = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged() =>  CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
