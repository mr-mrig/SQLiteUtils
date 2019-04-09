using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Input;
using SQLiteUtils.Commands;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using SQLiteUtils.Model;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace SQLiteUtils.ViewModel
{



    public class MainViewModel : BaseViewModel
    {


        #region Consts
        private const int ElapsedTimeRefreshRateMs = 1000;
        #endregion



        #region Private Fields
        private DispatcherTimer _elapsedTimeUpdTimer;
        #endregion



        #region INotifyPropertyChanged Implementation

        private TimeSpan _elapsedTime;

        /// <summary>
        /// Time elapsed during SQL processing.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        private string _errorMessage = string.Empty;

        /// <summary>
        /// When an error occurs, the message is stored here.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        #endregion



        #region Properties

        public List<DbManagerBaseViewModel> ChildViewModels { get; set; }

        public RawGeneratorManagerViewModel DbGeneratorViewModel { get; set; }

        public QueryManagerViewModel QueryManagerViewModel { get; set; }

        public SmartGeneratorManagerViewModel SmartGeneratorViewModel { get; set; }

        public BaseViewModel SelectedViewModel { get; set; }

        #endregion

        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// List of available DBs
        /// </summary>
        public List<string> DatabaseList { get; set; }

        private int _selectedDbIndex;

        /// <summary>
        /// Index of the selected DB
        /// </summary>
        public int SelectedDbIndex
        {
            get => _selectedDbIndex;
            set
            {
                _selectedDbIndex = value;

                // Update the DbName accordingly
                DbName = GymAppSQLiteConfig.GetDbFullpath(DatabaseList?.Where((x, i) => i == _selectedDbIndex).First());
            }
        }

        private string _dbName;

        public string DbName
        {
            get => _dbName;
            set
            {
                _dbName = value;

                // Propagate to the childs
                foreach (DbManagerBaseViewModel vm in ChildViewModels)
                    vm.DbName = _dbName;
            }
        }

        private float _taskProgress = 0;

        /// <summary>
        /// Progress of the ongoing task. Used to update the TaskBarInfo
        /// </summary>
        public float TaskProgress
        {
            get => _taskProgress;
            set => SetProperty(ref _taskProgress, value);
        }
        #endregion



        #region Ctors

        public MainViewModel()
        {
            DbGeneratorViewModel = new RawGeneratorManagerViewModel(IsProcessingChanged, UpdateTaskProgress, ErrorMessageReceived);
            QueryManagerViewModel = new QueryManagerViewModel(IsProcessingChanged, ErrorMessageReceived);
            SmartGeneratorViewModel = new SmartGeneratorManagerViewModel(IsProcessingChanged, UpdateTaskProgress, ErrorMessageReceived);

            ChildViewModels = new List<DbManagerBaseViewModel>()
            {
                SmartGeneratorViewModel, DbGeneratorViewModel, QueryManagerViewModel,
            };

            SelectedViewModel = ChildViewModels.First();

            DatabaseList = GymAppSQLiteConfig.GetDatabaseList()?.Select(x => Regex.Replace(Path.GetFileName(x), ".db", "")).ToList();

            try
            {
                SelectedDbIndex = DatabaseList.FindIndex(x => Path.GetFileName(x) == GymAppSQLiteConfig.DefaultDbName);
            }
            catch
            {
                SelectedDbIndex = 0;
            }

            _elapsedTimeUpdTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, ElapsedTimeRefreshRateMs),
            };
            _elapsedTimeUpdTimer.Tick += (o, e) =>
            {
                ElapsedTime = ElapsedTime.Add(new TimeSpan(0, 0, 0, 0, ElapsedTimeRefreshRateMs));
            };

        }
        #endregion



        #region Public Methods

        /// <summary>
        /// Must be called by the childs when their processing state changes
        /// </summary>
        /// <param name="isProcessing"></param>
        private void IsProcessingChanged(bool isProcessing)
        {
            // Update the timer
            if (isProcessing)
            {
                ElapsedTime = new TimeSpan();

                _elapsedTimeUpdTimer.Start();
            }
            else
                _elapsedTimeUpdTimer.Stop();
        }


        /// <summary>
        /// Must be called by the childs when their processing state changes
        /// </summary>
        /// <param name="isProcessing"></param>
        private void UpdateTaskProgress(long processValue, long processTotal)
        {
            TaskProgress = (float)processValue / (float)processTotal;
        }


        /// <summary>
        /// Must be called by the childs when they end in error
        /// </summary>
        /// <param name="error"></param>
        private void ErrorMessageReceived(string error)
        {

            //Dispatcher.CurrentDispatcher.Invoke(() => ErrorMessage = error);
            //// Display the error
            ErrorMessage = error;
        }
        #endregion
    }
}
