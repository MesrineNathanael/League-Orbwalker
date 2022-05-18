using System;
using System.Windows.Input;

namespace Gumayusi_Orbwalker.Core.MVVM
{
    public class CommandBase : ICommand
    {
        private Action<object> _executeMethod;

        private Func<object, bool> _canExecuteMethod;

        public object _parameter { get; set; }

        public string Libelle { get; set; }

        public event EventHandler CanExecuteChanged;

        public CommandBase(Action<object> executeMethod)
            : this(executeMethod, null, null, null)
        {

        }

        public CommandBase(Action<object> executeMethod, string libelle, object parameter)
            : this(executeMethod, null, libelle, parameter)
        {

        }

        public CommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, null, null)
        {

        }

        public CommandBase(Action<object> executeMethod, string libelle)
            : this(executeMethod, null, libelle, null)
        {

        }

        public CommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod, string libelle, object parameter)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
            Libelle = libelle;
            _parameter = parameter;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteMethod != null)
            {
                if (parameter != null)
                    return _canExecuteMethod(parameter);
                else
                    return _canExecuteMethod(_parameter);
            }
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
                _executeMethod(parameter);
            else
                _executeMethod(_parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
