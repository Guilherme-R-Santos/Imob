using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Imob.ViewModels.Commands
{
    public class AsyncRelayCommandWithParameter : ICommand
    {
        private readonly Func<object, Task> _execute;
        private readonly Predicate<object> _canExecute;
        private readonly Action<Exception> _onException;
        private bool _isExecuting;

        public AsyncRelayCommandWithParameter(Func<object, Task> execute, Predicate<object> canExecute = null, Action<Exception> onException = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            _onException = onException;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                _isExecuting = true;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                await _execute(parameter);
            }
            catch (Exception ex)
            {
                _onException?.Invoke(ex);
            }
            finally
            {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
