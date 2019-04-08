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


        SQLiteConnection SqlConnection { get; set; }
        string SqlScriptFilename { get; set; }
        List<DatabaseObjectWrapper> TableWrappers { get; set; }


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
