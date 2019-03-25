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
        List<DatabaseObjectWrapper> TableWrappers { get; set; }


        /// <summary>
        /// Open the communication and initializes the structures to be sued.
        /// </summary>
        /// </summary>
        /// <returns>Operation OK / KO</returns>
        bool Open();

        /// <summary>
        /// Start of the operations considered as a whole bulk insert.
        /// </summary>
        /// <returns></returns>
        bool StartTransaction();

        /// <summary>
        /// Append data to be written.
        /// Must be called after StartTransaction.
        /// <returns>Operation OK / KO</returns>
        bool Append();

        /// <summary>
        /// End of the operations considered as a whole bulk insert.
        /// </summary>
        /// <returns></returns>
        bool EndTransaction();

        /// <summary>
        /// Perform the Bulk inserts saved before.
        /// Must be called after StartTransaction - EndTransaction
        /// </summary>
        /// <returns>Operation OK / KO</returns>
        bool Write();

        /// <summary>
        /// Resources release and final operations.
        /// </summary>
        /// <returns>Operation OK / KO</returns>
        bool Close();

    }
}
