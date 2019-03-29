using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SQLiteUtils.Commands
{
    public class ParameterlessCommandAsync : ICommand
    {


        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isRunning = false;



        #region ctor
        public ParameterlessCommandAsync(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion



        #region Public Methods

        public bool CanExecute()
        {
            return !_isRunning && (_canExecute?.Invoke() ?? true);
        }


        public async Task Execute()
        {
            try
            {
                _isRunning = true;
                RaiseCanExecuteChanged();

                await _execute();
            }
            catch(Exception exc)
            {
                throw exc;
            }
            finally
            {
                _isRunning = false;
                RaiseCanExecuteChanged();
            }
        }


        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
        #endregion




        #region ICommand Implementation

        public bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        public async void Execute(object parameter)
        {
            try
            {
                await Execute();
            }
            catch(Exception exc)
            {
                throw exc;
            }
        }

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
        #endregion


    }
}
