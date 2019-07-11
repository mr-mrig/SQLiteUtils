﻿using SQLiteUtils.Commands;
using SQLiteUtils.Model.ORM;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using Dapper;

namespace SQLiteUtils.ViewModel
{

    public class QueryManagerViewModel : DbManagerBaseViewModel
    {



        #region Consts

        private const string DefaultTitle = "Query Manager";
        #endregion



        #region Properties

        public ParameterlessCommandAsync ExecDAOCommandAsync { get; private set; }
        public ParameterlessCommandAsync ExecEFCommandAsync { get; private set; }
        public ParameterlessCommandAsync ExecDapperCommandAsync { get; private set; }

        public long RowCounter { get; private set; } = 0;
        #endregion



        #region INotifyPropertyChanged Implementation

        private List<string> _sqlQueryHistory = new List<string>();

        /// <summary>
        /// History of the commands of the current session
        /// </summary>
        public List<string> SqlQueryHistory
        {
            get => _sqlQueryHistory;
            set => SetProperty(ref _sqlQueryHistory, value);
        }


        private string _sqlCommand = string.Empty;

        /// <summary>
        /// The SQL command
        /// </summary>
        public string SqlCommand
        {
            get => _sqlCommand;
            set
            {
                SetProperty(ref _sqlCommand, value);
                //SqlQueryHistory?.Add(_sqlCommand);
            }
        }

        private float _elapsedSeconds = 0;

        /// <summary>
        /// Query in process
        /// </summary>
        public float ElapsedSeconds
        {
            get => _elapsedSeconds;
            set
            {
                SetProperty(ref _elapsedSeconds, value);
            }
        }

        private string _queryLog;

        /// <summary>
        /// Query in process
        /// </summary>
        public string QueryLog
        {
            get => _queryLog;
            set
            {
                SetProperty(ref _queryLog, value);
            }
        }
        #endregion


        #region Ctors

        /// <summary>
        /// ViewModel for the Query Manager
        /// </summary>
        /// <param name="isProcessingChangedAction">Function to be called when the processing status changes (IE: operation starts/stops)</param>
        /// <param name="onErrorAction">Function to be called when an error is raised.</param>
        public QueryManagerViewModel(Action<bool> isProcessingChangedAction, Action<string> onErrorAction)
            : base(DefaultTitle, isProcessingChangedAction, onErrorAction)

        {
            ExecDAOCommandAsync = new ParameterlessCommandAsync(ExecQueryDaoAsync, () => !IsProcessing);
            ExecEFCommandAsync = new ParameterlessCommandAsync(ExecEfQueryAsync, () => !IsProcessing);
            ExecDapperCommandAsync = new ParameterlessCommandAsync(ExecDapperQueryAsync, () => !IsProcessing);
            DbContext = new MyGymAppModel();
        }
        #endregion





        //public class TempResult
        //{
        //    public long UserId { get; set; }
        //    public long PostId { get; set; }
        //    public long CommentId { get; set; }
        //}

        #region Public Methods

        /// <summary>
        /// Relat method for executing the query asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task ExecQueryDaoAsync()
        {
            IsProcessing = true;

            try
            {
                Stopwatch elapsed = new Stopwatch();
                elapsed.Start();

                await Task.Run(() => ExecQueryDAO());

                elapsed.Stop();
                ElapsedSeconds = (float)elapsed.Elapsed.TotalMilliseconds / 1000;

                QueryLog = $"Fetched rows: {RowCounter.ToString()} {Environment.NewLine}"
                    + $"In {Math.Round(ElapsedSeconds, 2).ToString()} s";
            }
            catch (Exception exc)
            {
                RaiseError(exc.Message);
            }
            finally
            {
                IsProcessing = false;
            }
        }


        /// <summary>
        /// Executes the query
        /// </summary>
        public void ExecQueryDAO()
        {
            IsProcessing = true;
            RowCounter = 0;

            try
            {
                SQLiteCommand query = new SQLiteCommand()
                {
                    Connection = this.Connection,
                    CommandText = SqlCommand,
                };


                using (SQLiteDataReader sqlRead = query.ExecuteReader() as SQLiteDataReader)
                {

                    while (sqlRead.Read())
                    {
                        RowCounter++;

                        //for (int icol = 0; icol < sqlRead.FieldCount; icol++)
                        //{
                        //    TypeAffinity type = sqlRead.GetFieldAffinity(icol);
                        //    dynamic val;

                        //    switch (type)
                        //    {
                        //        case TypeAffinity.Int64:

                        //            val = sqlRead.GetInt64(icol);
                        //            Console.WriteLine(Convert.ChangeType(val, typeof(string)));
                        //            break;

                        //        case TypeAffinity.Text:
                        //            val = sqlRead.GetString(icol);
                        //            Console.WriteLine(Convert.ChangeType(val, typeof(string)));
                        //            break;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception exc)
            {
                RaiseError(exc.Message);
            }
            finally
            {
                IsProcessing = false;
            }
        }


        public async Task ExecEfQueryAsync()
        {
            IsProcessing = true;

            try
            {
                Stopwatch elapsed = new Stopwatch();
                elapsed.Start();

                await Task.Run(() => ExecEfQuery());

                elapsed.Stop();
                ElapsedSeconds = (float)elapsed.Elapsed.TotalMilliseconds / 1000;

                QueryLog = $"Fetched rows: {RowCounter.ToString()} {Environment.NewLine}"
                    + $"In {Math.Round(ElapsedSeconds, 2).ToString()} s";
            }
            catch (Exception exc)
            {
                RaiseError(exc.Message);
            }
            finally
            {
                IsProcessing = false;
            }

        }




        //public class TestWrapper
        //{
        //    public Post Post1 { get; set; }
        //    public Post Post2 { get; set; }
        //}


        public void ExecEfQuery()
        {
            IsProcessing = true;
            RowCounter = 0;


            try
            {
                // Check if the user entered any command to be executed
                if (string.IsNullOrWhiteSpace(SqlCommand))
                {
                    //// If not, perform the following test query via LINQ
                    //var query = DbContext.Post
                    //    .Join(DbContext.User, p => p.UserId, u => u.Id, (p, u) => new { p, u })
                    //    .Join(DbContext.Image, x => x.p.Id, i => i.PostId, (u1, i) => new { u1, i })

                    //    .Join(DbContext.FitnessDayEntry, x => x.u1.p.Id, f => f.Id, (u2, f) => new { u2, f })
                    //    .Join(DbContext.DietDay, x => x.f.Id, dd => dd.Id, (u3, dd) => new { u3, dd })
                    //    .Join(DbContext.DietDayType, x => x.dd.DietDayTypeId, ddt => ddt.Id, (u4, ddt) => new { u4, ddt })
                    //    .Join(DbContext.Weight, x => x.u4.u3.f.Id, w => w.Id, (u5, w) => new { u5, w })
                    //    .Join(DbContext.ActivityDay, x => x.u5.u4.u3.f.Id, ad => ad.Id, (u6, ad) => new { u6, ad })
                    //    .Join(DbContext.WellnessDay, x => x.u6.u5.u4.u3.f.Id, wd => wd.Id, (u7, wd) => new { u7, wd })

                    //.Join(DbContext.DietPlan, x => x.u7.u6.u5.u4.u3.u2.u1.p.Id, dp => dp.Id, (u8, dp) => new { u8, dp })
                    //.Join(DbContext.DietPlanUnit, x => x.dp.Id, dpu => dpu.DietPlanId, (u9, dpu) => new { u9, dpu })
                    //.Join(DbContext.DietPlanDay, x => x.dpu.Id, dpd => dpd.DietPlanUnitId, (u10, dpd) => new { u10, dpd })
                    //.Join(DbContext.DietPlanDay, x => x.dpd.DietDayTypeId, ddt => ddt.Id, (u11, ddt) => new { u11, ddt })

                    //.Join(DbContext.MeasuresEntry, x => x.u11.u10.u9.u8.u7.u6.u5.u4.u3.u2.u1.p.Id, meas => meas.Id, (u12, meas) => new { u12, meas })
                    //.Join(DbContext.Circumference, x => x.meas.Id, circ => circ.Id, (u13, circ) => new { u13, circ })
                    //.Join(DbContext.BiaEntry, x => x.u13.meas.Id, bia => bia.Id, (u14, bia) => new { u14, bia })
                    //.Join(DbContext.Plicometry, x => x.u14.u13.meas.Id, pli => pli.Id, (u15, pli) => new { u15, pli })

                    ////.Where(x => x.u7.u6.u5.u4.u3.u2.u1.u.Id == 12).Take(20);


                    //.Where(x => x.u15.u14.u13.u12.u11.u10.u9.u8.u7.u6.u5.u4.u3.f.Id == 12)
                    //.Select(x => new
                    // {
                    //     EntryType = x.u15.u14.u13.u12.u11.u10.u9.u8.u7.u6.u5.u4.u3.f.Id > 0 ? "FitDay"
                    //            : x.u15.u14.u13.meas.Id > 0 ? "Meas"
                    //            : x.u15.u14.u13.u12.u11.u10.u9.dp.Id > 0 ? "DietPlan"
                    //            : "",

                    // }).Take(20);



                    var query = DbContext.Post
                        .Join(DbContext.Post, p1 => p1.Id, p2 => p2.UserId, (p1, p2) => new { p1, p2 })
                        .Take(1000000)
                        .Select(x => "hi");




                    IQueryable query2 = DbContext.TrainingPlan
                        .Join(DbContext.User, tp => tp.User.Id, u => u.Id, (tp, u) => new { tp, u })
                        .Where(x => x.u.Id == 12)
                        .OrderBy(x => x.tp.IsBookmarked)
                        .Select(x => new
                        {
                            PlanName = x.tp.Name,
                            IsBookmarked = x.tp.IsBookmarked,
                            Owner = x.u.Username,
                            AvgWorkoutDays = DbContext.TrainingWeek
                                                .Join(DbContext.WorkoutTemplate, tw => tw.Id, wt => wt.TrainingWeekId, (tw, wt) => new { tw, wt })
                                                .Where(y => y.tw.TrainingPlanId == x.tp.Id)
                                                .GroupBy(g => g.tw.Id)
                                                .Select(g => new
                                                {
                                                    //AvgWorkoutDays = g.Count()
                                                    Ciao = 1
                                                }).Average(z => z.Ciao),

                        });
                    
                    IQueryable query3 = DbContext.Post
                        .GroupJoin(DbContext.FitnessDayEntry, post => post.Id, fentry => fentry.Id, (post, fentry) => new { post, post.FitnessDayEntry })
                        .GroupJoin(DbContext.Weight, join1 => join1.post.Id, weight => weight.Id, (join1, weight) => new { join1, weight})
                        .Where(x => x.join1.post.Id < 1000);

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(query3.ToString());

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(query2.ToString());

                    //foreach (var row in query)
                    //{
                    //    //Console.WriteLine(row.EntryType.ToString());
                    //    RowCounter++;
                    //}

                    // Print the query ran - Useful when LINQ is used
                    //Console.WriteLine(query.ToString());
                }
                else
                {
                    // Otherwise execute what the user requested
                    //System.Data.Entity.Infrastructure.DbRawSqlQuery<List<string>> query = DbContext.Database.SqlQuery<List<string>>(SqlCommand);
                    //System.Data.Entity.Infrastructure.DbRawSqlQuery<dynamic> query = DbContext.Database.SqlQuery<dynamic>(SqlCommand);
                    System.Data.Entity.Infrastructure.DbRawSqlQuery<string> query = DbContext.Database.SqlQuery<string>(SqlCommand);

                    foreach (var row in query)
                    {
                        //Console.WriteLine(row.EntryType.ToString());
                        RowCounter++;
                    }
                }
            }
            catch (Exception exc)
            {
                RaiseError(exc.Message);
            }
            finally
            {
                IsProcessing = false;
            }


            //foreach (TempResult row in query)
            //{
            //    //Console.WriteLine($"{user.Id.ToString()} + {user.Post.First().Id}");
            //    Console.WriteLine($"{row.UserId.ToString()} - {row.PostId.ToString()}");
            //    //Console.WriteLine(row.ToString());
            //    RowCounter++;
            //}

        }




        public async Task ExecDapperQueryAsync()
        {
            IsProcessing = true;

            try
            {
                Stopwatch elapsed = new Stopwatch();
                elapsed.Start();

                await Task.Run(() => ExecDapperQuery());

                elapsed.Stop();
                ElapsedSeconds = (float)elapsed.Elapsed.TotalMilliseconds / 1000;

                QueryLog = $"Fetched rows: {RowCounter.ToString()} {Environment.NewLine}"
                    + $"In {Math.Round(ElapsedSeconds, 2).ToString()} s";
            }
            catch (Exception exc)
            {
                RaiseError(exc.Message);
            }
            finally
            {
                IsProcessing = false;
            }

        }


        public void ExecDapperQuery()
        {
            IsProcessing = true;
            RowCounter = 0;

            try
            {
                //var query = DbContext.Database.Connection.Query<User>(SqlCommand).ToList();
                var query = Connection.Query<string>(SqlCommand).ToList();

                foreach (var row in query)
                {
                    //Console.WriteLine(row.EntryType.ToString());
                    RowCounter++;
                }
            }
            catch (Exception exc)
            {
                RaiseError(exc.Message);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        #endregion



        #region Private Methods

        #endregion

    }
}
