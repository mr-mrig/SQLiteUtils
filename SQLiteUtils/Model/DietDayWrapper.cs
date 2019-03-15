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
        private const string DefaultTableName = "ActivityDay";
        #endregion







        #region Ctors
        public DietDayWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {

        }
        #endregion


        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="parentId">The ID of the FitnessDayEntry table which this table refers to</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> GenerateRandomEntry(long parentId)
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

                        col.Value = parentId.ToString();
                        break;

                    case "CarbGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 900).ToString();
                        break;

                    case "FatGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 300).ToString();
                        break;

                    case "ProteinGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 500).ToString();
                        break;

                    case "SodiumMg":

                        col.Value = RandomFieldGenerator.RandomIntNullAllowed(0, 800, 0.9f);
                        break;

                    case "WaterLiters":

                        col.Value = RandomFieldGenerator.RandomIntNullAllowed(0, 5, 0.7f);
                        break;

                    case "IsFreeMeal":

                        col.Value = RandomFieldGenerator.RandomInt(0, 1).ToString();
                        break;

                    default:

                        if (col.ValType == null)
                            return null;
                        else
                            col.Value = RandomFieldGenerator.GenerateRandomField(col.Affinity);

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

                        col.Value = RandomFieldGenerator.RandomInt(0, 900).ToString();
                        break;

                    case "FatGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 300).ToString();
                        break;

                    case "ProteinGrams":

                        col.Value = RandomFieldGenerator.RandomInt(0, 500).ToString();
                        break;

                    case "SodiumMg":

                        col.Value = RandomFieldGenerator.RandomIntNullAllowed(0, 800, 0.9f);
                        break;

                    case "WaterLiters":

                        col.Value = RandomFieldGenerator.RandomIntNullAllowed(0, 5, 0.7f);
                        break;

                    case "IsFreeMeal":

                        col.Value = RandomFieldGenerator.RandomInt(0, 1).ToString();
                        break;

                    default:

                        if (col.ValType == null)
                            return null;
                        else
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
