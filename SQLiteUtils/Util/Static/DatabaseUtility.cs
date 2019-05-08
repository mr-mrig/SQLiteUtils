using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using SQLiteUtils.Model;
using System.IO;

namespace SQLiteUtils
{
    static class DatabaseUtility
    {




        #region Consts
        public const int UnixTimestampSixMonthsDelta = 20000000;
        public const int UnixTimestampThreeMonthsDelta = 7948800;
        public const int UnixTimestampOneMonthDelta = 2500000;
        public const int UnixTimestampOneWeekDelta = 604800;
        //public const int UnixTimestampOneDay = 86400;
        //public const int UnixTimestampOneDay = 86900;
        public const int UnixTimestampOneDay = 87600;
        public const int UnixTimestampOneHourDelta = 3600;
        public const int UnixTimestampThreeHoursDelta = 10800;
        public static readonly DateTime UnixTimestampT0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        #endregion



        public static int GetUnixTimestamp(DateTime date)
        {
            return (int)(date - UnixTimestampT0).TotalSeconds;
        }


        
        /// <summary>
        /// Get columns definition of the selected table
        /// </summary>
        /// <param name="connection">A SQLite opened connection</param>
        /// <param name="tableName">The name of the table to be queried</param>
        /// <param name="skipId">Skip or include the ID column</param>
        /// <returns>A tuple with the columns name as Item1 and their AffinityType as Item2</returns>
        public static List<DatabaseColumnWrapper> GetColumnsDefinition(SQLiteConnection connection, string tableName, bool skipId = true)
        {
            SQLiteDataReader sqlr;
            List<DatabaseColumnWrapper> columns = new List<DatabaseColumnWrapper>();


            // Get columns definition
            SQLiteCommand cmd = new SQLiteCommand()
            {
                Connection = connection,
                CommandText = $"SELECT * FROM {tableName} LIMIT(1)",
            };

            try
            {
                sqlr = cmd.ExecuteReader();
            }
            catch (Exception)
            {
                return null;
            }

            // Fetch columns name (skip ID column)
            for (int icol = skipId ? 1 : 0; icol < sqlr.FieldCount; icol++)
            {
                columns.Add(new DatabaseColumnWrapper()
                {
                    Name = sqlr.GetName(icol),
                    Affinity = sqlr.GetFieldAffinity(icol),
                    Value = null,
                });
            }
                //columns.Add(sqlr.GetName(icol), sqlr.GetFieldAffinity(icol));

            return columns;
        }

        /// <summary>
        /// Get the Ids from the AccountStatusType table
        /// </summary>
        /// <param name="connection">An opened SQLite connection</param>
        /// <returns>The Id list</returns>
        public static List<int> GetAccountStatusTypeIds(SQLiteConnection connection)
        {
            return GetTableIds(connection, "AccountStatusType", "WHERE Id <> 8");
        }


        /// <summary>
        /// Get the Ids from the selected table table
        /// </summary>
        /// <param name="connection">An opened SQLite connection</param>
        /// <param name="tableName">The table to be queried</param>
        /// <param name="whereCondition">The where condition which the table must be filtered by</param>
        /// <returns>The Id list</returns>
        public static List<int> GetTableIds(SQLiteConnection connection, string tableName, string whereCondition = "")
        {
            List<int> ids = new List<int>();

            SQLiteCommand cmd = new SQLiteCommand(connection)
            {
                CommandText = $"SELECT Id FROM {tableName} {whereCondition}"
            };
            SQLiteDataReader sqlr = cmd.ExecuteReader();

            while (sqlr.Read())
            {
                ids.Add(sqlr.GetInt32(0));
            }

            sqlr.Close();
            return ids;
        }


        /// <summary>
        /// Get the maximum ID of the selected table. 
        /// </summary>
        /// <param name="connection">An opened SQLite connection</param>
        /// <param name="tableName">The table name to be queried</param>
        /// <returns>Returns the Max Id.</returns>
        public static int GetTableMaxId(SQLiteConnection connection, string tableName, bool hasId = false)
        {
            SQLiteDataReader sqlr = null;
            int maxId = 0;

            if (connection == null || connection?.State != System.Data.ConnectionState.Open)
                throw new SQLiteException("The connection is not opened");

            // If table has ID, just look into the Sequence table
            if (hasId)
            {
                // Get max id
                SQLiteCommand cmd = new SQLiteCommand()
                {
                    Connection = connection,
                    CommandText = $"SELECT seq FROM sqlite_sequence where name = '{tableName}'",
                };

                try
                {
                    sqlr = cmd.ExecuteReader();
                }
                catch (Exception)
                {
                    sqlr.Close();
                    throw new SQLiteException($"GetTableMaxId: error while performing SQL operation. Please check the DB. ");
                }

                if (sqlr.Read())
                    maxId = sqlr.GetInt32(0);

                else
                    maxId = 0;
            }
            else
                maxId = CountTableRows(connection, tableName);

            sqlr?.Close();

            return maxId;
        }



        /// <summary>
        /// Get the number of rows of the specified table
        /// </summary>
        /// <param name="connection">An opened SQLite connection</param>
        /// <param name="tableName">The table to be queried</param>
        /// <param name="whereCondition">The where condition which the table must be filtered by</param>
        /// <returns>The Id list</returns>
        public static int CountTableRows(SQLiteConnection connection, string tableName, string whereCondition = "")
        {

            SQLiteCommand cmd = new SQLiteCommand(connection)
            {
                CommandText = $"SELECT Count(*) FROM {tableName} {whereCondition}"
            };
            SQLiteDataReader sqlr = cmd.ExecuteReader();

            if (sqlr.HasRows)
            {
                try
                {
                    return sqlr.GetInt32(0);
                }
                catch(Exception exc)
                {
                    return 0;
                }
                finally
                {
                    sqlr.Close();
                }
            }
            else
                throw new SQLiteException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} : Error while reading {tableName} ");
        }



        /// <summary>
        /// Get the names of the tables storing notes or messages, which are made up of two columns: Id, Body
        /// </summary>
        /// <param name="connection">An opened SQLite connection</param>
        /// <returns>The list of the table names</returns>
        public static List<string> GetNotesTables(SQLiteConnection connection)
        {
            List<string> tableNames = new List<string>();
            SQLiteDataReader sqlread;

            if (connection == null || connection?.State != System.Data.ConnectionState.Open)
                throw new SQLiteException("The connection is not opened");

            // Query the DB for specific table names referring to Notes
            SQLiteCommand cmd = new SQLiteCommand()
            {
                Connection = connection,
                CommandText = $"SELECT name FROM sqlite_master" +
                                $"WHERE tbl_name LIKE '%message%'" +
                                $"   OR tbl_name LIKE '%note%'" +
                                $"   OR tbl_name LIKE '%annotation%'" +
                                $"   OR tbl_name LIKE '%comment%'",
            };

            try
            {
                sqlread = cmd.ExecuteReader();
            }
            catch
            {
                throw new SQLiteException($"GetNotesTables: error while performing SQL operation. Please check the DB. ");
            }

            // Append the results
            while (sqlread.Read())
                tableNames.Add(sqlread.GetString(0));

            sqlread.Close();

            return tableNames;
        }


        /// <summary>
        /// Get the names of the tables storing notes or messages, choosing among a list of wrappers according to the derived type of each instance. 
        /// The search criteria are: WrapperInstance.GetType() = DatabaseObjectWrapper and WrapperInstance has only one column which affinity is 'Text'
        /// </summary>
        /// <param name="tableWrappers">The list of table wrappers to be searched among</param>
        /// <returns>The list of the tables wrappers to be populated</returns>
        public static List<DatabaseObjectWrapper> GetNotesTables(List<DatabaseObjectWrapper>tableWrappers)
        {
            List<DatabaseObjectWrapper> tableNames = new List<DatabaseObjectWrapper>();

            foreach(DatabaseObjectWrapper table in tableWrappers)
            {
                // Consider the objects with generic wrapper only
                if (table.GetType() == typeof(DatabaseObjectWrapper))
                {
                    // Check tables with ID (which is not included) + text field
                    if (table.Entry.Count == 1
                        && (table.Entry[table.Entry.Count - 1].Affinity == TypeAffinity.Text) || table.Entry[table.Entry.Count - 1].Affinity == TypeAffinity.Null)

                        tableNames.Add(table);
                }
            }

            return tableNames;
        }


        /// <summary>
        /// Execute the Sql script file,
        /// </summary>
        /// <param name="scriptPath">Script full path</param>
        /// <param name="connection">An already opened SQLite connection</param>
        /// <returns></returns>
        public static async Task<long> ExecuteSqlScript(string scriptPath, SQLiteConnection connection)
        {
            long ret = 0;

            if(connection.State != System.Data.ConnectionState.Open)
                throw new Exception("{Path.GetFileName(srciptPath)} - Connection is not opened");

            using (StreamReader scriptFile = new StreamReader(File.OpenRead(scriptPath)))
            {

                // Import the file as a SQL command
                SQLiteCommand cmd = new SQLiteCommand()
                {
                    Connection = connection,
                    CommandText = scriptFile.ReadToEnd(),           // This might be vulnerable to OutOfMemoryException
                };

                // Execute SQL
                try
                {
                    ret = await Task.Run(() => cmd.ExecuteNonQueryAsync());
                }
                catch (Exception exc)
                {
                    throw new Exception($@"{Path.GetFileName(scriptPath)} - {exc.Message}", exc);
                }
                finally
                {
                    cmd.Dispose();
                }
            }
            return ret;
        }


        /// <summary>
        /// Cast the SQLite column affinity type to the corresponding application type
        /// </summary>
        /// <param name="affinity"></param>
        /// <returns></returns>
        public static Type CastAffinityToType(TypeAffinity affinity)
        {
            Type type;

            switch(affinity)
            {
                case TypeAffinity.Text:

                    type = typeof(string);
                    break;

                case TypeAffinity.Int64:
                case TypeAffinity.DateTime:

                    type = typeof(int);
                    break;

                case TypeAffinity.Double:

                    type = typeof(float);
                    break;

                default:

                    type = typeof(object);
                    break;

            }

            return type;
        }


        /// <summary>
        /// Sets up and opens a SQLite connection (if not already open) configured to be as fast as possibile.
        /// The drawback of this kind of connection is that it's exposed to incoherence issues in case of operation failure.
        /// </summary>
        /// <param name="connection">The SQLLite connection to be configured</param>
        /// <param name="dbName">The database path to enstablish a connection which</param>
        /// <returns>An opened SQL Connection object </returns>
        public static SQLiteConnection OpenFastestSQLConnection(SQLiteConnection connection, string dbName)
        {
            try
            {
                // Init SQLite connection
                if (connection == null || connection?.State != System.Data.ConnectionState.Open)
                {
                    SQLiteConnectionStringBuilder sqlConnStr = GetTradeoffConnectionBuilder(dbName);
                    connection = new SQLiteConnection(sqlConnStr.ToString());
                    connection.Open();
                }
            }
            catch
            {
                return null;
            }
            return connection;
        }

        /// <summary>
        /// Initializes, sets up and opens a SQLite connection (if not already open) configured to be as fast as possibile.
        /// The drawback of this kind of connection is that it's exposed to incoherence issues in case of operation failure.
        /// </summary>
        /// <param name="connection">The SQLLite connection to be configured</param>
        /// <param name="dbName">The database path to enstablish a connection which</param>
        /// <returns>An opened SQL Connection object </returns>
        public static SQLiteConnection NewFastestSQLConnection(string dbName)
        {
            SQLiteConnection connection = new SQLiteConnection();

            try
            {
                // Init SQLite connection
                if (connection == null || connection?.State != System.Data.ConnectionState.Open)
                {
                    SQLiteConnectionStringBuilder sqlConnStr = GetTradeoffConnectionBuilder(dbName);
                    connection = new SQLiteConnection(sqlConnStr.ToString());
                    connection.Open();
                }
            }
            catch
            {
                return null;
            }

            return connection;
        }


        #region Private Methods

        private static SQLiteConnectionStringBuilder GetFastestConnectionBuilder(string dbName)
        {
            return new SQLiteConnectionStringBuilder()
            {
                DataSource = dbName,
                JournalMode = SQLiteJournalModeEnum.Off,
                SyncMode = SynchronizationModes.Off,
                PageSize = ushort.MaxValue + 1,
                DefaultTimeout = 100,
            };
        }


        private static SQLiteConnectionStringBuilder GetTradeoffConnectionBuilder(string dbName)
        {
            return new SQLiteConnectionStringBuilder()
            {
                DataSource = dbName,
                JournalMode = SQLiteJournalModeEnum.Wal,
                SyncMode = SynchronizationModes.Normal,
                PageSize = ushort.MaxValue + 1,
                DefaultTimeout = 100,
            };
        }
        #endregion
    }
}
