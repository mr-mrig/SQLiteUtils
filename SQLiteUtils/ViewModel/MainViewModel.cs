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
    public class MainViewModel : INotifyPropertyChanged
    {


        #region Consts
        public const uint NotyfyPeriodRows = 500000;                                                                // Number of rows which the user is notified at (via Log row)


        private readonly float[] FitnessDayProbabilityArray = new float[4] { 0.5f, 0.9f, 0.3f, 0.85f };

        private readonly CultureInfo currentCulture = CultureInfo.GetCultureInfo("en-US");                          // Dot as decimal separator
        #endregion


        #region Private Fields
        private long _insertedRows = 0;
        private SQLiteConnection _connection = null;       // Global to avoid DB locking issues
        #endregion



        #region Properties
        private long _totalRowsNumber = long.MaxValue;
        public long TotalRowsNumber
        {
            get => _totalRowsNumber;
            set
            {
                _totalRowsNumber = value;
                RaisePropertyChanged();
            }
        }

        private long _processedRowsNumber = 0;
        public long ProcessedRowsNumber
        {
            get => _processedRowsNumber;
            set
            {
                _processedRowsNumber = value;
                RaisePropertyChanged();
            }
        }

        private string _sqlLogEntries = string.Empty;
        public string SqlLogEntries
        {
            get => _sqlLogEntries;
            set
            {
                _sqlLogEntries = value;
                RaisePropertyChanged();
            }
        }

        private string _sqlTableFailed = string.Empty;
        public string SqlFail
        {
            get => _sqlTableFailed;
            private set
            {
                _sqlTableFailed = value;
                RaisePropertyChanged();
            }
        }


        public ParameterlessCommandAsync _initDatabaseCommand;
        public ParameterlessCommandAsync InitDatabaseCommand
        {
            get => _initDatabaseCommand;
            private set
            {
                _initDatabaseCommand = value;
                RaisePropertyChanged();
            }
        }

        public ParameterlessCommandAsync _executeSqlCommand;
        public ParameterlessCommandAsync ExecuteSqlCommand
        {
            get => _executeSqlCommand;
            private set
            {
                _executeSqlCommand = value;
                RaisePropertyChanged();
            }
        }

        private bool _processing = false;
        public bool Processing
        {
            get => _processing;
            set
            {
                _processing = value;
                RaisePropertyChanged();

                Mouse.OverrideCursor = Processing ? Cursors.Wait : Cursors.Arrow;
            }
        }

        private bool _executingSql = false;
        public bool ExecutingSql
        {
            get => _executingSql;
            set
            {
                _executingSql = value;
                RaisePropertyChanged();

                // Link to Processing
                Processing = value;
            }
        }

        private string _title = "Populate the Database";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public MainViewModel()
        {
            InitDatabaseCommand = new ParameterlessCommandAsync(GenerateSqlScriptWrapperAync, () => !Processing);
            ExecuteSqlCommand = new ParameterlessCommandAsync(ExecuteSql, () => !Processing);

            // Decimal separator as dot, not comma
            CultureInfo.DefaultThreadCurrentCulture = currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = currentCulture;
        }


        //~MainViewModel()
        //{
        //    if(_connection.State != System.Data.ConnectionState.Closed)
        //        _connection.Close();
        //}


        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;


        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        private async Task ExecuteSql()
        {
            StringBuilder sqlString = new StringBuilder();
            int rowsModified = 0;
            Stopwatch partialTime = new Stopwatch();
            Stopwatch totalTime = new Stopwatch();

            ExecutingSql = true;

            ProcessedRowsNumber = 0;
            TotalRowsNumber = long.MaxValue;

            partialTime.Start();
            totalTime.Start();


            SqlFail = "";
            SqlLogEntries += Environment.NewLine;
            SqlLogEntries += Environment.NewLine;

            // Process the Script files
            try
            {
                if (!Directory.Exists(GymAppSQLiteConfig.SqlScriptFolder))
                {
                    SqlLogEntries += $"{Environment.NewLine}____________________________________  ERROR  ______________________________________________{Environment.NewLine}";
                    SqlLogEntries += $@"Path: {GymAppSQLiteConfig.SqlScriptFolder} does not exist{Environment.NewLine}";
                }
                else
                {
                    if (Directory.EnumerateFiles(GymAppSQLiteConfig.SqlScriptFolder).Count() == 0)
                    {
                        SqlLogEntries += $"{Environment.NewLine}____________________________________  ERROR  ______________________________________________{Environment.NewLine}";
                        SqlLogEntries += $@"Directory: {GymAppSQLiteConfig.SqlScriptFolder} is empty{Environment.NewLine}";
                    }
                    else
                    {

                        _connection = DatabaseUtility.OpenFastestSQLConnection(_connection, GymAppSQLiteConfig.DbName);
                        SQLiteTransaction sqlTrans = _connection.BeginTransaction();

                        TotalRowsNumber = GymAppSQLiteConfig.GetScriptFilesPath().Count();
                        ProcessedRowsNumber = 0;

                        foreach (string filename in GymAppSQLiteConfig.GetScriptFilesPath())
                        {
                            using (StreamReader scriptFile = new StreamReader(File.OpenRead(filename)))
                            {

                                // Import the file as a SQL command
                                SQLiteCommand cmd = new SQLiteCommand()
                                {
                                    Connection = _connection,
                                    CommandText = scriptFile.ReadToEnd(),           // This might be vulnerable to OutOfMemoryException
                                };

                                // Execute SQL
                                try
                                {
                                    rowsModified += await cmd.ExecuteNonQueryAsync();
                                }
                                catch (Exception exc)
                                {
                                    SqlLogEntries += $"{Environment.NewLine}____________________________________  ERROR  ______________________________________________{Environment.NewLine}";
                                    SqlLogEntries += $@"Error while executing the SQL command stored in {filename} {Environment.NewLine}";
                                    SqlLogEntries += $@"Exception: {exc.Message}{Environment.NewLine}";
                                }
                                finally
                                {
                                    cmd.Dispose();
                                }

                                partialTime.Stop();

                                SqlLogEntries += $@"{Path.GetFileName(filename)} processed in  " +
                                    $@"{totalTime.Elapsed.Hours.ToString()}:{totalTime.Elapsed.Minutes.ToString()}:{totalTime.Elapsed.Seconds.ToString()}{Environment.NewLine}";

                                // Update progress
                                ProcessedRowsNumber++;
                            }
                        }
                        sqlTrans.Commit();
                    }
                }
            }
            catch (Exception exc)
            {
                SqlLogEntries += $"{Environment.NewLine}\t\t\t------ ERROR ------{Environment.NewLine}";
                SqlLogEntries += $@"Error in the ICommand {Environment.NewLine}";
                SqlLogEntries += $@"Exception: {exc.Message}{Environment.NewLine}";
            }

            totalTime.Stop();

            // Display execution report
            SqlLogEntries += $@"___________________________________________________________________________________________{Environment.NewLine}";
            SqlLogEntries += $@"Number of rows inserted: {GetFormattedNumber(rowsModified)}{Environment.NewLine}";
            SqlLogEntries += $@"Total Time: {totalTime.Elapsed.Hours.ToString()}:{totalTime.Elapsed.Minutes.ToString()}:{totalTime.Elapsed.Seconds.ToString()}{Environment.NewLine}";
            SqlLogEntries += $@"Total Milliseconds per row: { ((float)totalTime.Elapsed.TotalMilliseconds / (float)rowsModified).ToString()} [ms]{Environment.NewLine}";

            ExecutingSql = false;

            return;
        }


        private async Task GenerateSqlScriptWrapperAync()
        {
            Stopwatch totalTime = new Stopwatch();

            totalTime.Start();

            SqlFail = string.Empty;
            SqlLogEntries = string.Empty;
            Processing = true;
            ProcessedRowsNumber = 0;

            // Delete old files
            foreach (string filePath in GymAppSQLiteConfig.GetScriptFilesPath())
                File.Delete(filePath);


            SqlLogEntries += Environment.NewLine;
            SqlLogEntries += Environment.NewLine;

            //SQLite connection
            _connection = DatabaseUtility.OpenFastestSQLConnection(_connection, GymAppSQLiteConfig.DbName);

            try
            {
                await Task.Run(() => WriteSqlScriptFiles(_connection));
            }
            catch (Exception exc)
            {
                SqlFail = $@"Error: {exc.Message}";
            }

            totalTime.Stop();

            SqlLogEntries += $@"______________________________________________________________________________________________" + Environment.NewLine;
            SqlLogEntries += $@"Processed Rows: {GetFormattedNumber(_insertedRows)}{Environment.NewLine}";
            SqlLogEntries += $@"Elapsed Time: {totalTime.Elapsed.Hours.ToString()}:{totalTime.Elapsed.Minutes.ToString()}:{totalTime.Elapsed.Seconds.ToString()}{Environment.NewLine}";
            SqlLogEntries += $@"Average row time: { ((float)totalTime.Elapsed.TotalMilliseconds / (float)_insertedRows).ToString()} [ms]";

            Processing = false;
        }


        private void WriteSqlScriptFiles(SQLiteConnection connection)
        {
            int partCounter = 0;
            uint totalNewRows = 0;


            Stopwatch partialTime = new Stopwatch();
            List<DatabaseObjectWrapper> tables;


            DbWrapper dbw = new DbWrapper(_connection);
            dbw.PropertyChanged += (o, e) =>
            {
                ProcessedRowsNumber = (o as DbWrapper).CurrentRow;
            };


            // User
            if (false)
            {
                totalNewRows = (int)(0.5f * 1000000);

                tables = new List<DatabaseObjectWrapper>();
                tables.Add(new UserWrapper(connection));

                if (totalNewRows > 0)
                    NewSqlScriptFile(dbw, tables, ++partCounter, totalNewRows);
            }

            // User relation
            if (true)
            {
                totalNewRows = 1 * 1000000;

                tables = new List<DatabaseObjectWrapper>();
                tables.Add(new UserRelationWrapper(connection));

                if (totalNewRows > 0)
                    NewSqlScriptFile(dbw, tables, ++partCounter, totalNewRows);
            }

            // Measures
            if (false)
            {
                totalNewRows = 1 * 1000000;
                //totalNewRows = 20;

                tables = new List<DatabaseObjectWrapper>();
                tables.Add(new PostWrapper(connection));
                tables.Add(new MeasureWrapper(connection));

                if (totalNewRows > 0)
                    NewSqlScriptFile(dbw, tables, ++partCounter, totalNewRows);
            }
            // FitnessDay
            if (false)
            {
                totalNewRows = 3 * 1000000;
                //totalNewRows = 20;

                tables = new List<DatabaseObjectWrapper>();
                tables.Add(new PostWrapper(connection));
                tables.Add(new FitnessDayWrapper(connection)); partialTime = new Stopwatch();

                if (totalNewRows > 0)
                    NewSqlScriptFile(dbw, tables, ++partCounter, totalNewRows);
            }

            // FitnessDay childs
            if (true)
            {
                totalNewRows = 1 * 1000000;

                tables = new List<DatabaseObjectWrapper>();
                tables.Add(new ActivityDayWrapper(connection));
                tables.Add(new DietDayWrapper(connection));
                tables.Add(new WellnessDayWrapper(connection));
                tables.Add(new WeightWrapper(connection));

                if (totalNewRows > 0)
                    NewSqlScriptFile(dbw, tables, ++partCounter, totalNewRows);
            }

            _insertedRows = dbw.NewRows;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbw">Database wrapper</param>
        /// <param name="tables">Object wrappers of the tables to be processed</param>
        /// <param name="filePart">Counter of the script file to be created</param>
        /// <param name="newRows">Numbers of rwos to be inserted</param>
        private void NewSqlScriptFile(DbWrapper dbw, List<DatabaseObjectWrapper> tables, int filePart, uint newRows )
        {
            // Get number of files to be generated
            ushort totalParts = (ushort)Math.Ceiling((float)newRows / GymAppSQLiteConfig.RowsPerScriptFile);
            string filenameSuffix = $"_part{filePart.ToString()}";

            Stopwatch partialTime = new Stopwatch();
            partialTime.Start();

            // Restart progressbar
            TotalRowsNumber = newRows * tables.Count;

            // Split the script file so it doesn't exceed the maximum number of rows per file
            for (ushort iPart = 0; iPart < totalParts; iPart++)
            {
                // Compute number of rows wrt the number of files
                uint currentNewRows = iPart == totalParts - 1 
                    ? newRows - (iPart * GymAppSQLiteConfig.RowsPerScriptFile) : GymAppSQLiteConfig.RowsPerScriptFile;

                SqlLogEntries += $"[{(iPart + 1).ToString()}/{totalParts.ToString()}] " +
                   $@" Processing tables: {string.Join(", ", tables.Select(x => x.TableName))} {Environment.NewLine}";

                dbw.BasicSqlScriptGenerator(currentNewRows, GetScriptFileFullpath(filenameSuffix, iPart), GymAppSQLiteConfig.SqlTempFilePath, tables);
            }

            partialTime.Stop();
            SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}" +
                $@"  ____________________________________________________________________________________{Environment.NewLine}";

            // Reset row counter
            dbw.CurrentRow = 0;
        }


        private long GetFitnessDayFirstId(SQLiteConnection connection)
        {


            // Get the first Id of FitnessDayEntry which has no child tables
            SQLiteCommand cmd = new SQLiteCommand()
            {
                Connection = connection,
                CommandText = $"SELECT F.Id FROM FitnessDayEntry F LEFT OUTER JOIN ActivityDay A ON F.Id = A.Id " +
                                $"LEFT OUTER JOIN DietDay D ON F.Id = D.Id LEFT OUTER JOIN WellnessDay W ON F.Id = W.Id " +
                                $"LEFT OUTER JOIN Weight WE ON F.Id = WE.Id WHERE A.Id IS NULL AND D.Id IS NULL " +
                                $"AND W.Id IS NULL AND WE.Id IS NULL ORDER BY F.Id LIMIT(1)",
            };

            //SQLiteDataReader sqlr = await Task.Run(() => cmd.ExecuteReaderAsync()) as SQLiteDataReader;
            SQLiteDataReader sqlr = cmd.ExecuteReader();


            if (sqlr.Read())
                return sqlr.GetInt32(0);

            else
                return -1;
        }



        private string GetScriptFileFullpath(string filenameSuffix, ushort partNumber)
        {
            return Regex.Replace(Regex.Replace(GymAppSQLiteConfig.SqlScriptFilePath, "##suffix##", filenameSuffix)
                , @"##part##", partNumber.ToString());
        }


        /// <summary>
        /// Make the input number suitable to be displayed properly. EG: EG: 123700000 --> 123.7M
        /// </summary>
        /// <param name="toBeDisplayed">The number to be displayed</param>
        /// <param name="automaticScaleToInputMagnitude">Tells whether the input should be scaled according to its magnitude. Otherwise use the default scale factor no matther what.</param>
        /// <seealso cref="GymAppSQLiteConfig.DefaultDisplayScaleFactor"/>
        /// <returns>The string to be displayed</returns>
        private string GetFormattedNumber(double toBeDisplayed, bool automaticScaleToInputMagnitude = true)
        {

            float displayScaleFactor = GymAppSQLiteConfig.DefaultDisplayScaleFactor;
            char displayScaleFactorName = GymAppSQLiteConfig.DefaultDisplayScaleFactorName;

            if (automaticScaleToInputMagnitude)
            {
                if (_insertedRows < 1000)
                {
                    displayScaleFactor = 1.0f;
                    displayScaleFactorName = ' ';
                }
                else if (_insertedRows < 1000000)
                {
                    displayScaleFactor = 1000.0f;
                    displayScaleFactorName = 'K';
                }
            }

            return (toBeDisplayed / displayScaleFactor).ToString() + displayScaleFactorName;
        }

    }
}
