using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class SetTemplateWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "SetTemplate";
        public static readonly List<string> CadenceList = new List<string>()
        {
            "1030", "3030", "5050", "3130", "1210", "1310", "3330", "X020", "X030", "1020", "1120", "3232", "3230", "5151", "7070", "1050", "1310", "1320"
,         };
        #endregion


        #region Private Fields
        private int _workUnitIdMin;
        private int _workUnitIdMax;
        private int _effortIdMin;
        private int _effortIdMax;
        #endregion


        #region Properties
        public int OrderNumber { get; set; } = 0;
        public int Effort { get; set; } = 0;
        public int Repetitions { get; set; } = 0;
        public GymAppSQLiteConfig.EffortType EffortType { get; set; } = GymAppSQLiteConfig.EffortType.NoValue;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public SetTemplateWrapper(SQLiteConnection connection) : base(connection, DefaultTableName, true)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "WorkUnitTemplate");

            try
            {
                _workUnitIdMin = ids.Min();
                _workUnitIdMax = ids.Max();
            }
            catch
            {
                _workUnitIdMin = 0;
                _workUnitIdMax = 0;
            }


            ids = DatabaseUtility.GetTableIds(connection, "EffortType");
            try
            {
                _effortIdMin = ids.Min();
                _effortIdMax = ids.Max();
            }
            catch
            {
                throw new SQLiteException("SetTemplateWrapper - Table EffortType has no rows");
            }
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long workUnitId = 0)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "ProgressiveNumber":

                        col.Value = OrderNumber;
                        break;

                    case "TargetRepetitions":

                        if (Repetitions == 0)
                        {
                            switch (EffortType)
                            {
                                case GymAppSQLiteConfig.EffortType.RPE:

                                    col.Value = RandomFieldGenerator.RandomIntNullable(6, 11, 0.1f);
                                    break;

                                case GymAppSQLiteConfig.EffortType.Intensity:

                                    if (Effort == 0)
                                        col.Value = RandomFieldGenerator.RandomIntNullable(3, 8, 0.1f);
                                    else
                                        col.Value = RandomFieldGenerator.ValidRepsFromIntensity(Effort, 0.1f);
                                    break;

                                case GymAppSQLiteConfig.EffortType.RM:

                                    if (Effort == 0)
                                        col.Value = RandomFieldGenerator.RandomIntNullable(3, 8, 0.1f);
                                    else
                                        col.Value = RandomFieldGenerator.ValidRepsFromRm(Effort, 0.1f);
                                    break;

                                default:

                                    col.Value = null;
                                    break;
                            }
                        }
                        else
                            col.Value = Repetitions;
                        break;


                    case "Rest":

                        col.Value = RandomFieldGenerator.RandomIntNullable(30, 241, 0.1f);
                        break;


                    case "Cadence":

                        col.Value = RandomFieldGenerator.ChooseText(CadenceList, 0.4f);
                        break;


                    case "Effort":

                        if (Effort == 0)
                            col.Value = RandomFieldGenerator.RandomEffortFromType(EffortType, 0.1f);
                        else
                            col.Value = Effort;
                        break;

                    case "EffortTypeId":

                        if (EffortType != GymAppSQLiteConfig.EffortType.NoValue)
                            col.Value = (int)EffortType;
                        else
                            col.Value = RandomFieldGenerator.RandomIntNullable(_effortIdMin, _effortIdMax + 1, 0.3f);
                        break;

                    case "WorkUnitId":

                        if (workUnitId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_workUnitIdMin, _workUnitIdMax + 1);
                        else
                            col.Value = workUnitId;

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


            OrderNumber = 0;
            Effort = 0;
            EffortType = GymAppSQLiteConfig.EffortType.NoValue;

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
