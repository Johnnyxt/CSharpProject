using System;
using System.Windows.Input;

namespace JW18001.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public DelegateCommand(Action<object> execute) : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    //internal class DelegateCommand<T> : ICommand
    //{
    //    private readonly Action<T> _executeMethod = null;
    //    private readonly Func<T, bool> _canExecuteMethod = null;

    //    public DelegateCommand(Action<T> executeMethod) : this(executeMethod, null)
    //    { }

    //    public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
    //    {
    //        if (executeMethod == null)
    //        {
    //            throw new ArgumentNullException(nameof(executeMethod));
    //            _executeMethod = executeMethod;
    //            _canExecuteMethod = canExecuteMethod;
    //        }
    //    }

    //    public bool CanExecute(T parameter)
    //    {
    //        if (_canExecuteMethod != null)
    //        {
    //            return _canExecuteMethod(parameter);
    //        }
    //        return true;
    //    }

    //    public void Execute(T parameter)
    //    {
    //        if (_executeMethod != null)
    //        {
    //            _executeMethod(parameter);
    //        }
    //    }

    //    event EventHandler ICommand.CanExecuteChanged
    //    {
    //        add { CommandManager.RequerySuggested += value; }
    //        remove { CommandManager.RequerySuggested -= value; }
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        if (parameter == null && typeof(T).IsValueType)
    //        {
    //            return (_canExecuteMethod == null);
    //        }
    //        return CanExecute((T)parameter);
    //    }

    //    public void Execute(object parameter)
    //    {
    //        Execute((T)parameter);
    //    }
    //}
}