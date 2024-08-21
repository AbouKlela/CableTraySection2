using System;
using System.Windows.Input;

namespace CableTraySection.ViewModel
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                // Ensure that the event handler is added correctly
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                // Ensure that the event handler is removed correctly
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            // Ensure that CanExecute is efficient and doesn’t perform heavy operations
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            try
            {
                execute(parameter);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Command execution failed: {ex.Message}");
                // Depending on your needs, you might want to rethrow the exception or handle it differently
                throw;
            }
        }
    }

}