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



    public class RawGeneratorManagerViewModel : DbGeneratorManagerBaseViewModel
    {

        #region Consts

        private const string DefaultTitle = "Raw Generator";
        #endregion


        #region Private Fields
        //private long _insertedRows = 0;
        #endregion



        #region INotifyPropertyChanged Implementation

        private long _currentTableProcessedRows = 0;
        public long CurrentTableProcessedRows
        {
            get => _currentTableProcessedRows;
            set => SetProperty(ref _currentTableProcessedRows, value);
        }

        public ParameterlessCommand ResetProcessTableDataCommand { get; set; }

        #endregion



        #region Properties

        #endregion



        #region Ctors

        /// <summary>
        /// ViewModel for the Raw Database Generator.
        /// The module provides basic bulk insert capabilities as it just insert random entries with no link between most of them.
        /// </summary>
        /// <param name="isProcessingChangedAction">Function to be called when the processing status changes (IE: operation starts/stops)</param>
        /// <param name="onErrorAction">Function to be called when an error is raised.</param>
        public RawGeneratorManagerViewModel(Action<bool> isProcessingChangedAction, Action<string> onErrorAction)
            : base (DefaultTitle, isProcessingChangedAction, onErrorAction)

        {
            CreateSqlScriptCommandAsync = new ParameterlessCommandAsync(GenerateSqlScriptWrapperAync, () => !IsProcessing);
            ExecuteSqlCommandAsync = new ParameterlessCommandAsync(ExecuteSqlWrapperAsync, () => !IsProcessing);
            ResetProcessTableDataCommand = new ParameterlessCommand(InitProcessTableData, () => !IsProcessing);

            InitProcessTableData();
        }
        #endregion



        #region Private Methods

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



        private async Task GenerateSqlScriptWrapperAync()
        {

            IsProcessing = true;
            //_insertedRows = 0;
            NewRows = 0;
            TotalRows = 1;

            Stopwatch totalTime = new Stopwatch();

            totalTime.Start();

            SqlLog = string.Empty;
            IsProcessing = true;
            CurrentTableProcessedRows = 0;

            // Delete old files
            foreach (string filePath in GymAppSQLiteConfig.GetScriptFilesPath())
                File.Delete(filePath);

            SqlLog += Environment.NewLine;

            //SQLite connection
            _connection = DatabaseUtility.OpenFastestSQLConnection(_connection, DbName);

            try
            {
                await Task.Run(() => GenerateSqlScript(_connection));
            }
            catch (Exception exc)
            {
                RaiseError($@"Error: {exc.Message}");
            }

            totalTime.Stop();

            // Display execution report
            ExecutionReport(NewRows, totalTime.Elapsed);

            IsProcessing = false;
        }



        private void GenerateSqlScript(SQLiteConnection connection)
        {
            List<DatabaseObjectWrapper> tables;

            try
            {
                // User
                tables = new List<DatabaseObjectWrapper>()
                {
                     new UserWrapper(_connection),
                };

                ProcessTables("User", tables);


                // Post
                tables = new List<DatabaseObjectWrapper>()
                {
                    new PostWrapper(_connection),
                };

                ProcessTables("Post", tables);


                // Measure
                tables = new List<DatabaseObjectWrapper>()
                {
                    new PostWrapper(_connection),
                    new MeasureWrapper(_connection),
                    new CircumferenceWrapper(_connection),
                    new PlicometryWrapper(_connection),
                    //new BiaWrapper(_connection),
                };

                ProcessTables("Measure", tables);


                // FitnessDay
                tables = new List<DatabaseObjectWrapper>()
                {
                    new PostWrapper(_connection),
                    new FitnessDayWrapper(_connection),
                    new ActivityDayWrapper(_connection),
                    new DietDayWrapper(_connection),
                    new WeightWrapper(_connection),
                    new WellnessDayWrapper(_connection),
                };

                ProcessTables("FitnessDay", tables);


                // Phase
                tables = new List<DatabaseObjectWrapper>()
                {
                    new PostWrapper(_connection),
                    new UserPhaseWrapper(_connection),
                };

                ProcessTables("Phase", tables);


                // Diet Plan
                tables = new List<DatabaseObjectWrapper>()
                {
                    new PostWrapper(_connection),
                    new DietPlanWrapper(_connection),
                    new DietPlanUnitWrapper(_connection),
                    new DietPlanDayWrapper(_connection),
                };

                ProcessTables("DietPlan", tables);
            }
            catch(Exception exc)
            {
                RaiseError(exc.Message);
            }
        }





        /// <summary>
        /// Process the table objects in the list provided
        /// </summary>
        /// <param name="processTableName">The name of the tables section to be processed</param>
        /// <param name="tables">The table objects to be processed</param>
        private void ProcessTables(string processTableName, List<DatabaseObjectWrapper> tables)
        {
            Stopwatch partialTime = new Stopwatch();

            uint rowNum = ProcessTablesData.Where(x => x.TableName == processTableName).First().TotalRows;

            if (rowNum > 0 && ProcessTablesData.Where(x => x.TableName == processTableName).First().Enabled)
            {
                // Restart the progressbar
                CurrentTableProcessedRows = 0;

                StartTableLog(processTableName);
                partialTime = new Stopwatch();
                partialTime.Start();

                // Process
                WriteScriptFiles(rowNum, processTableName, tables);

                // Log
                partialTime.Stop();
                EndTableLog(processTableName, partialTime.Elapsed);
            }
        }


        /// <summary>
        /// Writes as many script files as needed (the bulk inserts are split into different files to avoid high memory requirements when executing them)
        /// </summary>
        /// <param name="rowNum">Number of rows to be inserted</param>
        /// <param name="processTableName">The name of the tables section to be processed</param>
        /// <param name="tables">Tables to be processed</param>
        private void WriteScriptFiles(uint rowNum, string processTableName, List<DatabaseObjectWrapper> tables)
        {
            uint currentNewRows = 0;

            // Number of files to be generated
            ushort totalParts = (ushort)Math.Ceiling((float)rowNum / GymAppSQLiteConfig.RowsPerScriptFile);

            BuildDbWrapper();
            GymWrapper.CurrentRow = CurrentTableProcessedRows;

            // Split files so they don't exceed the maximum number of rows per file
            for (ushort iPart = 0; iPart < totalParts; iPart++)
            {
                // Compute number of rows wrt the number of files
                currentNewRows = iPart == totalParts - 1 ? rowNum - (iPart * GymAppSQLiteConfig.RowsPerScriptFile) : GymAppSQLiteConfig.RowsPerScriptFile;
                // Write
                GymWrapper.BasicSqlScriptGenerator(currentNewRows, GetScriptFileFullpath(processTableName, iPart, totalParts), GymAppSQLiteConfig.SqlScriptFilePath, tables);
                // Update the tables maxId
                tables.ForEach(x => x.MaxId += currentNewRows);
            }

            CurrentTableProcessedRows += GymWrapper.CurrentRow;
        }


        #endregion
    }
}
