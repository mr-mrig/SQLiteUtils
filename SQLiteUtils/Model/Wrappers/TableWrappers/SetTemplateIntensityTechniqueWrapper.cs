﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class SetTemplateIntensityTechniqueWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "SetTemplateIntensityTechnique";
        #endregion


        #region Private Fields
        private int _setIdMin;
        private int _setIdMax;
        private int _intTechIdMin;
        private int _intTechIdMax;
        #endregion


        #region Properties
        public int LinkedSetTemplate { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public SetTemplateIntensityTechniqueWrapper(SQLiteConnection connection) : base(connection, DefaultTableName, false)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "SetTemplate");
            try
            {
                _setIdMin = ids.Min();
                _setIdMax = ids.Max();
            }
            catch
            {
                _setIdMin = 0;
                _setIdMax = 0;
            }

            ids = DatabaseUtility.GetTableIds(connection, "IntensityTechnique");
            try
            {
                _intTechIdMin = ids.Min();
                _intTechIdMax = ids.Max();
            }
            catch
            {
                throw new SQLiteException("SetTemplateIntensityTechniqueWrapper - Table IntensityTechnique has no rows");
            }
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long setId = 0)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "IntensityTechniqueId":

                        col.Value = RandomFieldGenerator.RandomInt(_intTechIdMin, _intTechIdMax + 1);
                        break;

                    case "LinkedSetTemplateId":

                        if (LinkedSetTemplate != 0)
                            col.Value = LinkedSetTemplate;
                        else
                            col.Value = RandomFieldGenerator.RandomIntNullable(_setIdMin, _setIdMax + 1, 0.3f);
                        break;

                    case "SetTemplateId":

                        if (setId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_setIdMin, _setIdMax + 1);
                        else
                            col.Value = setId;

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


            LinkedSetTemplate = 0;

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
