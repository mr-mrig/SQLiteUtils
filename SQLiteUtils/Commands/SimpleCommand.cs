using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SQLiteUtils.Commands
{


    public class SimpleCommand<T> : ICommand
    {




        private Action<T> _execute;
        private Predicate<T> _canExecute;




        public SimpleCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        

        public SimpleCommand(Action<T> execute) : this(execute, null) { }




        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }


        public void Execute(object parameter)
        {
            _execute.Invoke((T)parameter);
        }

    }
}
