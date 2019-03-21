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




namespace SQLiteUtils.ViewModel
{



    public class MainViewModel : BaseViewModel
    {

        #region Consts
        #endregion


        #region Private Fields
        #endregion



        #region INotifyPropertyChanged Implementation

        #endregion



        #region Properties

        public List< BaseViewModel> ChildViewModels { get; set; }

        public DbGeneratorManagerViewModel DbGeneratorViewModel { get; set; }

        public QueryManagerViewModel QueryManagerViewModel { get; set; }

        public BaseViewModel SelectedViewModel { get; set; }

        /// <summary>
        /// App title shown on View
        /// </summary>
        public string Title { get; set; } = GymAppSQLiteConfig.AppName;

        /// <summary>
        /// App subtitle shown on View
        /// </summary>
        public string Subtitle { get; set; } = "DB tools to support development";


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
                DbName = GymAppSQLiteConfig.GetDbFullpath(DatabaseList.Where((x, i) => i == _selectedDbIndex).First());

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
            }
        }

        #endregion




        public MainViewModel()
        {
            DbGeneratorViewModel = new DbGeneratorManagerViewModel();
            QueryManagerViewModel = new QueryManagerViewModel();
            SelectedViewModel = DbGeneratorViewModel;


            ChildViewModels = new List<BaseViewModel>()
            {
                DbGeneratorViewModel, QueryManagerViewModel,
            };

            DatabaseList = GymAppSQLiteConfig.GetDatabaseList().Select(x => Regex.Replace(Path.GetFileName(x), ".db", "")).ToList();

            try
            {
                SelectedDbIndex = DatabaseList.FindIndex(x => Path.GetFileName(x) == GymAppSQLiteConfig.DefaultDbName);
            }
            catch
            {
                SelectedDbIndex = 0;
            }
        }

    }
}
