using SQLiteUtils.Commands;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.ViewModel
{

    public class QueryManagerViewModel :BaseViewModel
    {



        #region Private Fields
        private Action<bool> _isProcessingChangedAction = null;       // Action to be performed when processing starts/ends
        private Action<string> _onErrorAction = null;                    // Action to be performed when an error is raised
        #endregion



        #region Properties

        public string ViewTitle { get; set; } = "Query";

        public ParameterlessCommandAsync ExecSqlQueryCommand { get; private set; }
        #endregion



        #region INotifyPropertyChanged Implementation

        private List<string> _sqlQueryHistory = new List<string>();

        /// <summary>
        /// History of the commands of the current session
        /// </summary>
        public List<string> SqlQueryHistory
        {
            get => _sqlQueryHistory;
            set => SetProperty(ref _sqlQueryHistory, value);
        }


        private string _sqlCommand = string.Empty;

        /// <summary>
        /// The SQL command
        /// </summary>
        public string SqlCommand
        {
            get => _sqlCommand;
            set
            {
                SetProperty(ref _sqlCommand, value);
                //SqlQueryHistory?.Add(_sqlCommand);
            }
        }

        private string _name = null;

        /// <summary>
        /// The SQL command
        /// </summary>
        public string DbName
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                //SqlQueryHistory?.Add(_sqlCommand);
            }
        }


        private bool _isProcessing = false;

        /// <summary>
        /// Query in process
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                if (SetProperty(ref _isProcessing, value))
                    _isProcessingChangedAction?.Invoke(value);
            }
        }

        private float _elapsedSeconds = 0;

        /// <summary>
        /// Query in process
        /// </summary>
        public float ElapsedSeconds
        {
            get => _elapsedSeconds;
            set
            {
                SetProperty(ref _elapsedSeconds, value);
            }
        }
        #endregion



        #region Ctors

        /// <summary>
        /// ViewModel for the Query Manager
        /// </summary>
        /// <param name="isProcessingChangedAction">Action to be performed when the processing starts/ends</param>
        public QueryManagerViewModel(Action<bool> isProcessingChangedAction, Action<string> onErrorAction)
        {
            _isProcessingChangedAction = isProcessingChangedAction;
            _onErrorAction = onErrorAction;

            ExecSqlQueryCommand = new ParameterlessCommandAsync(ExecSqlQueryAsync, () => !IsProcessing);
        }
        #endregion



        #region Public Methods

        /// <summary>
        /// Relat method for executing the query asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task ExecSqlQueryAsync()
        {
            Stopwatch elapsed = new Stopwatch();
            elapsed.Start();

            await Task.Run(() => ExecSqlQuery());

            elapsed.Stop();
            ElapsedSeconds = (float)elapsed.Elapsed.TotalMilliseconds / 1000;
        }


        /// <summary>
        /// Executes the query
        /// </summary>
        public void ExecSqlQuery()
        {
            IsProcessing = true;

            try
            {
                using (SQLiteConnection connection = DatabaseUtility.NewFastestSQLConnection(DbName))
                {
                    SQLiteCommand query = new SQLiteCommand()
                    {
                        Connection = connection,
                        CommandText = SqlCommand,
                    };




                    using (SQLiteDataReader sqlRead = query.ExecuteReader() as SQLiteDataReader)
                    {

                    }
                }
            }
            catch (Exception exc)
            {
                Debugger.Break();
            }
            finally
            {
                IsProcessing = false;
            }
        }

        #endregion

    }
}
