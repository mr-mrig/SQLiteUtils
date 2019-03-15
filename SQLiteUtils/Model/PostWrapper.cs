using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class PostWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "Post";
        private const string tableTemplate = "post";
        #endregion




        #region Ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public PostWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {

        }
        #endregion



    }
}
