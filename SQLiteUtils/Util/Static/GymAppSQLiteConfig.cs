using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLiteUtils
{
    public static class GymAppSQLiteConfig
    {


        #region Enum
        public enum EffortType : byte
        {
            Intensity = 0,
            RM,
            RPE,
            NoValue,
        }
        #endregion


        #region Global Environment Contants
        public const string AppName = "GymApp";
        public const string AppDescription = "DB tools to support development";
        public const string DefaultDbName = "Test.db";
        public const string DbExtension = "db";
        //public static readonly string DefaultDbName = $@"{WorkingDir}\GymApp.db";
        //public static readonly string DefaultDbName = $@"{WorkingDir}\Test.db";

        public const string ValidSqlScriptRegex = @".:\\([ A-z0-9-_+]+\\)*PopulateTablesScript([A-z0-9]+\.(sql))";
        public const string SqlScriptPrefix = @"PopulateTablesScript";
        public static readonly string SpeedOptimizingSqlPragmas = $@"PRAGMA journal_mode = OFF; PRAGMA page_size = {(ushort.MaxValue + 1).ToString()}; PRAGMA synchronous=OFF";

        public const string SQLiteRoot = @"D:\Gym App\SQLite";
        public const string WorkingDir = SQLiteRoot + @"\Databases";
        public static readonly string SqlScriptFolder = $@"{WorkingDir}\Script\";
        public static readonly string SqlScriptFilePath = Path.Combine(SqlScriptFolder, $@"{SqlScriptPrefix}_##suffix##_##part##.sql");
        public static readonly string SqlTempFilePath = Path.Combine(SqlScriptFolder, $@"TempFile");

        /// <summary>
        /// Lower boundary when creating Dates
        /// </summary>
        public static readonly DateTime DbDateLowerBound = new DateTime(2016, 1, 1);
        /// <summary>
        /// Upper boundary when creating Dates
        /// </summary>
        public static readonly DateTime DbDateUpperBound = new DateTime(2019, 3, 31);

        /// <summary>
        /// Default culture info: dot as decimal separator instead of comma
        /// </summary>
        public static CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-US");

        /// <summary>
        /// Default factor which the numbers are divided with before being displayed. EG: 123700000 --> 123.7M
        /// </summary>
        public const float DefaultDisplayScaleFactor = 1000000.0f;
        /// <summary>
        /// Char identifier of the DisplayScaleFactor parameter.
        /// </summary>
        public const char DefaultDisplayScaleFactorName = 'M';

        /// <summary>
        /// Scale factor for converting float nunmbers to int (to save space)
        /// </summary>
        public const ushort FloatToIntScaleFactor = 10;

        public const uint RowsPerScriptFile = 2 * 1000000;          // Split the script files to a maximum number of rows. Tune this to avoid OutOfMemoryException.

        /// <summary>
        /// User Ids reserved for special use
        /// </summary>
        public const int ReservedUserIds = 2;       // TODO: move into initializator objects
        #endregion



        #region Methods

        /// <summary>
        /// Get SQL scripts full path
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetScriptFilesPath()
        {
            return Directory.EnumerateFiles(SqlScriptFolder).Where(x => Regex.IsMatch(x, ValidSqlScriptRegex));
        }


        /// <summary>
        /// Get the full path of the selected Db
        /// </summary>
        /// <param name="dbName">The DB name (with or without the extension)</param>
        /// <returns></returns>
        public static string GetDbFullpath(string dbName)
        {
            return Path.Combine(WorkingDir, Regex.Replace(dbName, ".db", "") + ".db");
        }



        /// <summary>
        /// Get the name list of the databases in aspecified directory. Algorithm doesn't search in sub-folders
        /// </summary>
        /// <param name="path">The directory path to be searched</param>
        /// <returns>A list of databse names</returns>
        public static List<string> GetDatabaseList(string path = WorkingDir)
        {
            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path).Where(x => Regex.IsMatch(x, "[a-zA-Z0-9-_]+.db")).ToList();
            }
            else
                return null;
        }
        #endregion

    }
}