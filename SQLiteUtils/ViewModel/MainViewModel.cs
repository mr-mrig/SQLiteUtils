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

namespace SQLiteUtils.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {


        #region Consts
        private const string SqlScriptFileName = @"C:\SQLite\Databases\PopulateTablesScript.sql";           // File holding the SQL statements
        private const string DbName = @"C:\SQLite\Databases\GymApp.db";                                     // Database name
        private const string UserTableTemplate = "user";                                                            // Template for the string columns of the User rows
        private const string PostTableTemplate = "post";                                                            // Template for the string columns of the Post rows
        private const uint NotyfyPeriodRows = 500000;                                                       // Number of rows which the user is notified at (via Log row)

        private readonly string _sqlPragmas = $@"PRAGMA journal_mode = OFF; PRAGMA page_size = {(ushort.MaxValue + 1).ToString()}; PRAGMA synchronous=OFF";
        private readonly DateTime PostDateLowerBound = new DateTime(2016, 1, 1);
        private readonly DateTime PostDateHigherBound = new DateTime(2019, 3, 31);
        private readonly float[] FitnessDayProbabilityArray = new float[4] { 0.5f, 0.9f, 0.3f, 0.85f };

        private readonly CultureInfo currentCulture = CultureInfo.GetCultureInfo("en-US");                          // Dot as decimal separator
        #endregion


        #region Private Fields
        private StreamWriter _sqlScriptFile;
        private StreamReader _sqlExecFile;
        private long _insertedRows = 0;
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
            InitDatabaseCommand = new ParameterlessCommandAsync(InitDatabaseWrapperAync, () => !Processing);
            ExecuteSqlCommand = new ParameterlessCommandAsync(ExecuteSql, () => !Processing);

            // Decimal separator as dot, not comma
            CultureInfo.DefaultThreadCurrentCulture = currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = currentCulture;
        }



        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;


        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        private async Task ExecuteSql()
        {
            string sqlString = string.Empty;
            int rowsModified = 0;
            Stopwatch partialTime = new Stopwatch();
            Stopwatch totalTime = new Stopwatch();

            Processing = true;
            ProcessedRowsNumber = 0;
            TotalRowsNumber = long.MaxValue;

            partialTime.Start();
            totalTime.Start();



            // Create a new file every time
            if (!File.Exists(SqlScriptFileName))
            {
                SqlFail = $@"Could not open {SqlScriptFileName}";
                return;
            }

            SqlFail = "";

            try
            {
                using (_sqlExecFile = new StreamReader(File.OpenRead(SqlScriptFileName)))
                {
                    // Read the SQL script file
                    sqlString = _sqlExecFile.ReadToEnd();
                }

                SqlLogEntries += Environment.NewLine;

                partialTime.Stop();
                SqlLogEntries += $@"SQL script read in: {partialTime.Elapsed.Hours.ToString()}:{partialTime.Elapsed.Minutes.ToString()}:{partialTime.Elapsed.Seconds.ToString()}{Environment.NewLine}";


                partialTime = new Stopwatch();
                partialTime.Start();

                // SQLite connection parameters
                SQLiteConnectionStringBuilder sqlConnStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = DbName,
                    JournalMode = SQLiteJournalModeEnum.Off,
                    SyncMode = SynchronizationModes.Off,
                    PageSize = ushort.MaxValue + 1,
                    DefaultTimeout = 100,
                };

                //using (SQLiteConnection connection = new SQLiteConnection($"Data Source = {DbName}; {_sqlPragmas}"))
                using (SQLiteConnection connection = new SQLiteConnection(sqlConnStr.ToString()))
                {
                    connection.Open();

                    // Execute it
                    SQLiteCommand cmd = new SQLiteCommand()
                    {
                        Connection = connection,
                        CommandText = sqlString,
                    };

                    // Bulk insert execute
                    rowsModified = await cmd.ExecuteNonQueryAsync();

                    partialTime.Stop();
                    totalTime.Stop();

                    SqlLogEntries += $@"SQL executed read in: {partialTime.Elapsed.Hours.ToString()}:{partialTime.Elapsed.Minutes.ToString()}:{partialTime.Elapsed.Seconds.ToString()}{Environment.NewLine}";
                    SqlLogEntries += $@"Total Time: {partialTime.Elapsed.Hours.ToString()}:{partialTime.Elapsed.Minutes.ToString()}:{partialTime.Elapsed.Seconds.ToString()}{Environment.NewLine}";

                    SqlLogEntries += $@"___________________________________________________________________________________________{Environment.NewLine}";
                    SqlLogEntries += $@"Number of rows inserted: {rowsModified.ToString()}{Environment.NewLine}";
                    SqlLogEntries += $@"Total Milliseconds per row: { ((float)totalTime.Elapsed.TotalMilliseconds / (float)rowsModified).ToString()} [ms]";
                    SqlLogEntries += $@"SQL Milliseconds per row: { ((float)partialTime.Elapsed.TotalMilliseconds / (float)rowsModified).ToString()} [ms]";

                    connection.Close();
                }
            }
            catch (Exception exc)
            {
                SqlFail = $@"Error while executing the script";
                SqlLogEntries += $@"\t\t\t--- ERROR ---{Environment.NewLine}";
                SqlLogEntries += $@"Exception: {exc.Message}";
            }
            Processing = false;
        }


        private async Task InitDatabaseWrapperAync()
        {
            Stopwatch totalTime = new Stopwatch();

            totalTime.Start();

            SqlFail = string.Empty;
            SqlLogEntries = string.Empty;
            Processing = true;
            ProcessedRowsNumber = 0;

            // Create a new file every time
            if (File.Exists(SqlScriptFileName))
                File.Delete(SqlScriptFileName);

            using (_sqlScriptFile = new StreamWriter(File.OpenWrite(SqlScriptFileName)))
            {
                // SQLite connection
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source = {DbName}"))
                {
                    connection.Open();

                    try
                    {
                        await Task.Run(() => InitDatabase(connection));
                    }
                    catch (Exception exc)
                    {
                        SqlFail = $@"Error: {exc.Message}";
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            totalTime.Stop();

            SqlLogEntries += $@"______________________________________________________________________________________________" + Environment.NewLine;
            SqlLogEntries += $@"Inserted Rows: {_insertedRows.ToString()}";
            SqlLogEntries += $@"Elapsed Time: {totalTime.Elapsed.Hours.ToString()}:{totalTime.Elapsed.Minutes.ToString()}:{totalTime.Elapsed.Seconds.ToString()}{Environment.NewLine}";
            SqlLogEntries += $@"Average row time: { ((float)totalTime.Elapsed.TotalMilliseconds / (float)_insertedRows).ToString()} [ms]";

            Processing = false;
        }


        private void InitDatabase(SQLiteConnection connection)
        {

            int numberOfNewRows = 0;
            long maxId = 0;

            Stopwatch partialTime = new Stopwatch();

            string tableName;

            _sqlScriptFile.WriteLine(@"BEGIN TRANSACTION;");


            ////
            ////  USER TABLE
            ////
            //_sqlScriptFile.WriteLine(@"BEGIN TRANSACTION;");

            //tableName = "User";
            //int numberOfNewRows = 100000;

            //partialTime.Start();

            //// Get Table last ID
            //int maxId = GetTableMaxId(connection, tableName);
            //// Populate it
            //bool ok = await PopulateUserTable(connection, maxId, numberOfNewRows);
            //// Log
            //partialTime.Stop();

            //_sqlScriptFile.WriteLine(@"END TRANSACTION;");



            //
            //  Post, Measure, FitnessDay
            //
            if (true)
            {
                tableName = "Post";
                numberOfNewRows = 0;

                if (numberOfNewRows > 0)
                {
                    partialTime.Start();

                    maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

                    maxId = PopulatePostTable(connection, maxId, numberOfNewRows, "MeasuresEntry");
                    partialTime.Stop();
                    SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                }
                //Populate child2

                //numberOfNewRows = 2000000;
                numberOfNewRows = 2000000;
                if (numberOfNewRows > 0)
                {
                    partialTime = new Stopwatch();
                    partialTime.Start();
                    maxId = PopulatePostTable(connection, maxId, numberOfNewRows, "FitnessDayEntry");

                    partialTime.Stop();

                    SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                }
            }

            //
            //  Post, DietPlan, Phase
            //
            if (true)
            {
                tableName = "Post";
                //numberOfNewRows = 1000000;
                numberOfNewRows = 1;

                if (numberOfNewRows > 0)
                {
                    partialTime.Start();

                    maxId = DatabaseUtility.GetTableMaxId(connection, tableName);

                    maxId = PopulatePostTable(connection, maxId, numberOfNewRows, "UserPhase");
                    partialTime.Stop();
                    SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";

                    //Populate child2

                    //numberOfNewRows = 1000000;
                    numberOfNewRows = 1;

                    partialTime = new Stopwatch();
                    partialTime.Start();
                    maxId = PopulatePostTable(connection, maxId, numberOfNewRows, "DietPlan");

                    partialTime.Stop();

                    SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                }
            }

            if (false)
            {
                numberOfNewRows = 2000000;

                if (numberOfNewRows > 0)
                {
                    partialTime = new Stopwatch();
                    partialTime.Start();
                    maxId = (int)GetFitnessDayFirstId(connection);
                    maxId = PopulateFitnessDayChilds(connection, maxId, numberOfNewRows);
                    // Log
                    partialTime.Stop();

                    SqlLogEntries += $@"{partialTime.Elapsed.Hours}:{partialTime.Elapsed.Minutes}:{partialTime.Elapsed.Seconds}{Environment.NewLine}";
                }
            }


            _sqlScriptFile.WriteLine(@"COMMIT;");
        }




        private bool PopulateUserTable(SQLiteConnection connection, long firstId, long rowNum)
        {
            string colValue = string.Empty;
            string usernameTemplate;

            StringBuilder sqlStr = new StringBuilder();

            List<string> columns = new List<string>();
            Dictionary<string, TypeAffinity> colTypes = new Dictionary<string, TypeAffinity>();

            // Restart the progressbar
            TotalRowsNumber = rowNum;
            ProcessedRowsNumber = 0;

            try
            {
                List<int> accountStatusTypes = GetAccountStatusTypeIds(connection);

                // Get columns definition
                SQLiteCommand cmd = new SQLiteCommand()
                {
                    Connection = connection,
                    CommandText = $"SELECT * FROM User LIMIT(1)",
                };

                SQLiteDataReader sqlr = cmd.ExecuteReader();

                while (sqlr.Read())
                {
                    // Fetch columns name (skip ID column)
                    for (int icol = 1; icol < sqlr.FieldCount; icol++)
                    {
                        columns.Add(sqlr.GetName(icol));
                        colTypes.Add(columns[columns.Count - 1], sqlr.GetFieldAffinity(icol));
                    }

                    SqlLogEntries += columns + Environment.NewLine;
                }

                SqlLogEntries += $@"Processing USER table..." + Environment.NewLine;


                //cmd = new SQLiteCommand()
                //{
                //    Connection = connection,
                //    CommandText = $@"INSERT INTO User ({string.Join(",", columns)}"
                //};
                //// Remove last comma
                //cmd.CommandText += ") VALUES";

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
                        return false;
                    }


                    //cmd.CommandText += $@"(";
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

                                colValue = new Random().Next(accountStatusTypes.Min(), accountStatusTypes.Max()).ToString();
                                break;

                            default:

                                RandomFieldGenerator.GenerateRandomField(colTypes[colName]);
                                break;
                        }
                        //cmd.CommandText += $@"{colValue},";
                        sqlStr.Append($@"{colValue},");
                    }
                    SqlLogEntries += $@"{i.ToString()}/{rowNum.ToString()}" + Environment.NewLine;


                    // Remove last comma
                    //cmd.CommandText += cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";
                    sqlStr.Remove(sqlStr.Length - 1, 1).Append("),");

                    //// Remove last comma
                    //cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";
                    //await cmd.ExecuteNonQueryAsync();

                    ProcessedRowsNumber++;
                }
                //cmd.CommandText += $@")";
                sqlStr.Remove(sqlStr.Length - 1, 1).Append(";");
            }
            catch (Exception exc)
            {
                return false;
            }

            // Write the file
            SqlLogEntries = sqlStr.ToString();
            _sqlScriptFile.WriteLine(sqlStr);

            // Update the counter
            _insertedRows += rowNum;
            return true;
        }




        /// <summary>
        /// Populate the Post Table together with one of its child tables
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="firstId"></param>
        /// <param name="rowNum"></param>
        /// <param name="childTableName"></param>
        /// <returns>Returns the new maxId</returns>
        private long PopulatePostTable(SQLiteConnection connection, long firstId, int rowNum, string childTableName)
        {
            string colValue = string.Empty;
            string postTemplate;
            string parentTableName = "Post";
            int startDate = 0;

            StringBuilder sqlStrParent = new StringBuilder();
            StringBuilder sqlStrChild = new StringBuilder();

            List<string> parentColumns = new List<string>();
            List<string> childColumns = new List<string>();
            Dictionary<string, TypeAffinity> parentColTypes = new Dictionary<string, TypeAffinity>();
            Dictionary<string, TypeAffinity> childColTypes = new Dictionary<string, TypeAffinity>();

            // Restart the progressbar
            TotalRowsNumber = rowNum;
            ProcessedRowsNumber = 0;


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


                SqlLogEntries += $@"Processing {parentTableName.ToUpper()} table [{rowNum.ToString()} rows]" + Environment.NewLine;
                SqlLogEntries += $@"Processing {childTableName.ToUpper()} table [{rowNum.ToString()} rows]" + Environment.NewLine;


                sqlStrParent.Append($@"INSERT INTO {parentTableName} ({string.Join(",", parentColumns)}) VALUES");
                sqlStrChild.Append($@"INSERT INTO {childTableName} ({string.Join(",", childColumns)}) VALUES");

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

                    sqlStrParent.Append($@"(");
                    sqlStrChild.Append($@"(");

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

                                colValue = $@"{RandomFieldGenerator.RandomUnixTimestamp(PostDateLowerBound, PostDateHigherBound).ToString()}";
                                break;

                            case "LastUpdate":

                                colValue = $@"{RandomFieldGenerator.RandomDateTimeNullAllowed(PostDateLowerBound, PostDateHigherBound, 0.6f)}";
                                break;

                            case "UserId":

                                colValue = RandomFieldGenerator.RandomInt(minUserId, maxUserId).ToString();
                                break;

                            default:

                                colValue = RandomFieldGenerator.GenerateRandomField(parentColTypes[colName]);
                                break;
                        }

                        sqlStrParent.Append($@"{colValue},");
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

                                colValue = $@"{RandomFieldGenerator.RandomUnixDate(PostDateLowerBound, PostDateHigherBound).ToString()}";
                                break;


                            case "LastUpdate":

                                colValue = $@"{RandomFieldGenerator.RandomDateTimeNullAllowed(PostDateLowerBound, PostDateHigherBound, 0.6f)}";
                                break;

                            case "CreatedOn":

                                colValue = $@"{RandomFieldGenerator.RandomUnixTimestamp(PostDateLowerBound, PostDateHigherBound).ToString()}";
                                break;

                            case "StartDate":

                                startDate = RandomFieldGenerator.RandomUnixDate(PostDateLowerBound, PostDateHigherBound);
                                colValue = $@"{startDate.ToString()}";
                                break;

                            case "EndDate":

                                if (startDate > 0)
                                {
                                    colValue = $@"{RandomFieldGenerator.RandomUnixDate(startDate, DatabaseUtility.UnixTimestampOneMonthDelta, DatabaseUtility.UnixTimestampSixMonthsDelta).ToString()}";
                                    startDate = 0;
                                }
                                else
                                    colValue = $@"{RandomFieldGenerator.RandomUnixDate(PostDateLowerBound, PostDateHigherBound).ToString()}";
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

                                colValue = RandomFieldGenerator.GenerateRandomField(childColTypes[colName1]);
                                break;
                        }

                        sqlStrChild.Append($@"{colValue},");
                    }

                    sqlStrParent.Remove(sqlStrParent.Length - 1, 1).Append("),");
                    sqlStrChild.Remove(sqlStrChild.Length - 1, 1).Append("),");

                    ProcessedRowsNumber++;
                }

                sqlStrParent.Remove(sqlStrParent.Length - 1, 1).Append(";");
                sqlStrChild.Remove(sqlStrChild.Length - 1, 1).Append(";");
            }
            catch (Exception exc)
            {
                return -1;
            }

            // Write the SQL script
            _sqlScriptFile.WriteLine(sqlStrParent);
            _sqlScriptFile.WriteLine();
            _sqlScriptFile.WriteLine(sqlStrChild);
            _sqlScriptFile.WriteLine();

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
        private long PopulateFitnessDayChilds(SQLiteConnection connection, long firstId, int rowNum)
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

                SqlLogEntries += $@"Processing {tableName}..." + Environment.NewLine;


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

                                    colValue = RandomFieldGenerator.GenerateRandomField(colTypes1[colName]);
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

                                    colValue = RandomFieldGenerator.GenerateRandomField(colTypes2[colName]);
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

                                    colValue = RandomFieldGenerator.GenerateRandomField(colTypes3[colName]);
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

                                    colValue = RandomFieldGenerator.GenerateRandomField(colTypes4[colName]);
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
            _sqlScriptFile.WriteLine(sqlStr1);
            _sqlScriptFile.WriteLine();
            _sqlScriptFile.WriteLine(sqlStr2);
            _sqlScriptFile.WriteLine();
            _sqlScriptFile.WriteLine(sqlStr3);
            _sqlScriptFile.WriteLine();
            _sqlScriptFile.WriteLine(sqlStr4);

            // Update the counter
            _insertedRows += (rowNum * 2);

            return firstId + rowNum;
        }



        private List<int> GetAccountStatusTypeIds(SQLiteConnection connection)
        {
            List<int> StatusTypeIds = new List<int>();

            SQLiteCommand cmd = new SQLiteCommand(connection)
            {
                CommandText = "SELECT Id FROM AccountStatusType WHERE Id <> 8"
            };
            SQLiteDataReader sqlr = cmd.ExecuteReader();

            while (sqlr.Read())
            {
                StatusTypeIds.Add(sqlr.GetInt32(0));
            }

            return StatusTypeIds;
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
