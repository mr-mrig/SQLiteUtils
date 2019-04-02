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

        public List< BaseViewModel> ChildViewModels { get; set; }

        public DbGeneratorManagerViewModel DbGeneratorViewModel { get; set; }

        public QueryManagerViewModel QueryManagerViewModel { get; set; }

        public BaseViewModel SelectedViewModel { get; set; }



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
                if(DbGeneratorViewModel != null)
                    DbGeneratorViewModel.DbName = _dbName;

                // Propagate to the childs
                if (QueryManagerViewModel != null)
                    QueryManagerViewModel.DbName = _dbName;
            }
        }

        #endregion



        #region Ctors

        public MainViewModel()
        {
            DbGeneratorViewModel = new DbGeneratorManagerViewModel(IsProcessingChanged, ErrorMessageReceived);
            QueryManagerViewModel = new QueryManagerViewModel(IsProcessingChanged, ErrorMessageReceived);
            SelectedViewModel = DbGeneratorViewModel;


            ChildViewModels = new List<BaseViewModel>()
            {
                DbGeneratorViewModel, QueryManagerViewModel,
            };

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
        /// Must be called by the childs when they end in error
        /// </summary>
        /// <param name="error"></param>
        private void ErrorMessageReceived(string error)
        {
            // Display the error
            ErrorMessage = error;
        }
        #endregion
    }
}
