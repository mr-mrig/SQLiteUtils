﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model
{
    public class WeightWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "Weight";
        #endregion


        #region Properties

        /// <summary>
        /// Weight [Kg]. The orginal source value (float) must be converted to make it integer
        /// </summary>
        public ushort Kg { get; set; } = 0;
        #endregion


        #region Ctors
        public WeightWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {

        }
        #endregion


        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="parentId">The ID of the FitnessDayEntry table which this table refers to</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long parentId)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {
                    case "Id":

                        col.Value = parentId;
                        break;

                    case "Kg":

                        if (Kg == 0)
                            col.Value = RandomFieldGenerator.RandomInt(350, 1200);
                        else
                            col.Value = Kg;

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

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="parentId">The ID of the FitnessDayEntry table which this table refers to</param>
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

                    case "Kg":

                        col.Value = RandomFieldGenerator.RandomInt(350, 1200);
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
