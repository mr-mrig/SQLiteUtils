using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class LinkedWUTemplateWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "LinkedWorkUnitTemplate";
        #endregion


        #region Private Fields
        private int _wuIdMin;
        private int _wuIdMax;
        private int _intTechIdMin;
        private int _intTechIdMax;
        #endregion


        #region Properties
        public int NextWorkUnitId { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public LinkedWUTemplateWrapper(SQLiteConnection connection) : base(connection, DefaultTableName, false)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "WorkUnitTemplate");
            _wuIdMin = ids.Min();
            _wuIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "IntensityTechnique");
            _intTechIdMin = ids.Min();
            _intTechIdMax = ids.Max();
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long wuId1 = 0)
        {
            int tempId = 0;

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "IntensityTechniqueId":

                        col.Value = RandomFieldGenerator.RandomInt(_intTechIdMin, _intTechIdMax + 1);
                        break;

                    case "SecondWorkUnitId":

                        if (NextWorkUnitId != 0)
                            col.Value = NextWorkUnitId;
                        else
                            col.Value = RandomFieldGenerator.RandomIntValueExcluded(_wuIdMin, _wuIdMax + 1, new List<int>() { tempId });
                        break;

                    case "FirstWorkUnitId":

                        if (wuId1 == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_wuIdMin, _wuIdMax + 1);
                        else
                            col.Value = wuId1;

                        tempId = (int)col.Value;
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

            NextWorkUnitId = 0;

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
