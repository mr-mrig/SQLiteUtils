using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model
{
    public class DietDayWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "DietDay";
        #endregion



        #region Private Fields
        private int _dietDayTypeIdMin;
        private int _dietDayTypeIdMax;
        #endregion



        #region Ctors
        public DietDayWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            // Get Diet Plan TUnitpe id
            List<int> ids = DatabaseUtility.GetTableIds(connection, "DietDayType");
            _dietDayTypeIdMin = ids.Min();
            _dietDayTypeIdMax = ids.Max();
        }

        public DietDayWrapper(SQLiteConnection connection, int dietDayTypeIdMin, int dietDayTypeIdMax) : base(connection, DefaultTableName)
        {
            _dietDayTypeIdMin = dietDayTypeIdMin;
            _dietDayTypeIdMax = dietDayTypeIdMax;
        }
        #endregion


        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="parentId">The ID of the FitnessDayEntry table which this table refers to</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long parentId)
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
                    case "Id":

                        col.Value = parentId;
                        break;

                    case "CarbGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 900);
                        break;

                    case "FatGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 300);
                        break;

                    case "ProteinGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 500);
                        break;

                    case "SaltGrams":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 800, 0.9f);
                        break;

                    case "WaterLiters":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 5, 0.7f);
                        break;

                    case "IsFreeMeal":

                        col.Value = RandomFieldGenerator.RandomInt(0, 2);
                        break;

                    case "DietDayTypeId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_dietDayTypeIdMin, _dietDayTypeIdMax, 0.4f);
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

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// </summary>
        public List<DatabaseColumnWrapper> GenerateRandomEntry()
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
                    case "Id":

                        throw new NotImplementedException();
                        break;

                    case "CarbGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 900);
                        break;

                    case "FatGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 300);
                        break;

                    case "ProteinGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 500);
                        break;

                    case "SodiumMg":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 800, 0.9f);
                        break;

                    case "WaterLiters":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 5, 0.7f);
                        break;

                    case "IsFreeMeal":

                        col.Value = RandomFieldGenerator.RandomInt(0, 2);
                        break;

                    case "DietDayTypeId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_dietDayTypeIdMin, _dietDayTypeIdMax, 0.4f);
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
