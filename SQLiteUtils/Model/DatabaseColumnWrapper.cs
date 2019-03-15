using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model
{



    public class DatabaseColumnWrapper
    {



        #region Properties
        public string Name { get; set; }
        public TypeAffinity Affinity { get; set; }
        public Type ValType { get; set; }
        public object Value { get; set; }
        #endregion



        #region Ctors
        public DatabaseColumnWrapper()
        {
                
        }
        #endregion


    }
}
