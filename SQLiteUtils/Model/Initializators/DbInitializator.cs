using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model
{
    public class DbInitializator
    {



        #region Consts
        
        #endregion


        #region Properties
        public SQLiteConnection SqlConnection { get; set; }
        #endregion





        public DbInitializator(SQLiteConnection connection)
        {
            SqlConnection = connection;
        }



        #region Public Methods
        public void InitUserTable()
        {
            
        }


        public void InitDietDayTypeTable()
        {

        }
        #endregion



    }
}
