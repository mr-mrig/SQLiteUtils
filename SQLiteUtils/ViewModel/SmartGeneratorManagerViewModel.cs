using SQLiteUtils.Commands;
using SQLiteUtils.Model;
using SQLiteUtils.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.ViewModel
{

    public class SmartGeneratorManagerViewModel : DbManagerBaseViewModel
    {


        #region Enums

        #endregion


        #region Consts

        private const string DefaultTitle = "Query Manager";
        #endregion



        #region Properties
        
        public ParameterlessCommandAsync CreateSqlScriptCommandAsync { get; set; } = null;
        #endregion


        #region INotifyPropertyChanged Implementation


        private ObservableCollection<TableProcessData> _processTablesData;
        public ObservableCollection<TableProcessData> ProcessTablesData
        {
            get => _processTablesData;
            set => SetProperty(ref _processTablesData, value);
        }
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
            IsProcessing = true;

            long rowNum = ProcessTablesData.Where(x => x.TableName == "Messages").First().TotalRows;

            if (rowNum > 0)
            {
                await Task.Run(() => CreateMessageTablesScript(GymAppSQLiteConfig.SqlScriptFolder, DbName));
            }

            rowNum = ProcessTablesData.Where(x => x.TableName == "Relations").First().TotalRows;
            if (rowNum > 0)
            {
                //await Task.Run(() => CreateMessageTablesScript(GymAppSQLiteConfig.SqlScriptFolder, DbName));
            }

            rowNum = ProcessTablesData.Where(x => x.TableName == "FullUserData").First().TotalRows;
            if (rowNum > 0)
            {
                //await Task.Run(() => CreateMessageTablesScript(GymAppSQLiteConfig.SqlScriptFolder, DbName));
            }

            rowNum = ProcessTablesData.Where(x => x.TableName == "UserPosts").First().TotalRows;
            if (rowNum > 0)
            {
                //await Task.Run(() => CreateMessageTablesScript(GymAppSQLiteConfig.SqlScriptFolder, DbName));
            }

            rowNum = ProcessTablesData.Where(x => x.TableName == "UserTraining").First().TotalRows;
            if (rowNum > 0)
            {
                //await Task.Run(() => CreateMessageTablesScript(GymAppSQLiteConfig.SqlScriptFolder, DbName));
            }

            IsProcessing = false;
        }


        private void CreateMessageTablesScript(string rootDir, string databasePath)
        {
            BulkInsertScriptDbWriter writer = new BulkInsertScriptDbWriter(rootDir, databasePath);
            DbWrapper dbWrapper = new DbWrapper(writer);

            dbWrapper.PopulateNotesTables(2);
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
