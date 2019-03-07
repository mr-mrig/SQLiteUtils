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
        private bool _isRunning = false;




        #region Ctors
        /// <summary>
        /// Generic MVVM Command implementation
        /// </summary>
        /// <param name="execute">Action to be performed by the Command</param>
        /// <param name="canExecute">Tells wheter the Command can be executed or not. If left null the Command itself handles it according to the internal operation status.</param>
        public ParameterlessCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion


        /// <summary>
        /// Forces the Command Manager to requery for the CanExecute status
        /// </summary>
        public void RaiseCanExecuteChange()
        {
            CommandManager.InvalidateRequerySuggested();
        }


        #region ICommand Implementation
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
            return  !_isRunning && (_canExecute?.Invoke() ?? true);
        }


        public void Execute(object parameter)
        {
            try
            {
                RaiseCanExecuteChange();

                _isRunning = true;
                _execute();
            }
            finally
            {
                RaiseCanExecuteChange();
                _isRunning = false;
            }
        }
        #endregion
    }



}
