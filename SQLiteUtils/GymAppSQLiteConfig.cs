using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLiteUtils
{
    public static class GymAppSQLiteConfig
    {


        #region Global Environment Contants
        public const uint RowsPerScriptFile = 2 * 1000000;          // Split the script files to a maximum number of rows. Tune this to avoid OutOfMemoryException.

        public const string ValidSqlScriptRegex = @".:\\([ A-z0-9-_+]+\\)*PopulateTablesScript([A-z0-9]+\.(sql))";
        public const string SqlScriptPrefix = @"PopulateTablesScript";
        public const string SQLiteRoot = @"D:\Gym App\SQLite";

        public static readonly string SpeedOptimizingSqlPragmas = $@"PRAGMA journal_mode = OFF; PRAGMA page_size = {(ushort.MaxValue + 1).ToString()}; PRAGMA synchronous=OFF";

        public static readonly string SqlScriptFolder = $@"{SQLiteRoot}\Databases\Script\";
        public static readonly string DbName = $@"{SQLiteRoot}\Databases\GymApp.db";                                                      // Database name
        public static readonly string SqlScriptFilePath = Path.Combine(SqlScriptFolder, $@"{SqlScriptPrefix}_##suffix##_##part##.sql");       // File storing the SQL statements
        public static readonly string SqlTempFilePath = Path.Combine(SqlScriptFolder, $@"TempFile");
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
        #endregion

    }
}
