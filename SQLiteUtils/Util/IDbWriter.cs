using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteUtils.Model;



namespace SQLiteUtils.Util
{



    public interface IDbWriter : IDisposable
    {

        #region Properties

        /// <summary>
        /// SQLite connection to the database
        /// </summary>
        SQLiteConnection SqlConnection { get; set; }

        /// <summary>
        /// Table wrappers to be processed
        /// </summary>
        List<DatabaseObjectWrapper> TableWrappers { get; set; }

        /// <summary>
        /// Databse path
        /// </summary>
        string DbPath { get; set; }
        #endregion





        /// <summary>
        /// Open the communication and initializes the structures to be sued.
        /// </summary>
        /// </summary>
        void Open();

        /// <summary>
        /// Start of the operations considered as a whole bulk insert.
        /// </summary>
        void StartTransaction();

        /// <summary>
        /// Process the bulk insert action. Inserts might be split to balance the workload.
        /// </summary>
        /// <param name="bulkInsertAction">Action that performs the bulk insert</param>
        /// <param name="rowNum">Number of rows to be processed</param>
        void ProcessTransaction(string processTitle, Func<long, long> bulkInsertAction, long rowNum, uint rowsPerScriptFile = 0);

        /// <summary>
        /// Append data to be written.
        /// Must be called after StartTransaction.
        void Append();

        /// <summary>
        /// End of the operations considered as a whole bulk insert.
        /// </summary>
        void EndTransaction();

        /// <summary>
        /// Perform the Bulk inserts saved before.
        /// Must be called after StartTransaction - EndTransaction
        /// </summary>
        /// <param name="entry">Object Wrapper of the entry to be inserted</param>
        void Write(DatabaseObjectWrapper entry);

        /// <summary>
        /// Resources release and final operations.
        /// </summary>
        void Close();

    }
}
