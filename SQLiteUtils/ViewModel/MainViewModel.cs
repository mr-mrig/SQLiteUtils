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
using SQLiteUtils;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SQLiteUtils.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {


        #region Consts
        public const string UserTableTemplate = "user";                                                            // Template for the string columns of the User rows
        public const string PostTableTemplate = "post";                                                            // Template for the string columns of the Post rows

        public const uint NotyfyPeriodRows = 500000;                                                                // Number of rows which the user is notified at (via Log row)

        private readonly DateTime DbDateLowerBound = new DateTime(2016, 1, 1);
        private readonly DateTime DbDateUpperBound = new DateTime(2019, 3, 31);
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

            Processing = false;
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



            //
            //  USER TABLE
            //
            if (false)
            {
                tableName = "User";
                totalNewRows = 1000000;

                if (totalNewRows > 0)
                {
                    filenameSuffix = $"_part{(++partCounter).ToString()}";

                    using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, totalParts))))
                    {
                        partialTime.Start();

                        // Get Table last ID
                        maxId = DatabaseUtility.GetTableMaxId(connection, tableName);
                        // Populate it
                        PopulateUserTable(connection, scriptFile, maxId, totalNewRows);
                        // Log
                        partialTime.Stop();

                        SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                    }
                }
            }

            //
            //      User Relations
            //
            if (false)
            {

                tableName = "UserRelation";
                totalNewRows = 0;

                if (totalNewRows > 0)
                {
                    filenameSuffix = $"_part{(++partCounter).ToString()}";

                    // Get initial maxId of the table to be processed
                    maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

                    // Get number of files to be generated
                    totalParts = (ushort)Math.Ceiling((float)totalNewRows / GymAppSQLiteConfig.RowsPerScriptFile);

                    partialTime = new Stopwatch();
                    partialTime.Start();

                    // Split files so they don't exceed the maximum number of rows per file
                    for (ushort iPart = 0; iPart < totalParts; iPart++)
                    {
                        using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, iPart))))
                        {
                            // Compute number of rows wrt the number of files
                            currentNewRows = iPart == totalParts - 1 ? totalNewRows - (uint)(iPart * GymAppSQLiteConfig.RowsPerScriptFile) : GymAppSQLiteConfig.RowsPerScriptFile;
                            // Generate script file
                            maxId = PopulateUserRelationTable(connection, scriptFile, maxId, currentNewRows, tableName);
                        }
                    }

                    partialTime.Stop();
                    SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                }
            }

            //
            //      Post, Measure, FitnessDay
            //
            if (true)
            {
                tableName = "Post";

                // Measures
                totalNewRows = 2 * 1000000;

                if (totalNewRows > 0)
                {
                    filenameSuffix = $"_part{(++partCounter).ToString()}";

                    // Get initial maxId of the table to be processed
                    maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

                    // Get number of files to be generated
                    totalParts = (ushort)Math.Ceiling((float)totalNewRows / GymAppSQLiteConfig.RowsPerScriptFile);

                    partialTime = new Stopwatch();
                    partialTime.Start();

                    // Restart the progressbar
                    TotalRowsNumber = totalNewRows;
                    ProcessedRowsNumber = 0;

                    // Split files so they don't exceed the maximum number of rows per file
                    for (ushort iPart = 0; iPart < totalParts; iPart++)
                    {
                        using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, iPart))))
                        {
                            // Compute number of rows wrt the number of files
                            currentNewRows = iPart == totalParts - 1 ? totalNewRows - (uint)(iPart * GymAppSQLiteConfig.RowsPerScriptFile) : GymAppSQLiteConfig.RowsPerScriptFile;
                            // Generate script file
                            maxId = PopulatePostTable(connection, scriptFile, maxId, currentNewRows, "MeasuresEntry");
                        }
                    }

                    partialTime.Stop();
                    SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                }

                // FitnessDay
                totalNewRows = 50 * 1000000;

                if (totalNewRows > 0)
                {
                    filenameSuffix = $"_part{(++partCounter).ToString()}";

                    // Get initial maxId of the table to be processed
                    maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

                    // Get number of files to be generated
                    totalParts = (ushort)Math.Ceiling((float)totalNewRows / GymAppSQLiteConfig.RowsPerScriptFile);

                    partialTime = new Stopwatch();
                    partialTime.Start();

                    // Restart the progressbar
                    TotalRowsNumber = totalNewRows;
                    ProcessedRowsNumber = 0;

                    // Split files so they don't exceed the maximum number of rows per file
                    for (ushort iPart = 0; iPart < totalParts; iPart++)
                    {
                        using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, iPart))))
                        {
                            // Compute number of rows wrt the number of files
                            currentNewRows = iPart == totalParts - 1 ? totalNewRows - (uint)(iPart * GymAppSQLiteConfig.RowsPerScriptFile) : GymAppSQLiteConfig.RowsPerScriptFile;
                            // Generate script file
                            maxId = PopulatePostTable(connection, scriptFile, maxId, currentNewRows, "FitnessDayEntry");
                        }
                    }

                    partialTime.Stop();
                    SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                }
            }

            //
            //      Post, DietPlan, Phase
            //
            if (false)
            {

                tableName = "Post";
                totalNewRows = 0;

                if (totalNewRows > 0)
                {
                    filenameSuffix = $"_part{(++partCounter).ToString()}";

                    using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, totalParts))))
                    {
                        partialTime.Start();

                        maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

                        maxId = PopulatePostTable(connection, scriptFile, maxId, totalNewRows, "UserPhase");
                        partialTime.Stop();
                        SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";

                        //Populate child2

                        totalNewRows = 1000000;

                        partialTime = new Stopwatch();
                        partialTime.Start();
                        maxId = PopulatePostTable(connection, scriptFile, maxId, totalNewRows, "DietPlan");

                        partialTime.Stop();

                        SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                    }
                }
            }

            //
            //      FitnessDay childs
            //
            if (false)
            {

                totalNewRows = 5000000;

                if (totalNewRows > 0)
                {
                    filenameSuffix = $"_part{(++partCounter).ToString()}";

                    using (StreamWriter scriptFile = new StreamWriter(File.OpenWrite(GetScriptFileFullpath(filenameSuffix, totalParts))))
                    {
                        partialTime = new Stopwatch();
                        partialTime.Start();
                        maxId = (int)GetFitnessDayFirstId(connection);
                        maxId = PopulateFitnessDayChilds(connection, scriptFile, maxId, totalNewRows);
                        // Log
                        partialTime.Stop();

                        SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                    }
                }
            }
        }




        private long PopulateUserTable(SQLiteConnection connection, StreamWriter scriptFile, long firstId, long rowNum)
        {
            string colValue = string.Empty;
            string usernameTemplate;
            string tableName = "User";

            StringBuilder sqlStr = new StringBuilder();

            List<string> columns = new List<string>();
            Dictionary<string, TypeAffinity> colTypes = new Dictionary<string, TypeAffinity>();

            // Restart the progressbar
            TotalRowsNumber = rowNum;
            ProcessedRowsNumber = 0;

            try
            {
                List<int> accountStatusTypes = DatabaseUtility.GetAccountStatusTypeIds(connection);
                int accountStatusTypeIdMin = accountStatusTypes.Min();
                int accountStatusTypeIdMax = accountStatusTypes.Max();

                // Get columns definition
                (columns, colTypes) = DatabaseUtility.GetColumnsDefinition(connection, tableName);

                SqlLogEntries += $@"Processing {tableName.ToUpper()} table [{GetFormattedNumber(rowNum)}]" + Environment.NewLine;
                sqlStr.Append($@"INSERT INTO User ({string.Join(",", columns)}) VALUES");


                // Prepare values to be inserted
                for (long i = firstId; i < firstId + rowNum; i++)
                {
                    // Set the template for the values to be inserted
                    if (i < 9999)
                        usernameTemplate = $"{UserTableTemplate}_{i.ToString("d4")}";
                    else if (i < 99999999)
                        usernameTemplate = $"{UserTableTemplate}_2_{(i - 9999).ToString("d4")}";
                    else if (i < 999999999999)
                        usernameTemplate = $"{UserTableTemplate}_3_{(i - 99999999).ToString("d4")}";
                    else
                    {
                        SqlLogEntries = "Too many rows already inserted in the User table";
                        return i;
                    }

                    sqlStr.Append($@"(");


                    foreach (string colName in columns)
                    {
                        switch (colName)
                        {
                            case "Email":

                                colValue = $@"'{usernameTemplate}@email.com'";
                                break;

                            case "Username":

                                colValue = $@"'{usernameTemplate}'";
                                break;

                            case "Password":

                                colValue = $@"'{usernameTemplate}.myPwd'";
                                break;

                            case "Salt":

                                colValue = $@"'{usernameTemplate}.mySalt'";
                                break;

                            case "AccountStatusTypeId":

                                colValue = RandomFieldGenerator.Rand.Next(accountStatusTypeIdMin, accountStatusTypeIdMax).ToString();
                                break;

                            default:

                                if (colTypes == null)
                                    SqlFail = $"Table {tableName}: not possible to auto-detect columns affinity";
                                else
                                    RandomFieldGenerator.GenerateRandomField(colTypes[colName]);
                                break;
                        }

                        sqlStr.Append($@"{colValue},");
                    }

                    SqlLogEntries += $@"{i.ToString()}/{rowNum.ToString()}" + Environment.NewLine;
                    sqlStr.Remove(sqlStr.Length - 1, 1).Append("),");

                    ProcessedRowsNumber++;
                }
                sqlStr.Remove(sqlStr.Length - 1, 1).Append(";");
            }
            catch (Exception exc)
            {
                return -1;
            }

            // Write the file
            SqlLogEntries = sqlStr.ToString();
            scriptFile.WriteLine(sqlStr);

            // Update the counter
            _insertedRows += rowNum;
            return _insertedRows;
        }



        private long PopulateUserRelationTable(SQLiteConnection connection, StreamWriter scriptFile, long firstId, long rowNum, string tableName)
        {
            string colValue = string.Empty;
            int user1 = 0;

            StringBuilder sqlStr = new StringBuilder();

            List<string> columns = new List<string>();
            Dictionary<string, TypeAffinity> colTypes = new Dictionary<string, TypeAffinity>();

            // Restart the progressbar
            TotalRowsNumber = rowNum;
            ProcessedRowsNumber = 0;

            try
            {
                // Get User Ids
                List<int> userIds = DatabaseUtility.GetTableIds(connection, "User");
                int userIdsMin = userIds.Min();
                int userIdsMax = userIds.Max();

                // Get RelationStatus Ids
                List<int> RelationStatusIds = DatabaseUtility.GetTableIds(connection, "RelationStatus");
                int relationStatusIdMin = userIds.Min();
                int relationStatusIdMax = userIds.Max();

                // Get columns definition
                (columns, colTypes) = DatabaseUtility.GetColumnsDefinition(connection, tableName);

                SqlLogEntries += $@"Processing {tableName.ToUpper()} table [{GetFormattedNumber(rowNum)} rows]" + Environment.NewLine;
                sqlStr.Append($@"INSERT INTO {tableName} ({string.Join(",", columns)}) VALUES ");


                // Prepare values to be inserted
                for (long i = firstId; i < firstId + rowNum; i++)
                {
                    // Set the template for the values to be inserted
                    if (i >= 999999999999)
                    {
                        SqlLogEntries = "Too many rows already inserted in the User table";
                        return i;
                    }


                    sqlStr.Append($@"(");


                    foreach (string colName in columns)
                    {
                        switch (colName)
                        {
                            case "SourceUserId":

                                // Store the Source user to ensure if doesn't appear as Target also
                                user1 = RandomFieldGenerator.RandomInt(userIdsMin, userIdsMax);
                                colValue = user1.ToString();
                                break;

                            case "TargetUserId":

                                colValue = RandomFieldGenerator.RandomIntValueExcluded(userIdsMin, userIdsMax, new List<int> { user1 }).ToString();
                                break;

                            case "LastUpdate":

                                colValue = RandomFieldGenerator.RandomUnixTimestamp(DbDateLowerBound, DbDateUpperBound).ToString();
                                break;


                            case "RelationStatusId":

                                colValue = new Random().Next(relationStatusIdMin, relationStatusIdMax).ToString();
                                break;

                            default:

                                RandomFieldGenerator.GenerateRandomField(colTypes[colName]);
                                break;
                        }

                        sqlStr.Append($@"{colValue},");
                    }

                    SqlLogEntries += $@"{i.ToString()}/{rowNum.ToString()}" + Environment.NewLine;
                    sqlStr.Remove(sqlStr.Length - 1, 1).Append("),");

                    ProcessedRowsNumber++;
                }
                sqlStr.Remove(sqlStr.Length - 1, 1).Append(";");
            }
            catch (Exception exc)
            {
                return -1;
            }

            // Write the file
            SqlLogEntries = sqlStr.ToString();
            scriptFile.WriteLine(sqlStr);


            // Update the counter
            _insertedRows += rowNum;
            return _insertedRows;
        }


        /// <summary>
        /// Populate the Post Table together with one of its child tables. This algorithm exploits temporary files which is the best method memory-wise
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="firstId"></param>
        /// <param name="rowNum"></param>
        /// <param name="childTableName"></param>
        /// <returns>Returns the new maxId</returns>
        private long PopulatePostTable(SQLiteConnection connection, StreamWriter scriptFile, long firstId, uint rowNum, string childTableName)
        {
            string colValue = string.Empty;
            string postTemplate;
            string parentTableName = "Post";
            int startDate = 0;
            int tempFileCounter = 0;

            // Write temp file instead of using big StringBuilders in order to save memory
            StreamWriter tempFileW1;
            StreamWriter tempFileW2;
            StringBuilder sqlStr1 = new StringBuilder();
            StringBuilder sqlStr2 = new StringBuilder();
            string fname1 = Path.Combine(GymAppSQLiteConfig.SqlTempFilePath + $"{(tempFileCounter++).ToString()}");
            string fname2 = Path.Combine(GymAppSQLiteConfig.SqlTempFilePath + $"{(tempFileCounter++).ToString()}");

            try
            {
                tempFileW1 = new StreamWriter(File.OpenWrite(fname1));
                tempFileW2 = new StreamWriter(File.OpenWrite(fname2));
            }
            catch (Exception exc)
            {
                SqlFail = exc.Message;
                return -1;
            }

            List<string> parentColumns = new List<string>();
            List<string> childColumns = new List<string>();
            Dictionary<string, TypeAffinity> parentColTypes = new Dictionary<string, TypeAffinity>();
            Dictionary<string, TypeAffinity> childColTypes = new Dictionary<string, TypeAffinity>();

            try
            {
                int maxUserId = DatabaseUtility.GetTableMaxId(connection, "User");
                int minUserId = 1;
                int maxPhaseId = DatabaseUtility.GetTableMaxId(connection, "Phase");
                int minPhaseId = 1;
                int maxPhaseAnnotationId = DatabaseUtility.GetTableMaxId(connection, "UserPhaseAnnotation");
                int minPhaseAnnotationId = 1;

                // Get columns definition
                (parentColumns, parentColTypes) = DatabaseUtility.GetColumnsDefinition(connection, parentTableName);
                (childColumns, childColTypes) = DatabaseUtility.GetColumnsDefinition(connection, childTableName);


                SqlLogEntries += $@"Processing {parentTableName.ToUpper()} table [{GetFormattedNumber(rowNum)} rows]" + Environment.NewLine;
                SqlLogEntries += $@"Processing {childTableName.ToUpper()} table [{GetFormattedNumber(rowNum)} rows]" + Environment.NewLine;

                tempFileW1.WriteLine($@";{Environment.NewLine} INSERT INTO {parentTableName} ({string.Join(",", parentColumns)}) VALUES");
                tempFileW2.WriteLine($@";{Environment.NewLine} INSERT INTO {childTableName} ({string.Join(",", childColumns)}) VALUES");


                // Prepare values to be inserted
                for (long i = firstId; i < firstId + rowNum; i++)
                {

                    // Set the template for the values to be inserted (to avoid duplicate rows)
                    if (i < 9999)
                        postTemplate = $"{PostTableTemplate}_{i.ToString("d4")}";
                    else if (i < 99999999)
                        postTemplate = $"{PostTableTemplate}_2_{(i - 9999).ToString("d4")}";
                    else if (i < 999999999999)
                        postTemplate = $"{PostTableTemplate}_3_{(i - 99999999).ToString("d4")}";
                    else
                    {
                        SqlLogEntries = $"Too many rows already inserted in the {parentTableName} table";
                        return i;
                    }

                    postTemplate = $"{PostTableTemplate}_{i.ToString("d4")}";

                    tempFileW1.Write($@"(");
                    tempFileW2.Write($@"(");

                    // POST
                    foreach (string colName in parentColumns)
                    {
                        switch (colName)
                        {
                            case "Caption":

                                colValue = $@"'{postTemplate}{RandomFieldGenerator.RandomTextValue(new Random().Next(10, 250))}'";
                                break;

                            case "IsPublic":

                                colValue = $@"'{RandomFieldGenerator.RandomBoolWithProbability(0.7f)}'";
                                break;

                            case "CreatedOn":

                                colValue = $@"{RandomFieldGenerator.RandomUnixTimestamp(DbDateLowerBound, DbDateUpperBound).ToString()}";
                                break;

                            case "LastUpdate":

                                colValue = $@"{RandomFieldGenerator.RandomDateTimeNullAllowed(DbDateLowerBound, DbDateUpperBound, 0.6f)}";
                                break;

                            case "UserId":

                                colValue = RandomFieldGenerator.RandomInt(minUserId, maxUserId).ToString();
                                break;

                            default:

                                if (parentColTypes == null)
                                    SqlFail = $"Table {parentTableName}: not possible to auto-detect columns affinity";
                                else
                                    RandomFieldGenerator.GenerateRandomField(parentColTypes[colName]);
                                break;
                        }

                        sqlStr1.Append($@"{colValue},");
                    }

                    // CHILD
                    foreach (string colName1 in childColumns)
                    {
                        switch (colName1)
                        {
                            case "OwnerNote":

                                colValue = $@"'{postTemplate}{RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.Rand.Next(10, 250))}'";
                                break;

                            case "Rating":

                                colValue = $@"{RandomFieldGenerator.RandomInt(0, 5)}";
                                break;

                            case "Date":

                                colValue = $@"{RandomFieldGenerator.RandomUnixDate(DbDateLowerBound, DbDateUpperBound).ToString()}";
                                break;


                            case "LastUpdate":

                                colValue = $@"{RandomFieldGenerator.RandomDateTimeNullAllowed(DbDateLowerBound, DbDateUpperBound, 0.6f)}";
                                break;

                            case "CreatedOn":

                                colValue = $@"{RandomFieldGenerator.RandomUnixTimestamp(DbDateLowerBound, DbDateUpperBound).ToString()}";
                                break;

                            case "StartDate":

                                startDate = RandomFieldGenerator.RandomUnixDate(DbDateLowerBound, DbDateUpperBound);
                                colValue = $@"{startDate.ToString()}";
                                break;

                            case "EndDate":

                                if (startDate > 0)
                                {
                                    colValue = $@"{RandomFieldGenerator.RandomUnixDate(startDate, DatabaseUtility.UnixTimestampOneMonthDelta, DatabaseUtility.UnixTimestampSixMonthsDelta).ToString()}";
                                    startDate = 0;
                                }
                                else
                                    colValue = $@"{RandomFieldGenerator.RandomUnixDate(DbDateLowerBound, DbDateUpperBound).ToString()}";
                                break;

                            case "OwnerId":

                                colValue = RandomFieldGenerator.RandomInt(minUserId, maxUserId).ToString();
                                break;

                            case "PhaseId":

                                colValue = RandomFieldGenerator.RandomInt(minPhaseId, maxPhaseId).ToString();
                                break;

                            case "UserPhaseAnnotationId":

                                colValue = RandomFieldGenerator.RandomIntNullAllowed(minPhaseAnnotationId, maxPhaseAnnotationId, 0.6f);
                                break;

                            case "Name":

                                colValue = $"'{RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.Rand.Next(5, 25))}'";
                                break;

                            case "WeeklyFreeMealsNumber":

                                colValue = RandomFieldGenerator.RandomIntNullAllowed(0, 3, 0.3f);
                                break;


                            default:

                                if (childColTypes == null)
                                    SqlFail = $"Table {childTableName}: not possible to auto-detect columns affinity";
                                else
                                    RandomFieldGenerator.GenerateRandomField(childColTypes[colName1]);
                                break;
                        }

                        sqlStr2.Append($@"{colValue},");
                    }

                    tempFileW1.Write(sqlStr1.Remove(sqlStr1.Length - 1, 1).Append("),"));
                    tempFileW2.Write(sqlStr2.Remove(sqlStr2.Length - 1, 1).Append("),"));
                    sqlStr1.Clear();
                    sqlStr2.Clear();

                    ProcessedRowsNumber++;
                }

                tempFileW1.Close();
                tempFileW2.Close();
            }
            catch (Exception exc)
            {
                SqlFail = exc.Message;
                return -1;
            }

            // Write the SQL script
            try
            {
                // Need to open the file again to remove last char
                using (FileStream fs = File.Open(fname1, FileMode.Open, FileAccess.ReadWrite))
                {
                    fs.SetLength(fs.Length - 1);

                    // Copy to script file
                    fs.CopyTo(scriptFile.BaseStream);
                    fs.Flush();
                }

                scriptFile.WriteLine(";");

                // Need to open the file again to remove last char
                using (FileStream fs = File.Open(fname2, FileMode.Open, FileAccess.ReadWrite))
                {
                    fs.SetLength(fs.Length - 1);

                    fs.CopyTo(scriptFile.BaseStream);
                    fs.Flush();
                }
                scriptFile.WriteLine(";");
            }
            catch (Exception exc)
            {
                SqlFail = exc.Message;
                return -1;
            }
            finally
            {
                // Delete temp files
                File.Delete(fname1);
                File.Delete(fname2);
            }

            // Update the counter
            _insertedRows += (rowNum * 2);

            return firstId + rowNum;
        }



        /// <summary>
        /// Populate the tables linked to FitnessActivityDate
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="firstId"></param>
        /// <param name="rowNum"></param>
        /// <returns>Returns the new maxId</returns>
        private long PopulateFitnessDayChilds(SQLiteConnection connection, StreamWriter scriptFile, long firstId, uint rowNum)
        {
            string colValue = string.Empty;
            string tableName = "FitnessDay child tables";


            StringBuilder sqlStr1 = new StringBuilder();
            StringBuilder sqlStr2 = new StringBuilder();
            StringBuilder sqlStr3 = new StringBuilder();
            StringBuilder sqlStr4 = new StringBuilder();

            List<string> columns1 = new List<string>();
            List<string> columns2 = new List<string>();
            List<string> columns3 = new List<string>();
            List<string> columns4 = new List<string>();

            Dictionary<string, TypeAffinity> colTypes1 = new Dictionary<string, TypeAffinity>();
            Dictionary<string, TypeAffinity> colTypes2 = new Dictionary<string, TypeAffinity>();
            Dictionary<string, TypeAffinity> colTypes3 = new Dictionary<string, TypeAffinity>();
            Dictionary<string, TypeAffinity> colTypes4 = new Dictionary<string, TypeAffinity>();

            // Restart the progressbar
            TotalRowsNumber = rowNum;
            ProcessedRowsNumber = 0;


            try
            {
                // Get columns definition
                (columns1, colTypes1) = DatabaseUtility.GetColumnsDefinition(connection, "ActivityDay", false);
                (columns2, colTypes2) = DatabaseUtility.GetColumnsDefinition(connection, "DietDay", false);
                (columns3, colTypes3) = DatabaseUtility.GetColumnsDefinition(connection, "WellnessDay", false);
                (columns4, colTypes4) = DatabaseUtility.GetColumnsDefinition(connection, "Weight", false);

                SqlLogEntries += $@"Processing {tableName} table [{GetFormattedNumber(rowNum)}]..." + Environment.NewLine;


                sqlStr1.Append($@"INSERT INTO ActivityDay ({string.Join(",", columns1)}) VALUES");
                sqlStr2.Append($@"INSERT INTO DietDay ({string.Join(",", columns2)}) VALUES");
                sqlStr3.Append($@"INSERT INTO WellnessDay ({string.Join(",", columns3)}) VALUES");
                sqlStr4.Append($@"INSERT INTO Weight ({string.Join(",", columns4)}) VALUES");


                // Prepare values to be inserted
                for (long i = firstId; i < firstId + rowNum; i++)
                {
                    bool atLeastOneEntry = false;


                    // Table1
                    if (RandomFieldGenerator.Rand.NextDouble() <= FitnessDayProbabilityArray[0])
                    {
                        atLeastOneEntry = true;
                        sqlStr1.Append($@"(");

                        foreach (string colName in columns1)
                        {
                            switch (colName)
                            {

                                case "Id":

                                    colValue = i.ToString();
                                    break;

                                case "Steps":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(4000, 30000, 0.05f);
                                    break;

                                case "CaloriesOut":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(600, 9999, 0.05f);
                                    break;

                                case "Stairs":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(0, 500, 0.1f);
                                    break;

                                case "SleepMinutes":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(0, 800, 0.1f);
                                    break;

                                case "SleepQuality":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(0, 5, 0.15f);
                                    break;

                                case "HeartRateRest":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(20, 100, 0.15f);
                                    break;

                                case "HeartRateMax":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(100, 220, 0.15f);
                                    break;

                                default:

                                    if (colTypes1 == null)
                                        SqlFail = $"Table ActivityDay: not possible to auto-detect columns affinity";
                                    else
                                        RandomFieldGenerator.GenerateRandomField(colTypes1[colName]);
                                    break;
                            }

                            sqlStr1.Append($@"{colValue},");
                        }
                        sqlStr1.Remove(sqlStr1.Length - 1, 1).Append("),");
                    }

                    // Table2
                    if (RandomFieldGenerator.Rand.NextDouble() <= FitnessDayProbabilityArray[1])
                    {
                        atLeastOneEntry = true;
                        sqlStr2.Append($@"(");

                        foreach (string colName in columns2)
                        {
                            switch (colName)
                            {

                                case "Id":

                                    colValue = i.ToString();
                                    break;

                                case "CarbGrams":

                                    colValue = RandomFieldGenerator.RandomInt(0, 900).ToString();
                                    break;

                                case "FatGrams":

                                    colValue = RandomFieldGenerator.RandomInt(0, 300).ToString();
                                    break;

                                case "ProteinGrams":

                                    colValue = RandomFieldGenerator.RandomInt(0, 500).ToString();
                                    break;

                                case "SodiumMg":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(0, 800, 0.9f);
                                    break;

                                case "WaterLiters":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(0, 5, 0.7f);
                                    break;

                                case "IsFreeMeal":

                                    colValue = RandomFieldGenerator.RandomInt(0, 1).ToString();
                                    break;

                                default:

                                    if (colTypes2 == null)
                                        SqlFail = $"Table DietDay: not possible to auto-detect columns affinity";
                                    else
                                        RandomFieldGenerator.GenerateRandomField(colTypes2[colName]);
                                    break;
                            }

                            sqlStr2.Append($@"{colValue},");
                        }
                        sqlStr2.Remove(sqlStr2.Length - 1, 1).Append("),");
                    }


                    // Table3
                    if (RandomFieldGenerator.Rand.NextDouble() <= FitnessDayProbabilityArray[2])
                    {
                        atLeastOneEntry = true;
                        sqlStr3.Append($@"(");

                        foreach (string colName in columns3)
                        {
                            switch (colName)
                            {

                                case "Id":

                                    colValue = i.ToString();
                                    break;

                                case "Temperature":

                                    colValue = RandomFieldGenerator.RandomDouble(34, 38, 1).ToString(currentCulture);
                                    break;

                                case "Glycemia":

                                    colValue = RandomFieldGenerator.RandomIntNullAllowed(0, 150, 0.85f).ToString();
                                    break;


                                default:

                                    if (colTypes3 == null)
                                        SqlFail = $"Table WellnessDay: not possible to auto-detect columns affinity";
                                    else
                                        RandomFieldGenerator.GenerateRandomField(colTypes3[colName]);
                                    break;
                            }

                            sqlStr3.Append($@"{colValue},");
                        }
                        sqlStr3.Remove(sqlStr3.Length - 1, 1).Append("),");
                    }

                    // Table4
                    if (RandomFieldGenerator.Rand.NextDouble() <= FitnessDayProbabilityArray[3] || !atLeastOneEntry)
                    {
                        sqlStr4.Append($@"(");

                        foreach (string colName in columns4)
                        {
                            switch (colName)
                            {

                                case "Id":

                                    colValue = i.ToString();
                                    break;

                                case "Kg":

                                    colValue = RandomFieldGenerator.RandomDouble(35.0, 120.0, 1).ToString(currentCulture);
                                    break;

                                default:

                                    if (colTypes4 == null)
                                        SqlFail = $"Table Weight: not possible to auto-detect columns affinity";
                                    else
                                        RandomFieldGenerator.GenerateRandomField(colTypes4[colName]);
                                    break;
                            }

                            sqlStr4.Append($@"{colValue},");
                        }
                        sqlStr4.Remove(sqlStr4.Length - 1, 1).Append("),");
                    }

                    ProcessedRowsNumber++;
                }

                sqlStr1.Remove(sqlStr1.Length - 1, 1).Append(";");
                sqlStr2.Remove(sqlStr2.Length - 1, 1).Append(";");
                sqlStr3.Remove(sqlStr3.Length - 1, 1).Append(";");
                sqlStr4.Remove(sqlStr4.Length - 1, 1).Append(";");
            }
            catch (Exception exc)
            {
                return -1;
            }

            // Write the SQL script
            scriptFile.WriteLine(sqlStr1);
            scriptFile.WriteLine();
            scriptFile.WriteLine(sqlStr2);
            scriptFile.WriteLine();
            scriptFile.WriteLine(sqlStr3);
            scriptFile.WriteLine();
            scriptFile.WriteLine(sqlStr4);

            // Update the counter
            _insertedRows += (rowNum * 2);

            return firstId + rowNum;
        }



        //private long PopulateTrainingPlan(SQLiteConnection connection, StreamWriter scriptFile, long firstId, long rowNum, string tableName)
        //{
        //    string colValue = string.Empty;
        //    string trainingNameTemplate;

        //    StringBuilder sqlStr = new StringBuilder();

        //    List<string> columns = new List<string>();
        //    Dictionary<string, TypeAffinity> colTypes = new Dictionary<string, TypeAffinity>();

        //    // Restart the progressbar
        //    TotalRowsNumber = rowNum;
        //    ProcessedRowsNumber = 0;

        //    try
        //    {
        //        // Get columns definition
        //        (columns, colTypes) = DatabaseUtility.GetColumnsDefinition(connection, tableName);

        //        SqlLogEntries += $@"Processing {tableName.ToUpper()} table [{GetFormattedNumber(rowNum)} rows]" + Environment.NewLine;
        //        sqlStr.Append($@"INSERT INTO {tableName} ({string.Join(",", columns)}) VALUES ");


        //        // Prepare values to be inserted
        //        for (long i = firstId; i < firstId + rowNum; i++)
        //        {
        //            // Set the template for the values to be inserted
        //            if (i < 9999)
        //                trainingNameTemplate = $"{TrainingTableTemplate}_{i.ToString("d4")}";
        //            else if (i < 99999999)
        //                trainingNameTemplate = $"{TrainingTableTemplate}_2_{(i - 9999).ToString("d4")}";
        //            else if (i < 999999999999)
        //                trainingNameTemplate = $"{TrainingTableTemplate}_3_{(i - 99999999).ToString("d4")}";
        //            else
        //            {
        //                SqlLogEntries = "Too many rows already inserted in the User table";
        //                return i;
        //            }

        //            sqlStr.Append($@"(");


        //            foreach (string colName in columns)
        //            {
        //                switch (colName)
        //                {
        //                    case "Email":

        //                        colValue = $@"'{trainingNameTemplate}@email.com'";
        //                        break;

        //                    case "Username":

        //                        colValue = $@"'{trainingNameTemplate}'";
        //                        break;

        //                    case "Password":

        //                        colValue = $@"'{trainingNameTemplate}.myPwd'";
        //                        break;

        //                    case "Salt":

        //                        colValue = $@"'{trainingNameTemplate}.mySalt'";
        //                        break;

        //                    case "AccountStatusTypeId":

        //                        colValue = RandomFieldGenerator.Rand.Next(accountStatusTypeIdMin, accountStatusTypeIdMax).ToString();
        //                        break;

        //                    default:

        //                        RandomFieldGenerator.GenerateRandomField(colTypes[colName]);
        //                        break;
        //                }

        //                sqlStr.Append($@"{colValue},");
        //            }

        //            SqlLogEntries += $@"{i.ToString()}/{rowNum.ToString()}" + Environment.NewLine;
        //            sqlStr.Remove(sqlStr.Length - 1, 1).Append("),");

        //            ProcessedRowsNumber++;
        //        }
        //        sqlStr.Remove(sqlStr.Length - 1, 1).Append(";");
        //    }
        //    catch (Exception exc)
        //    {
        //        return -1;
        //    }

        //    // Write the file
        //    SqlLogEntries = sqlStr.ToString();
        //    scriptFile.WriteLine(sqlStr);

        //    // Update the counter
        //    _insertedRows += rowNum;
        //    return _insertedRows;
        //}


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
