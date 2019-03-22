using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using SQLiteUtils.Model;



namespace SQLiteUtils
{
    static class DatabaseUtility
    {





        public const int UnixTimestampSixMonthsDelta = 20000000;
        public const int UnixTimestampOneMonthDelta = 2500000;



        /// <summary>
        /// Get columns definition of the selected table
        /// </summary>
        /// <param name="connection">A SQLite opened connection</param>
        /// <param name="tableName">The name of the table to be queried</param>
        /// <param name="skipId">Skip or include the ID column</param>
        /// <returns>A tuple with the columns name as Item1 and their AffinityType as Item2</returns>
        public static (List<string>, Dictionary<string,TypeAffinity>) GetColumnsDefinition(SQLiteConnection connection, string tableName, bool skipId = true)
        {
            SQLiteDataReader sqlr;
            List<string> columns = new List<string>();
            Dictionary<string, TypeAffinity> colTypes = new Dictionary<string, TypeAffinity>();


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
            catch(Exception exc)
            {
                return (null, null);
            }

            // Fetch columns name (skip ID column)
            for (int icol = skipId ? 1 : 0; icol < sqlr.FieldCount; icol++)
            {
                columns.Add(sqlr.GetName(icol));
                colTypes.Add(columns[columns.Count - 1], sqlr.GetFieldAffinity(icol));
            }

            return (columns, colTypes);
        }

        /// <summary>
        /// Get columns definition of the selected table
        /// </summary>
        /// <param name="connection">A SQLite opened connection</param>
        /// <param name="tableName">The name of the table to be queried</param>
        /// <param name="skipId">Skip or include the ID column</param>
        /// <returns>A tuple with the columns name as Item1 and their AffinityType as Item2</returns>
        public static List<DatabaseColumnWrapper> GetColumnsDefinition(SQLiteConnection connection, string tableName, int dummy, bool skipId = true)
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
                    ValType = CastAffinityToType(sqlr.GetFieldAffinity(icol)),
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

            return ids;
        }



        /// <summary>
        /// Get the maximum ID of the selected table
        /// </summary>
        /// <param name="connection">An opened SQLite connection</param>
        /// <param name="tableName">The table name to be queried</param>
        /// <returns>Returns the Max Id.</returns>
        public static int GetTableMaxId(SQLiteConnection connection, string tableName)
        {
            SQLiteDataReader sqlr;

            // Get max id
            SQLiteCommand cmd = new SQLiteCommand()
            {
                Connection = connection,
                CommandText = $"SELECT seq FROM sqlite_sequence where name = '{tableName}'",
            };

            //SQLiteDataReader sqlr = await Task.Run(() => cmd.ExecuteReaderAsync()) as SQLiteDataReader;
            try
            {
                sqlr = cmd.ExecuteReader();
            }
            catch(Exception exc)
            {
                return -1;
            }

            if (sqlr.Read())
                return sqlr.GetInt32(0);

            else
                return -1;
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
                    SQLiteConnectionStringBuilder sqlConnStr = new SQLiteConnectionStringBuilder()
                    {
                        DataSource = dbName,
                        JournalMode = SQLiteJournalModeEnum.Off,
                        SyncMode = SynchronizationModes.Off,
                        PageSize = ushort.MaxValue + 1,
                        DefaultTimeout = 100,
                    };
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
                    SQLiteConnectionStringBuilder sqlConnStr = new SQLiteConnectionStringBuilder()
                    {
                        DataSource = dbName,
                        JournalMode = SQLiteJournalModeEnum.Off,
                        SyncMode = SynchronizationModes.Off,
                        PageSize = ushort.MaxValue + 1,
                        DefaultTimeout = 100,
                    };
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
    }
}
