using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.ORM.EF
{



    public class GymAppDbTestContext : DbContext
    {



        #region Consts

        private const string DefaultDbName = "TestDb.sql";
        #endregion



        #region Private Fields

        private string _dbName;
        #endregion



        #region DbSet

        public DbSet<User> Users { get; set; }
        public DbSet<AccountStatusType> AccountStatusTypes { get; set; }
        public DbSet<Post> Posts{ get; set; }
        public DbSet<FitnessDayEntry> FitnessDayEntres { get; set; }
        public DbSet<Weight> Weights { get; set; }
        public DbSet<Wellness> Wellnesses { get; set; }
        public DbSet<ActivityDay> ActivityDays { get; set; }
        public DbSet<DietDay> DietDays { get; set; }
        public DbSet<DietDayType> DietDayTypes { get; set; }
        public DbSet<Mus> Muses { get; set; }
        #endregion




        #region Ctors

        public GymAppDbTestContext() : this(DefaultDbName) { }


        public GymAppDbTestContext(string dbName) : this(dbName, false) { }


        public GymAppDbTestContext(string dbName, bool isDummyInit) : base(dbName)
        {
            _dbName = dbName;

            // Dummy initialization
            Database.SetInitializer<GymAppDbTestContext>(new GymAppDbTestInitializer());
        }
        #endregion




        #region Public Methods



        #endregion



    }
}
