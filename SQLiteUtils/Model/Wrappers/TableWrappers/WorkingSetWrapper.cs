using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class WorkingSetWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "WorkingSet";
        #endregion


        #region Private Fields
        private int _workUnitIdMin;
        private int _workUnitIdMax;
        private int _effortIdMin;
        private int _effortIdMax;
        #endregion


        #region Properties
        public int Cadence { get; set; } = 0;
        public int TargetReps { get; set; } = 0;
        public int Rest { get; set; } = 0;
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
        public WorkingSetWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            string tableName = string.Empty;
            List<int> ids;

            tableName = "WorkUnitTemplate";
            ids = DatabaseUtility.GetTableIds(connection, tableName);

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

            try
            {
                tableName = "EffortType";
                ids = DatabaseUtility.GetTableIds(connection, tableName);

                _effortIdMin = ids.Min();
                _effortIdMax = ids.Max();
            }
            catch
            {
                throw new SQLiteException($"{GetType().Name} - Table {tableName} has no rows");
            }

            EffortType = GymAppSQLiteConfig.EffortType.NoValue;
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long workUnitId = 0)
        {

            int tempInt = 0;

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                try
                {

                    switch (col.Name)
                    {


                        case "ProgressiveNumber":

                            col.Value = OrderNumber;
                            break;

                        case "RepetitionsTarget":

                            if (TargetReps != 0)
                                col.Value = TargetReps;

                            else
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

                            try
                            {
                                if (!int.TryParse(col.Value.ToString(), out tempInt))
                                    tempInt = 10;
                            }
                            catch
                            {
                                tempInt = 10;
                            }
                            break;

                        case "Repetitions":

                            if (Repetitions == 0)
                            {
                                float rand = (float)RandomFieldGenerator.RandomDouble(0, 1);

                                if (rand < 0.1f)
                                    col.Value = RandomFieldGenerator.RandomInt(tempInt + 1, (int)Math.Max(tempInt * 1.33f, tempInt) + 1);

                                else if (rand < 0.2f)
                                    col.Value = RandomFieldGenerator.RandomIntNullable((int)Math.Max(tempInt * 0.5f, 1), (int)Math.Max(tempInt * 0.9f, 2));

                                else if (rand < 0.95f)
                                    col.Value = tempInt;
                                else
                                    col.Value = null;
                            }
                            else
                                col.Value = Repetitions;

                            break;

                        case "Rest":

                            if (Rest != 0)
                                col.Value = Rest;
                            else
                                col.Value = RandomFieldGenerator.RandomIntNullable(30, 241, 0.1f);
                            break;


                        case "Cadence":

                            if (Cadence != 0)
                                col.Value = Cadence;
                            else
                                col.Value = RandomFieldGenerator.ChooseText(SetTemplateWrapper.CadenceList, 0.4f);
                            break;


                        case "Effort":

                            if (Effort == 0)
                                col.Value = RandomFieldGenerator.RandomEffortFromType(EffortType, 0.1f);
                            else
                                col.Value = Effort;
                            break;


                        case "KgTarget":

                            col.Value = RandomFieldGenerator.RandomIntNullable(5, 100, 0.2f);
                            tempInt = (int)(col.Value ?? 0);
                            break;


                        case "Kg":

                            if (tempInt == 0)
                                col.Value = RandomFieldGenerator.RandomIntNullable(20, 200, 0.2f);
                            else
                                col.Value = RandomFieldGenerator.RandomIntNullable((int)(tempInt * 0.5f), (int)(tempInt * 1.33f), 0.2f);
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
                catch(Exception exc)
                {
                    System.Diagnostics.Debugger.Break();
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


            Cadence = 0;
            Rest = 0;
            TargetReps = 0;
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
