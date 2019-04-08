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

    public class QueryManagerViewModel : DbManagerBaseViewModel
    {



        #region Consts

        private const string DefaultTitle = "Query Manager";
        #endregion



        #region Properties

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
        /// <param name="isProcessingChangedAction">Function to be called when the processing status changes (IE: operation starts/stops)</param>
        /// <param name="onErrorAction">Function to be called when an error is raised.</param>
        public QueryManagerViewModel(Action<bool> isProcessingChangedAction, Action<string> onErrorAction)
            : base(DefaultTitle, isProcessingChangedAction, onErrorAction)

        {
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
            IsProcessing = true;

            try
            {
                Stopwatch elapsed = new Stopwatch();
                elapsed.Start();

                await Task.Run(() => ExecSqlQuery());

                elapsed.Stop();
                ElapsedSeconds = (float)elapsed.Elapsed.TotalMilliseconds / 1000;
            }
            catch (Exception exc)
            {
                RaiseError(exc.Message);
            }
            finally
            {
                IsProcessing = false;
            }
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
                RaiseError(exc.Message);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        #endregion



        #region Private Methods

        #endregion

    }
}
