﻿using SQLiteUtils.Commands;
using SQLiteUtils.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLiteUtils.ViewModel
{




    public class TableProcessData
    {
        public string TableName { get; set; }
        public bool Enabled { get; set; }
        public uint TotalRows { get; set; }
        public ushort OrderNumber { get; set; }
    }




    public class DbGeneratorManagerBaseViewModel : DbManagerBaseViewModel
    {



        #region Private fields

        protected SQLiteConnection _connection = null;            // Global to avoid DB locking issues
        #endregion


        #region Commands

        /// <summary>
        /// Command for the execution of the SQl script
        /// </summary>
        public ParameterlessCommandAsync CreateSqlScriptCommandAsync { get; set; }

        /// <summary>
        /// Command for the execution of the SQl script
        /// </summary>
        public ParameterlessCommandAsync ExecuteSqlCommandAsync { get; set;}
        #endregion


        #region INotifyPropertyChanged Implementation

        private bool _isExecutingSql = false;

        /// <summary>
        /// Executing the bulk inserts from the script file.
        /// </summary>
        public bool IsExecutingSql
        {
            get => _isExecutingSql;
            set
            {
                SetProperty(ref _isExecutingSql, value);

                // Link to Processing
                IsProcessing = value;
            }
        }

        private string _sqlLog = string.Empty;

        /// <summary>
        /// Log entries
        /// </summary>
        public string SqlLog
        {
            get => _sqlLog;
            set => SetProperty(ref _sqlLog, value);
        }


        private DbWrapper _gymWrapper;

        /// <summary>
        /// Db wrapper to be used for the databse operations
        /// </summary>
        public DbWrapper GymWrapper
        {
            get => _gymWrapper;
            set => SetProperty(ref _gymWrapper, value);
        }

        private ObservableCollection<TableProcessData> _processTablesData;

        /// <summary>
        /// List of process tables data.
        /// </summary>
        public ObservableCollection<TableProcessData> ProcessTablesData
        {
            get => _processTablesData;
            set => SetProperty(ref _processTablesData, value);
        }

        #endregion




        #region Ctors


        /// <summary>
        /// ViewModel for the Raw Database Generator.
        /// The module provides basic bulk insert capabilities as it just insert random entries with no link between most of them.
        /// </summary>
        /// <param name="isProcessingChangedAction">Function to be called when the processing status changes (IE: operation starts/stops)</param>
        /// <param name="onErrorAction">Function to be called when an error is raised.</param>
        public DbGeneratorManagerBaseViewModel(string viewModelTitle, Action<bool> isProcessingChangedAction, Action<string> onErrorAction)
            : base(viewModelTitle, isProcessingChangedAction, onErrorAction)

        {

        }
        #endregion



        #region Private Methods



        /// <summary>
        /// Async function to execute the SQL script files generated.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task ExecuteSqlWrapperAsync()
        {
            long newRows = 0;
            Stopwatch partialTime = new Stopwatch();
            Stopwatch totalTime = new Stopwatch();

            IsExecutingSql = true;

            SqlLog += Environment.NewLine;

            // Process the Script files
            try
            {
                if (!Directory.Exists(GymAppSQLiteConfig.SqlScriptFolder))

                    RaiseError($@"Path: {GymAppSQLiteConfig.SqlScriptFolder} does not exist{Environment.NewLine}");
                else
                {
                    if (Directory.EnumerateFiles(GymAppSQLiteConfig.SqlScriptFolder).Count() == 0)

                        RaiseError($@"Directory: {GymAppSQLiteConfig.SqlScriptFolder} is empty{Environment.NewLine}");
                    else
                    {

                        _connection = DatabaseUtility.OpenFastestSQLConnection(_connection, DbName);
                        SQLiteTransaction sqlTrans = _connection.BeginTransaction();


                        foreach (string filename in GymAppSQLiteConfig.GetScriptFilesPath())
                        {
                            partialTime.Start();
                            totalTime.Start();

                            newRows += await DatabaseUtility.ExecuteSqlScript(filename, _connection);

                            partialTime.Stop();
                            EndTableLog(Path.GetFileName(filename), totalTime.Elapsed);
                        }

                        sqlTrans.Commit();
                    }
                }
            }
            catch (Exception exc)
            {
                RaiseError(exc.Message);
            }

            totalTime.Stop();

            // Display execution report
            ExecutionReport(newRows, totalTime.Elapsed);
            IsExecutingSql = false;

            return;
        }


        /// <summary>
        /// Get the execution report log
        /// </summary>
        /// <param name="processedRows">Number of rows processed</param>
        /// <param name="executionTime">Execution time</param>
        /// <returns></returns>
        protected virtual void ExecutionReport(long processedRows, TimeSpan executionTime)
        {
            SqlLog += $@"___________________________________________________________________________________________{Environment.NewLine}"
                + $@"Number of rows inserted: {GetFormattedNumber(processedRows)}{Environment.NewLine}"
                + $@"Total Time: {executionTime.Hours.ToString()}:{executionTime.Minutes.ToString()}:{executionTime.Seconds.ToString()}{Environment.NewLine}"
                + $@"Total Milliseconds per row: { ((float)executionTime.TotalMilliseconds / (float)processedRows).ToString()} [ms]{Environment.NewLine}";
        }


        /// <summary>
        /// Make the input number suitable to be displayed properly. EG: EG: 123700000 --> 123.7M
        /// </summary>
        /// <param name="toBeDisplayed">The number to be displayed</param>
        /// <param name="automaticScaleToInputMagnitude">Tells whether the input should be scaled according to its magnitude. Otherwise use the default scale factor no matther what.</param>
        /// <seealso cref="GymAppSQLiteConfig.DefaultDisplayScaleFactor"/>
        /// <returns>The string to be displayed</returns>
        protected virtual string GetFormattedNumber(double toBeDisplayed, bool automaticScaleToInputMagnitude = true)
        {

            float displayScaleFactor = GymAppSQLiteConfig.DefaultDisplayScaleFactor;
            char displayScaleFactorName = GymAppSQLiteConfig.DefaultDisplayScaleFactorName;

            if (automaticScaleToInputMagnitude)
            {
                if (toBeDisplayed < 1000)
                {
                    displayScaleFactor = 1.0f;
                    displayScaleFactorName = ' ';
                }
                else if (toBeDisplayed < 1000000)
                {
                    displayScaleFactor = 1000.0f;
                    displayScaleFactorName = 'K';
                }
            }

            return (toBeDisplayed / displayScaleFactor).ToString() + displayScaleFactorName;
        }



        protected virtual string GetScriptFileFullpath(string filenameSuffix, ushort partNumber, ushort totalPartsNumber)
        {
            return Regex.Replace(Regex.Replace(GymAppSQLiteConfig.SqlScriptFilePath, "##suffix##", filenameSuffix)
                , @"##part##", $"_{partNumber.ToString("d2")}_of_{totalPartsNumber.ToString("d2")}");
        }


        protected virtual void StartTableLog(string processTableName)
        {
            SqlLog += $@"-- {processTableName} Started {Environment.NewLine}";
        }


        protected virtual void EndTableLog(string processTitle, TimeSpan elapsedTime)
        {
            SqlLog += $@"-- {processTitle} Ended in: ";
            SqlLog += $@"{elapsedTime.Hours}:{elapsedTime.Minutes}:{elapsedTime.Seconds}{Environment.NewLine}";
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

        #endregion


    }
}
