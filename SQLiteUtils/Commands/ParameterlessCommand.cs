using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SQLiteUtils.Commands
{


    public class ParameterlessCommand : ICommand
    {


        private Action _execute;
        private Func<bool> _canExecute;




        public ParameterlessCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }



        public ParameterlessCommand(Action execute) : this(execute, null) { }




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
            return _canExecute == null || _canExecute();
        }


        public void Execute(object parameter)
        {
            _execute.Invoke();
        }

    }



}
