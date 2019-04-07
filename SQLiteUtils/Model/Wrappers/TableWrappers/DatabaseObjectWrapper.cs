using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SQLiteUtils.Model
{

    public class DatabaseObjectWrapper
    {

        #region Static
        public long GeneratedEntryNumber { get; set; } = 0;
        #endregion



        #region Properties       
        /// <summary>
        /// The opened SQLite connection to the database
        /// </summary>
        public virtual SQLiteConnection SqlConnection { get; private set; }

        /// <summary>
        /// The table name which the object maps
        /// </summary>
        public virtual string TableName { get; private set; }


        /// <summary>
        /// List of table column objects representing an Entry.
        /// </summary>
        public virtual List<DatabaseColumnWrapper> Entry { get; protected set; } = null;

        private long _maxId;
        /// <summary>
        /// Table Maximum Id. 
        /// If no rows have been deleted then all Ids span from 1 to MaxId (included)
        /// </summary>
        public virtual long MaxId
        {
            get => _maxId;
            set
            {
                if (value < 0)
                    throw new OverflowException($"Max ID cannot be a negative number. Check for arithmetic overflow");

                _maxId = value;
            }
        }
        #endregion



        #region Ctors

        /// <summary>
        /// Generic constructor for a DatabaseObject mapping a specific DB table
        /// </summary>
        /// <param name="connection">An SQLite already opened connection</param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="hasId">Auto-increase ID present/missing.</param>
        public DatabaseObjectWrapper(SQLiteConnection connection, string tableName, bool hasId = true)
        {
            SqlConnection = connection;
            TableName = tableName;

            // Get maximum ID
            MaxId = hasId ? GetMaxId() : 0;

            // Get columns definition
            Entry = DatabaseUtility.GetColumnsDefinition(SqlConnection, TableName, hasId);
        }
        #endregion



        #region Virtual Methods

        /// <summary>
        /// Generates an entry with random values.
        /// Childs sure ensure integrity if overriding.
        /// <param name="foreignKeyId">The foreign key Id which is primary key for the table. To be used only if the caller want to specify it.</param>
        /// </summary>
        public virtual List<DatabaseColumnWrapper> Create(long foreignKeyId = 0)
        {
            foreach (DatabaseColumnWrapper col in Entry)
            {
                try
                {
                    col.Value = RandomFieldGenerator.GenerateRandomField(col).Value;
                }
                catch
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
            return Entry;
        }


        /// <summary>
        /// Get the predefinied and static entries, if they exist.
        /// </summary>
        /// <returns>A string representing the entry values</returns>
        public virtual List<DatabaseColumnWrapper> GetPredefinedEntries()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get a string representation suitable for SQL insert statements
        /// </summary>
        /// <returns>A string representing the entry values</returns>
        public virtual string ToSqlString()
        {
            string strEntry = string.Empty;

            foreach(DatabaseColumnWrapper col in Entry)
            {
                try
                {
                    if (col.Value == null)
                        strEntry += "NULL, ";

                    else
                    {
                        if (col.Value.GetType() == typeof(object))
                            strEntry += col.Value.ToString() + ", ";

                        else if (col.Value.GetType() == typeof(string))
                            strEntry += $@"'{col.Value.ToString().ToString(CultureInfo.GetCultureInfo("en-US"))}', ";

                        else
                            // Point as decimal separator
                            strEntry += float.Parse(col.Value.ToString()).ToString(CultureInfo.GetCultureInfo("en-US")) + ", ";
                    }


                    //if (col.Value == null || col.Affinity == TypeAffinity.Null)
                    //    strEntry += "NULL, ";

                    //else
                    //{
                    //    if (col.Value.GetType() == typeof(object))
                    //        strEntry += col.Value.ToString() + ", ";

                    //    else if (col.Value.GetType() == typeof(string))
                    //        strEntry += $@"'{col.Value.ToString().ToString(CultureInfo.GetCultureInfo("en-US"))}', ";

                    //    else
                    //        // Point as decimal separator
                    //        strEntry += float.Parse(col.Value.ToString()).ToString(CultureInfo.GetCultureInfo("en-US")) + ", ";
                    //}
                }
                catch(Exception exc)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }

            return strEntry.Substring(0, strEntry.Length - 2);
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Get the maximum ID inside the table (or -1 if no ID field)
        /// </summary>
        /// <returns></returns>
        protected long GetMaxId()
        {
            // Get Table last ID
            return DatabaseUtility.GetTableMaxId(SqlConnection, TableName);
        }
        #endregion
    }
}
