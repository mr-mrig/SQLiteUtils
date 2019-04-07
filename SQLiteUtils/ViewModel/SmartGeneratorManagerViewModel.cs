using SQLiteUtils.Commands;
using SQLiteUtils.Model;
using SQLiteUtils.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public long NewRows { get; set; } = 0;

        #endregion


        #region INotifyPropertyChanged Implementation

        //private long _currentTableProcessedRows = 0;
        //public long CurrentTableProcessedRows
        //{
        //    get => _currentTableProcessedRows;
        //    set => SetProperty(ref _currentTableProcessedRows, value);
        //}

        #endregion



        #region Ctors

        /// <summary>
        /// ViewModel for the Query Manager
        /// </summary>
        /// <param name="isProcessingChangedAction">Function to be called when the processing status changes (IE: operation starts/stops)</param>
        /// <param name="onErrorAction">Function to be called when an error is raised.</param>
        public SmartGeneratorManagerViewModel(Action<bool> isProcessingChangedAction, Action<string> onErrorAction)
            : base(DefaultTitle, isProcessingChangedAction, onErrorAction)

        {
            CreateSqlScriptCommandAsync = new ParameterlessCommandAsync(CreateSqlScriptAsync, () => !IsProcessing);

            InitProcessTablesData();
        }

        #endregion


        #region Public Methods

        #endregion



        #region Private Methods

        private async Task CreateSqlScriptAsync()
        {
            string tablesSelectionName;
            Stopwatch partialTime;
            long rowNum;

            Stopwatch totalTime = new Stopwatch();
            totalTime.Start();

            IsProcessing = true;

            tablesSelectionName = "Messages";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

            if (rowNum > 0)
            {
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                await Task.Run(() => WriteScriptFiles(rowNum, tablesSelectionName, CreateMessageTablesScript));

                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            tablesSelectionName = "Relations";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

            if (rowNum > 0)
            {
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                await Task.Run(() => WriteScriptFiles(rowNum, tablesSelectionName, CreateMessageTablesScript));

                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            tablesSelectionName = "FullUserData";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

            if (rowNum > 0)
            {
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                await Task.Run(() => WriteScriptFiles(rowNum, tablesSelectionName, CreateMessageTablesScript));

                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            tablesSelectionName = "UserPosts";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

            if (rowNum > 0)
            {
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                await Task.Run(() => WriteScriptFiles(rowNum, tablesSelectionName, CreateMessageTablesScript));

                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            tablesSelectionName = "UserTraining";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

            if (rowNum > 0)
            {
                StartTableLog(tablesSelectionName);
                partialTime = new Stopwatch();
                partialTime.Start();

                await Task.Run(() => WriteScriptFiles(rowNum, tablesSelectionName, CreateMessageTablesScript));

                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            totalTime.Stop();
            ExecutionReport(NewRows, totalTime.Elapsed);

            IsProcessing = false;
        }



        /// <summary>
        /// Writes as many script files as needed (the bulk inserts are split into different files to avoid high memory requirements when executing them)
        /// </summary>
        /// <param name="rowNum">Number of rows to be inserted</param>
        /// <param name="processTableName">The name of the tables section to be processed</param>
        /// <param name="ScriptGenerator">Function which writes the script file</param>
        private void WriteScriptFiles(long rowNum, string processTableName, Action<string, long, ushort, ushort> ScriptGenerator)
        {
            uint currentNewRows = 0;

            // Number of files to be generated
            ushort totalParts = (ushort)Math.Ceiling((float)rowNum / GymAppSQLiteConfig.RowsPerScriptFile);

            // Split files so they don't exceed the maximum number of rows per file
            for (ushort iPart = 0; iPart < totalParts; iPart++)
            {
                // Compute number of rows wrt the number of files
                currentNewRows = (uint)(iPart == totalParts - 1 ? rowNum - (iPart * GymAppSQLiteConfig.RowsPerScriptFile) : GymAppSQLiteConfig.RowsPerScriptFile);
                // Write
                ScriptGenerator(processTableName, rowNum, (ushort)(iPart + 1), totalParts);
            }

            NewRows += GymWrapper.CurrentRow;
        }


        /// <summary>
        /// Creates the SQL script file for the Messages type tables
        /// </summary>
        /// <param name="tablesTypeName">Name of the tables type to be processed</param>
        /// <param name="rowNum">Number of rows to be inserted</param>
        /// <param name="partNumber">File part number</param>
        /// <param name="totalParts">File parts total number</param>
        private void CreateMessageTablesScript(string tablesTypeName, long rowNum, ushort partNumber, ushort totalParts)
        {
            try
            {
                BulkInsertScriptDbWriter writer = new BulkInsertScriptDbWriter(GymAppSQLiteConfig.SqlScriptFolder, DbName, GetScriptFileFullpath(tablesTypeName, partNumber, totalParts));
                GymWrapper = new DbWrapper(writer);

                GymWrapper.PopulateNotesTables(rowNum);
            }
            catch(Exception exc)
            {
                RaiseError($"{GetScriptFileFullpath(tablesTypeName, partNumber, totalParts)} - {exc.Message}");
            }
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
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "Relations",
                TotalRows = 0,
                Enabled = false,
                OrderNumber = count++,
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "FullUserData",
                TotalRows = 0,
                Enabled = true,
                OrderNumber = count++,
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "UserPosts",
                TotalRows = 0,
                Enabled = false,
                OrderNumber = count++,
            });

            ProcessTablesData.Add(new TableProcessData()
            {
                TableName = "UserTraining",
                TotalRows = 0,
                Enabled = false,
                OrderNumber = count++,
            });
        }

        #endregion
    }
}
