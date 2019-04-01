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
using SQLiteUtils.Util;

namespace SQLiteUtils.ViewModel
{



    public class TableProcessData
    {
        public string TableName { get; set; }
        public bool Enabled { get; set; }
        public uint TotalRows { get; set; }
        public ushort OrderNumber { get; set; }
    }




    public class DbGeneratorManagerViewModel : BaseViewModel
    {

        #region Consts
        public const uint NotyfyPeriodRows = 500000;                                                                // Number of rows which the user is notified at (via Log row)


        private readonly float[] FitnessDayProbabilityArray = new float[4] { 0.5f, 0.9f, 0.3f, 0.85f };

        private readonly CultureInfo currentCulture = CultureInfo.GetCultureInfo("en-US");                          // Dot as decimal separator
        #endregion


        #region Private Fields
        private long _insertedRows = 0;
        private SQLiteConnection _connection = null;            // Global to avoid DB locking issues
        private Action<bool> _isProcessingChangedAction = null;       // Action to be performed when processing starts/ends
        private Action<string> _onErrorAction= null;                    // Action to be performed when an error is raised
        #endregion



        #region INotifyPropertyChanged Implementation

        private ObservableCollection<TableProcessData> _processTablesData;
        public ObservableCollection<TableProcessData> ProcessTablesData
        {
            get => _processTablesData;
            set => SetProperty(ref _processTablesData, value);
        }


        private long _totalRowsNumber = long.MaxValue;
        public long TotalRowsNumber
        {
            get => _totalRowsNumber;
            set => SetProperty(ref _totalRowsNumber, value);
        }

        private long _processedRowsNumber = 0;
        public long ProcessedRowsNumber
        {
            get => _processedRowsNumber;
            set => SetProperty(ref _processedRowsNumber, value);
        }

        private string _sqlLogEntries = string.Empty;
        public string SqlLogEntries
        {
            get => _sqlLogEntries;
            set => SetProperty(ref _sqlLogEntries, value);
        }

        private string _sqlTableFailed = string.Empty;
        public string SqlFail
        {
            get => _sqlTableFailed;
            private set => SetProperty(ref _sqlTableFailed, value);
        }

        private ParameterlessCommand _resetProcessTableDataCommand;
        public ParameterlessCommand ResetProcessTableDataCommand { get; set; }


        public ParameterlessCommandAsync _initDatabaseCommand;
        public ParameterlessCommandAsync InitDatabaseCommand
        {
            get => _initDatabaseCommand;
            private set => SetProperty(ref _initDatabaseCommand, value);
        }

        public ParameterlessCommandAsync _executeSqlCommand;
        public ParameterlessCommandAsync ExecuteSqlCommand
        {
            get => _executeSqlCommand;
            private set => SetProperty(ref _executeSqlCommand, value);
        }

        private bool _isProcessing = false;
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                if (SetProperty(ref _isProcessing, value))
                    _isProcessingChangedAction?.Invoke(value);
            }
        }

        private bool _executingSql = false;
        public bool ExecutingSql
        {
            get => _executingSql;
            set
            {
                SetProperty(ref _executingSql, value);

                // Link to Processing
                IsProcessing = value;
            }
        }
        #endregion



        #region Properties
        public string DbName { get; set; }

        public string ViewTitle { get; set; } = "Populate";
        #endregion



        #region Ctors

        /// <summary>
        /// ViewModel for the Database Generator
        /// </summary>
        /// <param name="isProcessingChangedAction">Action to be performed when the processing starts/ends</param>
        public DbGeneratorManagerViewModel(Action<bool> isProcessingChangedAction, Action<string> onErrorAction)
        {
            InitDatabaseCommand = new ParameterlessCommandAsync(GenerateSqlScriptWrapperAync, () => !IsProcessing);
            ExecuteSqlCommand = new ParameterlessCommandAsync(ExecuteSql, () => !IsProcessing);
            ResetProcessTableDataCommand = new ParameterlessCommand(InitProcessTableData, () => !IsProcessing);

            // Decimal separator as dot, not comma
            CultureInfo.DefaultThreadCurrentCulture = currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = currentCulture;

            _isProcessingChangedAction = isProcessingChangedAction;
            _onErrorAction = onErrorAction;

            InitProcessTableData();

            BulkInsertScriptDbWriter dbWriter = new BulkInsertScriptDbWriter($@"D:\Gym App\SQLite\Databases\Script", $@"D:\Gym App\SQLite\Databases\Db WrapperTest.db");

            DbWrapper dbWrapper = new DbWrapper(dbWriter);

            dbWrapper.InsertUsers(DateTime.Today, DateTime.Today.AddDays(180), 2);


            dbWrapper.Dispose();
        }
        #endregion



        private void InitProcessTableData()
        {
            ProcessTablesData = new ObservableCollection<TableProcessData>()
            {
                new TableProcessData() { TableName = "User", Enabled = true, TotalRows = 0, OrderNumber = 0 },
                new TableProcessData() { TableName = "Post", Enabled = true, TotalRows = 0, OrderNumber = 1 },
                new TableProcessData() { TableName = "Measure", Enabled = true, TotalRows = 0, OrderNumber = 2 },
                new TableProcessData() { TableName = "FitnessDay", Enabled = true, TotalRows = 0, OrderNumber = 3 },
                new TableProcessData() { TableName = "Phase", Enabled = true, TotalRows = 0, OrderNumber = 4 },
                new TableProcessData() { TableName = "DietPlan", Enabled = true, TotalRows = 0, OrderNumber = 5 },
                new TableProcessData() { TableName = "Comments", Enabled = false, TotalRows = 0, OrderNumber = 6 },
                new TableProcessData() { TableName = "TrainingPlan", Enabled = false, TotalRows = 0, OrderNumber = 7 },
                new TableProcessData() { TableName = "TrainingDay", Enabled = false, TotalRows = 0, OrderNumber = 8 },
            };
        }


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

                        _connection = DatabaseUtility.OpenFastestSQLConnection(_connection, DbName);
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
            IsProcessing = true;
            ProcessedRowsNumber = 0;

            // Delete old files
            foreach (string filePath in GymAppSQLiteConfig.GetScriptFilesPath())
                File.Delete(filePath);


            SqlLogEntries += Environment.NewLine;
            SqlLogEntries += Environment.NewLine;

            //SQLite connection
            _connection = DatabaseUtility.OpenFastestSQLConnection(_connection, DbName);

            try
            {
                await Task.Run(() => GenerateSqlScript(_connection));
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

            IsProcessing = false;
        }


        private void GenerateSqlScript(SQLiteConnection connection)
        {

            uint totalNewRows = 0;
            uint currentNewRows = 0;
            long maxId = 0;
            ushort totalParts = 0;

            Stopwatch partialTime = new Stopwatch();

            string tableName;
            string filenameSuffix;
            int partCounter = 0;

            UserWrapper user = new UserWrapper(_connection);
            user.Create();

            //
            //  USER TABLE
            //
            if (ProcessTablesData.Where(x => x.TableName == "User").First().Enabled && ProcessTablesData.Where(x => x.TableName == "User").First().TotalRows > 0)
            {
                tableName = "User";
                totalNewRows = ProcessTablesData.Where(x => x.TableName == "User").First().TotalRows;
                totalNewRows = 100000;

                if (totalNewRows > 0)
                {
                    filenameSuffix = $"_part{(++partCounter).ToString()}";

                    using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, totalParts))))
                    {
                        partialTime.Start();

                        // Get Table last ID
                        maxId = DatabaseUtility.GetTableMaxId(connection, tableName);
                        // Populate it
                        //PopulateUserTable(connection, scriptFile, maxId, totalNewRows);
                        // Log
                        partialTime.Stop();

                        SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                    }
                }
            }

            ////
            ////      User Relations
            ////
            //if (false)
            //{

            //    tableName = "UserRelation";
            //    totalNewRows = 0;

            //    if (totalNewRows > 0)
            //    {
            //        filenameSuffix = $"_part{(++partCounter).ToString()}";

            //        // Get initial maxId of the table to be processed
            //        maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

            //        // Get number of files to be generated
            //        totalParts = (ushort)Math.Ceiling((float)totalNewRows / GymAppSQLiteConfig.RowsPerScriptFile);

            //        partialTime = new Stopwatch();
            //        partialTime.Start();

            //        // Split files so they don't exceed the maximum number of rows per file
            //        for (ushort iPart = 0; iPart < totalParts; iPart++)
            //        {
            //            using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, iPart))))
            //            {
            //                // Compute number of rows wrt the number of files
            //                currentNewRows = iPart == totalParts - 1 ? totalNewRows - (uint)(iPart * GymAppSQLiteConfig.RowsPerScriptFile) : GymAppSQLiteConfig.RowsPerScriptFile;
            //                // Generate script file
            //                maxId = PopulateUserRelationTable(connection, scriptFile, maxId, currentNewRows, tableName);
            //            }
            //        }

            //        partialTime.Stop();
            //        SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
            //    }
            //}

            ////
            ////      Post, Measure, FitnessDay
            ////
            //if (false)
            //{
            //    tableName = "Post";

            //    // Measures
            //    totalNewRows = 2 * 1000000;

            //    if (totalNewRows > 0)
            //    {
            //        filenameSuffix = $"_part{(++partCounter).ToString()}";

            //        // Get initial maxId of the table to be processed
            //        maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

            //        // Get number of files to be generated
            //        totalParts = (ushort)Math.Ceiling((float)totalNewRows / GymAppSQLiteConfig.RowsPerScriptFile);

            //        partialTime = new Stopwatch();
            //        partialTime.Start();

            //        // Restart the progressbar
            //        TotalRowsNumber = totalNewRows;
            //        ProcessedRowsNumber = 0;

            //        // Split files so they don't exceed the maximum number of rows per file
            //        for (ushort iPart = 0; iPart < totalParts; iPart++)
            //        {
            //            using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, iPart))))
            //            {
            //                // Compute number of rows wrt the number of files
            //                currentNewRows = iPart == totalParts - 1 ? totalNewRows - (uint)(iPart * GymAppSQLiteConfig.RowsPerScriptFile) : GymAppSQLiteConfig.RowsPerScriptFile;
            //                // Generate script file
            //                maxId = PopulatePostTable(connection, scriptFile, maxId, currentNewRows, "MeasuresEntry");
            //            }
            //        }

            //        partialTime.Stop();
            //        SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
            //    }

            //    // FitnessDay
            //    totalNewRows = 50 * 1000000;

            //    if (totalNewRows > 0)
            //    {
            //        filenameSuffix = $"_part{(++partCounter).ToString()}";

            //        // Get initial maxId of the table to be processed
            //        maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

            //        // Get number of files to be generated
            //        totalParts = (ushort)Math.Ceiling((float)totalNewRows / GymAppSQLiteConfig.RowsPerScriptFile);

            //        partialTime = new Stopwatch();
            //        partialTime.Start();

            //        // Restart the progressbar
            //        TotalRowsNumber = totalNewRows;
            //        ProcessedRowsNumber = 0;

            //        // Split files so they don't exceed the maximum number of rows per file
            //        for (ushort iPart = 0; iPart < totalParts; iPart++)
            //        {
            //            using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, iPart))))
            //            {
            //                // Compute number of rows wrt the number of files
            //                currentNewRows = iPart == totalParts - 1 ? totalNewRows - (uint)(iPart * GymAppSQLiteConfig.RowsPerScriptFile) : GymAppSQLiteConfig.RowsPerScriptFile;
            //                // Generate script file
            //                maxId = PopulatePostTable(connection, scriptFile, maxId, currentNewRows, "FitnessDayEntry");
            //            }
            //        }

            //        partialTime.Stop();
            //        SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
            //    }
            //}

            ////
            ////      Post, DietPlan, Phase
            ////
            //if (false)
            //{

            //    tableName = "Post";
            //    totalNewRows = 0;

            //    if (totalNewRows > 0)
            //    {
            //        filenameSuffix = $"_part{(++partCounter).ToString()}";

            //        using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, totalParts))))
            //        {
            //            partialTime.Start();

            //            maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

            //            maxId = PopulatePostTable(connection, scriptFile, maxId, totalNewRows, "UserPhase");
            //            partialTime.Stop();
            //            SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";

            //            //Populate child2

            //            totalNewRows = 1000000;

            //            partialTime = new Stopwatch();
            //            partialTime.Start();
            //            maxId = PopulatePostTable(connection, scriptFile, maxId, totalNewRows, "DietPlan");

            //            partialTime.Stop();

            //            SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
            //        }
            //    }
            //}

            ////
            ////      FitnessDay childs
            ////
            //if (false)
            //{

            //    totalNewRows = 5000000;

            //    if (totalNewRows > 0)
            //    {
            //        filenameSuffix = $"_part{(++partCounter).ToString()}";

            //        using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, totalParts))))
            //        {
            //            partialTime = new Stopwatch();
            //            partialTime.Start();
            //            maxId = (int)GetFitnessDayFirstId(connection);
            //            maxId = PopulateFitnessDayChilds(connection, scriptFile, maxId, totalNewRows);
            //            // Log
            //            partialTime.Stop();

            //            SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
            //        }
            //    }
            //}
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


        #region Not Used
        private bool PopulateDummyTable(SQLiteConnection connection, string tableName, long rowNum, bool hasID = true)
        {
            string columns = string.Empty;
            string colValue = string.Empty;
            List<TypeAffinity> colTypes = new List<TypeAffinity>();


            // Get columns definition
            SQLiteCommand cmd = new SQLiteCommand()
            {
                Connection = connection,
                CommandText = $"SELECT * FROM {tableName} LIMIT(1)",
            };

            SQLiteDataReader sqlr = cmd.ExecuteReader();

            while (sqlr.Read())
            {
                // Fetch columns name (skip ID column)
                for (int icol = hasID ? 1 : 0; icol < sqlr.FieldCount; icol++)
                {
                    columns += $"{sqlr.GetName(icol)} ,";
                    colTypes.Add(sqlr.GetFieldAffinity(icol));
                }
                // Remove last comma
                columns = columns.Substring(0, columns.Length - 1);

                SqlLogEntries += columns + Environment.NewLine;
            }

            cmd = new SQLiteCommand()
            {
                Connection = connection,
                CommandText = $"INSERT INTO {tableName} ({columns}) VALUES "
            };

            foreach (TypeAffinity colType in colTypes)
            {
                switch (colType)
                {
                    case TypeAffinity.Text:

                        colValue = $"'{""}'";
                        break;

                    case TypeAffinity.Int64:

                        colValue = $"'{new Random().Next(0, Int32.MaxValue)}'";
                        break;
                }
            }

            return true;
        }
        #endregion
    }
}
