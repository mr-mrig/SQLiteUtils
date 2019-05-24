using SQLiteUtils.Model.ORM.EF_Imported;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace SQLiteUtils.ViewModel
{




    public class DbManagerBaseViewModel : BaseViewModel
    {


        #region Consts
        public readonly CultureInfo CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        #endregion


        #region Private fields
        protected Action<bool> _isProcessingChangedAction = null;         // Action to be performed when processing starts/ends
        protected Action<string> _onErrorAction = null;                   // Action to be performed when an error is raised
        #endregion


        #region Properties

        private string _dbName;

        /// <summary>
        /// Database path. Setting this value automatically opens the connection
        /// </summary>
        public string DbName
        {
            get => _dbName;
            set
            {
                _dbName = value;

                if (DbName != null)
                    Connection = DatabaseUtility.NewTradeoffSQLConnection(DbName);
            }
        }

        /// <summary>
        /// View Title
        /// </summary>
        public string ViewTitle { get; set; }

        /// <summary>
        /// DAO - SQLite connection to the database
        /// </summary>
        public SQLiteConnection Connection { get; set; } = null;            // Global to avoid DB locking issues

        /// <summary>
        /// EF - SQLite connection to the database
        /// </summary>
        public GymAppDbContext DbContext { get; set; } = null;
        #endregion


        #region INotifyPropertyChanged Implementation


        private bool _isProcessing = false;
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                if (SetProperty(ref _isProcessing, value))
                    _isProcessingChangedAction?.Invoke(value);

                // Reset errors
                if(_isProcessing)
                {
                    RaiseError("");
                    HasError = false;
                }
            }
        }


        private bool _hasError;

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }
        #endregion



        #region Ctors

        /// <summary>
        /// Base ViewModel for the app.
        /// </summary>
        /// <param name="title">View Model name</param>
        /// <param name="isProcessingChangedAction">Function to be called when the processing status changes (IE: operation starts/stops)</param>
        /// <param name="onErrorAction">Function to be called when an error is raised.</param>
        public DbManagerBaseViewModel(string title, Action<bool> isProcessingChangedAction, Action<string> onErrorAction)
        {
            // Parse parameters
            _isProcessingChangedAction = isProcessingChangedAction;
            _onErrorAction = onErrorAction;
            ViewTitle = title;

            // Decimal separator as dot, not comma
            CultureInfo.DefaultThreadCurrentCulture = CurrentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CurrentCulture;
        }
        #endregion


        #region Private Methods

        /// <summary>
        /// Raises an error routing it to the manager
        /// </summary>
        /// <param name="errorMessage">The error message. If empty then the manager will consider it as "no error"</param>
        protected virtual void RaiseError(string errorMessage)
        {
            HasError = true;
            _onErrorAction?.Invoke(errorMessage);
        }


        #endregion

    }
}
