using SQLiteUtils.Commands;
using SQLiteUtils.Model;
using SQLiteUtils.Util;
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

    public class SmartGeneratorManagerViewModel : DbGeneratorManagerBaseViewModel
    {


        #region Enums

        #endregion


        #region Consts

        private const string DefaultTitle = "Query Manager";
        #endregion



        #region Properties

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        #endregion


        #region INotifyPropertyChanged Implementation

        private bool _isFullOperationRequired = false;

        /// <summary>
        /// User asked for full operation: write script + exec SQL
        /// </summary>
        public bool IsFullOperationRequired
        {
            get => _isFullOperationRequired;
            set => SetProperty(ref _isFullOperationRequired, value);
        }
        #endregion



        #region Ctors

        /// <summary>
        /// ViewModel for the Query Manager
        /// </summary>
        /// <param name="isProcessingChangedAction">Function to be called when the processing status changes (IE: operation starts/stops)</param>
        /// <param name="isProgressChanged">Function to be called when the execution progress changes</param>
        /// <param name="onErrorAction">Function to be called when an error is raised.</param>
        public SmartGeneratorManagerViewModel(Action<bool> isProcessingChangedAction
            , Action<long, long> isProgressChanged, Action<string> onErrorAction)
            : base(DefaultTitle, isProcessingChangedAction, isProgressChanged, onErrorAction)

        {
            CreateSqlScriptCommandAsync = new ParameterlessCommandAsync(CreateSqlScriptAsync, () => !IsProcessing);
            ExecuteSqlCommandAsync = new ParameterlessCommandAsync(ExecuteSqlWrapperAsync, () => !IsProcessing);

            FromDate = GymAppSQLiteConfig.DbDateLowerBound;
            ToDate = GymAppSQLiteConfig.DbDateUpperBound;
            
            InitProcessTablesData();
        }

        #endregion


        #region Public Methods

        #endregion



        #region Private Methods

        private async Task CreateSqlScriptAsync()
        {
            Stopwatch totalTime = new Stopwatch();
            totalTime.Start();

            IsProcessing = true;
            NewRows = 0;
            TotalRows = 1;

            // Async so the UI is not blocked
            await Task.Run(() => ProcessSqlAsync());

            totalTime.Stop();
            ExecutionReport(NewRows, totalTime.Elapsed);

            IsProcessing = false;
        }


        /// <summary>
        /// Main process for the script generation
        /// </summary>
        /// <returns></returns>
        private async Task ProcessSqlAsync()
        {
            Stopwatch partialTime;
            string tablesSelectionName;
            long rowNum;

            tablesSelectionName = "Messages";
            rowNum = GetScaledRowNumber(tablesSelectionName);

            if (rowNum > 0)
            {
                // Log
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                // Init Wrappers
                DbWriter.DbPath = DbName;
                DbWriter.Open();
                BuildDbWrapper();
                GymWrapper.Init();

                try
                {
                    await Task.Run(() => GymWrapper?.PopulateNotesTables(rowNum));
                }
                catch (Exception exc) when (!HasError)  // Error already raised by BuildDbWrapper
                {
                    RaiseError($"Error while processing {tablesSelectionName} - {exc.Message}");
                    IsProcessing = false;
                    return;
                }
                GymWrapper?.DbWriter?.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }


            tablesSelectionName = "Relations";
            rowNum = GetScaledRowNumber(tablesSelectionName);

            if (rowNum > 0)
            {
                // Log
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                // Init Wrappers
                DbWriter.DbPath = DbName;
                DbWriter.Open();
                BuildDbWrapper();
                GymWrapper.Init();

                try
                {
                    await Task.Run(() => GymWrapper?.InsertRelations(rowNum));
                }
                catch (Exception exc) when (!HasError)  // Error already raised by BuildDbWrapper
                {
                    RaiseError($"Error while processing {tablesSelectionName} - {exc.Message}");
                    IsProcessing = false;
                    return;
                }

                GymWrapper?.DbWriter?.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            tablesSelectionName = "FullUserData";
            rowNum = GetScaledRowNumber(tablesSelectionName);

            if (rowNum > 0)
            {
                // Log
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                // Init Wrappers
                DbWriter.DbPath = DbName;
                DbWriter.Open();
                BuildDbWrapper();
                GymWrapper.Init();

                try
                {
                    await Task.Run(() => GymWrapper?.InsertUsers(FromDate, ToDate, (ushort)rowNum));
                }
                catch (Exception exc) when (!HasError)  // Error already raised by BuildDbWrapper
                {
                    RaiseError($"Error while processing {tablesSelectionName} - {exc.Message}");
                    IsProcessing = false;
                    return;
                }

                GymWrapper?.DbWriter?.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            tablesSelectionName = "UserPosts";
            rowNum = GetScaledRowNumber(tablesSelectionName);

            if (rowNum > 0)
            {
                // Log
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                // Init Wrappers
                DbWriter.DbPath = DbName;
                DbWriter.Open();
                BuildDbWrapper();
                GymWrapper.Init();

                try
                {
                    await Task.Run(() => GymWrapper?.PopulateNotesTables(rowNum));
                }
                catch (Exception exc) when (!HasError)  // Error already raised by BuildDbWrapper
                {
                    RaiseError($"Error while processing {tablesSelectionName} - {exc.Message}");
                    IsProcessing = false;
                    return;
                }

                GymWrapper?.DbWriter?.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            tablesSelectionName = "UserTraining";
            rowNum = GetScaledRowNumber(tablesSelectionName);

            if (rowNum > 0)
            {
                // Log
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                // Init Wrappers
                DbWriter.DbPath = DbName;
                DbWriter.Open();
                BuildDbWrapper();
                GymWrapper.Init();

                try
                {
                    await Task.Run(() => GymWrapper?.PopulateNotesTables(rowNum));
                }
                catch (Exception exc) when (!HasError)  // Error already raised by BuildDbWrapper
                {
                    RaiseError($"Error while processing {tablesSelectionName} - {exc.Message}");
                    IsProcessing = false;
                    return;
                }

                GymWrapper?.DbWriter?.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }



            tablesSelectionName = "Comments";
            rowNum = GetScaledRowNumber(tablesSelectionName);

            if (rowNum > 0)
            {
                // Log
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                // Init Wrappers
                DbWriter.DbPath = DbName;
                DbWriter.Open();
                BuildDbWrapper();
                GymWrapper.Init();

                try
                {
                    await Task.Run(() => GymWrapper?.InsertComments(rowNum));
                }
                catch (Exception exc) when (!HasError)  // Error already raised by BuildDbWrapper
                {
                    RaiseError($"Error while processing {tablesSelectionName} - {exc.Message}");
                    IsProcessing = false;
                    return;
                }

                GymWrapper?.DbWriter?.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }


            tablesSelectionName = "PersonalRecord";
            rowNum = GetScaledRowNumber(tablesSelectionName);

            if (rowNum > 0)
            {
                // Log
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                // Init Wrappers
                DbWriter.DbPath = DbName;
                DbWriter.Open();
                BuildDbWrapper();
                //GymWrapper.Init();

                try
                {
                    await Task.Run(() => GymWrapper?.InsertPersonalRecords(rowNum));
                }
                catch (Exception exc) when (!HasError)  // Error already raised by BuildDbWrapper
                {
                    RaiseError($"Error while processing {tablesSelectionName} - {exc.Message}");
                    IsProcessing = false;
                    return;
                }

                GymWrapper?.DbWriter?.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }


            if (IsFullOperationRequired)
                await ExecuteSqlWrapperAsync();

            if(IsShutdownRequired)
                Process.Start("shutdown", "/s /t 0");

            IsProcessing = false;
        }


        /// <summary>
        /// Process the 
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="processTitle"></param>
        /// <param name="processFun"></param>
        private async Task ProcessTables(long rowNum, string processTitle, Action<long>processFun)
        {
            Stopwatch partialTime;

            // Log
            StartTableLog(processTitle);
            partialTime = new Stopwatch();
            partialTime.Start();

            try
            {
                await Task.Run(() => processFun(rowNum));
            }
            catch (Exception exc) when (!HasError)  // Error already raised by BuildDbWrapper
            {
                RaiseError($"Error while processing {processTitle} - {exc.Message}");
                IsProcessing = false;
                return;
            }

            GymWrapper?.DbWriter.Close();

            // Log
            partialTime.Stop();
            EndTableLog(processTitle, partialTime.Elapsed);
        }


        private void InitProcessTablesData()
        {
            ProcessTablesData = new ObservableCollection<TableProcessData>();
            byte count = 0;

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "Messages",
                TotalRows = 0,
                Enabled = true,
                OrderNumber = count++,
                ScaleFactor = 1000000,
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "Relations",
                TotalRows = 0,
                Enabled = true,
                OrderNumber = count++,
                ScaleFactor = 1000000,
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "FullUserData",
                TotalRows = 0,
                Enabled = true,
                OrderNumber = count++,
                ScaleFactor = 1,
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "UserPosts",
                TotalRows = 0,
                Enabled = false,
                OrderNumber = count++,
                ScaleFactor = 1,
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "UserTraining",
                TotalRows = 0,
                Enabled = false,
                OrderNumber = count++,
                ScaleFactor = 1,
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "Comments",
                TotalRows = 0,
                Enabled = true,
                OrderNumber = count++,
                ScaleFactor = 1000000,
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "PersonalRecord",
                TotalRows = 0,
                Enabled = true,
                OrderNumber = count++,
                ScaleFactor = 1000000,
            });
        }



        /// <summary>
        /// Async function to execute the SQL script files generated.
        /// </summary>
        /// <returns></returns>
        protected override async Task ExecuteSqlWrapperAsync()
        {
            SQLiteTransaction sqlTrans = null;
            Stopwatch partialTime = new Stopwatch();
            Stopwatch totalTime = new Stopwatch();

            IsExecutingSql = true;
            NewRows = 0;
            TotalRows = 1;

            SqlLog += Environment.NewLine;
            totalTime.Start();

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
                        DbWriter.DbPath = DbName;
                        DbWriter.Open();

                        SQLiteConnection connection = DbWriter.SqlConnection;

                        // Progress as processed files Vs total ones
                        TotalRows = GymAppSQLiteConfig.GetScriptFilesPath().Count();

                        // Process the script files
                        foreach (string filename in GymAppSQLiteConfig.GetScriptFilesPath().ToList())
                        {
                            sqlTrans = connection.BeginTransaction();

                            partialTime = new Stopwatch();
                            partialTime.Start();

                            await DatabaseUtility.ExecuteSqlScript(filename, connection);

                            partialTime.Stop();
                            EndTableLog(Path.GetFileName(filename), partialTime.Elapsed);

                            sqlTrans.Commit();
                            File.Delete(filename);
                            NewRows++;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                sqlTrans?.Rollback();
                RaiseError(exc.Message);
                return;
            }
            finally
            {
                sqlTrans?.Dispose();
                DbWriter.Close();

                // Display execution report
                totalTime.Stop();
                ExecutionReport(NewRows, totalTime.Elapsed);
                IsExecutingSql = false;
            }

            // If no errors then delete the files - The stats file should be the only one left at this point
            foreach (string filename in Directory.EnumerateFiles(GymAppSQLiteConfig.SqlScriptFolder))
                File.Delete(filename);

            if (IsShutdownRequired)
                Process.Start("shutdown", "/s /t 0");

            return;
        }
        #endregion
    }
}
