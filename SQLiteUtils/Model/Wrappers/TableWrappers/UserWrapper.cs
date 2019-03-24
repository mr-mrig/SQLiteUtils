using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model
{
    public class UserWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "User";
        private const string tableTemplate = "user";
        #endregion


        #region Private Fields
        private int _accountStatusTypeIdMin;
        private int _accountStatusTypeIdMax;
        #endregion




        #region Ctors
        public UserWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            List<int> accountStatusTypes = DatabaseUtility.GetAccountStatusTypeIds(connection);
            _accountStatusTypeIdMin = accountStatusTypes.Min();
            _accountStatusTypeIdMax = accountStatusTypes.Max();
        }
        #endregion


        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="parentId">Should be left empty</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long parentId = 0)
        {
            string usernameTemplate;

            // Create new ID
            try
            {
                checked { MaxId++; };
            }
            catch(OverflowException)
            {
                return null;
            }


            // Set the template for the values to be inserted (to avoid duplicate fields)
            if (MaxId < 99999)
                usernameTemplate = $"{tableTemplate}_{MaxId.ToString("d5")}";
            else if (MaxId < 999999999)
                usernameTemplate = $"{tableTemplate}_2_{(MaxId - 9999).ToString("d5")}";
            else
                usernameTemplate = $"{tableTemplate}_3_{(MaxId - 99999999).ToString("d5")}";


            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {
                    case "Email":

                        col.Value = $@"{usernameTemplate}@email.com";
                        break;

                    case "Username":

                        col.Value = $@"{usernameTemplate}";
                        break;

                    case "Password":

                        col.Value = $@"{usernameTemplate}.myPwd";
                        break;

                    case "Salt":

                        col.Value = $@"{usernameTemplate}.mySalt";
                        break;

                    case "AccountStatusTypeId":

                        col.Value = RandomFieldGenerator.RandomInt(_accountStatusTypeIdMin, _accountStatusTypeIdMax + 1);
                        break;

                    default:

                        col.Value = RandomFieldGenerator.GenerateRandomField(col);
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
