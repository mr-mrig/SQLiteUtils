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

        #endregion


        #region INotifyPropertyChanged Implementation

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
            ExecuteSqlCommandAsync = new ParameterlessCommandAsync(ExecuteSqlWrapperAsync, () => !IsExecutingSql);

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
            Stopwatch partialTime;
            string tablesSelectionName;
            long rowNum;

            Stopwatch totalTime = new Stopwatch();
            totalTime.Start();

            IsProcessing = true;
            NewRows = 0;
            TotalRows = 1;

            tablesSelectionName = "Messages";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

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

                await Task.Run(() => GymWrapper.PopulateNotesTables(rowNum));

                GymWrapper.DbWriter.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }


            tablesSelectionName = "Relations";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

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

                await Task.Run(() => GymWrapper.PopulateNotesTables(rowNum));

                GymWrapper.DbWriter.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            tablesSelectionName = "FullUserData";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

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

                await Task.Run(() => GymWrapper.PopulateNotesTables(rowNum));

                GymWrapper.DbWriter.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }
            
            tablesSelectionName = "UserPosts";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

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

                await Task.Run(() => GymWrapper.PopulateNotesTables(rowNum));

                GymWrapper.DbWriter.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            tablesSelectionName = "UserTraining";
            rowNum = ProcessTablesData.Where(x => x.TableName == tablesSelectionName).First().TotalRows;

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

                await Task.Run(() => GymWrapper.PopulateNotesTables(rowNum));

                GymWrapper.DbWriter.Close();

                // Log
                partialTime.Stop();
                EndTableLog(tablesSelectionName, partialTime.Elapsed);
            }

            totalTime.Stop();
            ExecutionReport(NewRows, totalTime.Elapsed);

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

            // Init Wrappers
            DbWriter.DbPath = DbName;
            DbWriter.Open();
            BuildDbWrapper();

            await Task.Run(() => processFun(rowNum));

            GymWrapper.DbWriter.Close();

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
