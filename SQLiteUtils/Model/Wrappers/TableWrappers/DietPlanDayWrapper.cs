using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class DietPlanDayWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "DietPlanDay";
        #endregion



        #region Private Fields
        private int _dietDayTypeIdMin;
        private int _dietDayTypeIdMax;
        private int _dietPlanUnitIdMin;
        private int _dietPlanUnitIdMax;
        #endregion


        #region Properties
        #endregion


        #region Ctors
        /// <summary>
        /// Wrapper for the BiaEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        public DietPlanDayWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            // Get Diet Plan Type id
            List<int> ids = DatabaseUtility.GetTableIds(connection, "DietPlanType");
            _dietDayTypeIdMin = ids.Min();
            _dietDayTypeIdMax = ids.Max();

            // Get Diet Plan TUnitpe id
            ids = DatabaseUtility.GetTableIds(connection, "DietPlanUnit");
            _dietPlanUnitIdMin = ids.Min();
            _dietPlanUnitIdMax = ids.Max();
        }


        /// <summary>
        /// Wrapper for the BiaEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId ffrom the User table</param>
        /// <param name="userIdMax">Highest userId ffrom the User table</param>
        public DietPlanDayWrapper(SQLiteConnection connection, int dietPlanUnitIdMin, int dietPlanUnitIdMax, int dietDayTypeIdMin, int dietDayTypeIdMax)
            : base(connection, DefaultTableName)
        {
            _dietPlanUnitIdMin = dietPlanUnitIdMin;
            _dietPlanUnitIdMax = dietPlanUnitIdMax;

            _dietDayTypeIdMin = dietDayTypeIdMin;
            _dietDayTypeIdMax = dietDayTypeIdMax;
        }
        #endregion



        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="dietPlanUnitId">DietPlanUnit Id, toherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long dietPlanUnitId = 0)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "Name":

                        col.Value = RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.RandomInt(5, 15));
                        break;

                    case "CarbGrams":

                        col.Value = RandomFieldGenerator.RandomInt(20, 800);
                        break;

                    case "FatGrams":

                        col.Value = RandomFieldGenerator.RandomInt(10, 200);
                        break;

                    case "ProteinGrams":

                        col.Value = RandomFieldGenerator.RandomInt(40, 350);
                        break;

                    case "SaltGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 20);
                        break;

                    case "WaterLiters":

                        col.Value = RandomFieldGenerator.RandomInt(0, 100);
                        break;

                    case "SpecificWeekDay":

                        col.Value = RandomFieldGenerator.RandomIntNullable(1, 7, 0.7f);
                        break;

                    case "DietDayTypeId":

                            col.Value = RandomFieldGenerator.RandomInt(_dietDayTypeIdMin, _dietDayTypeIdMax + 1);
                        break;

                    case "DietPlanUnitId":

                        if (dietPlanUnitId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_dietPlanUnitIdMin, _dietPlanUnitIdMax + 1);
                        else
                        {
                            col.Value = dietPlanUnitId;
                            dietPlanUnitId = 0;
                        }
                        break;

                    default:

                        col.Value = RandomFieldGenerator.GenerateRandomField(col);
                        break;
                }
            }

            // Create new ID
            try
            {
                checked { MaxId++; };
            }
            catch (OverflowException)
            {
                return null;
            }

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
