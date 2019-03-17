using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class UserPhaseNoteWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "UserPhaseNote";
        #endregion


        #region Private Fields
        #endregion


        #region Properties
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the UserPhaseNote DB table.
        /// </summary>
        /// <param name="connection"></param>
        public UserPhaseNoteWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {

        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> GenerateRandomEntry(long userId = 0)
        {

            // Create new ID
            try
            {
                checked { MaxId++; };
            }
            catch (OverflowException)
            {
                return null;
            }


            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {

                    case "Body":

                        col.Value = RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.RandomInt(50, 500));
                        break;

                    default:

                        col.Value = RandomFieldGenerator.GenerateRandomField(col.Affinity);
                        break;
                }
            }
            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
