using SQLiteUtils.Model.Initializator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class DbWrapper : INotifyPropertyChanged
    {



        #region Enums
        public enum EffortType : byte
        {
            Intensity = 0,
            RM,
            RPE,
            NoValue,
        }
        #endregion


        #region INotifyPropertyChanged Implementation
        private long _currentRow = 0;
        /// <summary>
        /// Number of processed rows
        /// </summary>
        public long CurrentRow
        {
            get => _currentRow;
            set
            {
                if (_currentRow != value)
                {
                    RaisePropertyChanged();
                    _currentRow = value;
                }
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        #region Properties
        /// <summary>
        /// Number of processed rows
        /// </summary>
        public long NewRows { get; set; } = 0;

        /// <summary>
        /// An already opened SQLite connection
        /// </summary>
        public SQLiteConnection SqlConnection { get; set; }

        public UserWrapper User { get; set; }

        public UserRelationWrapper UserRelation{ get; set; }

        public PostWrapper Post { get; set; }

        public FitnessDayWrapper FitnessDay { get; set; }

        public WeightWrapper Weight { get; set; }

        public ActivityDayWrapper ActivityDay{ get; set; }

        public DietDayWrapper DietDay { get; set; }

        public WellnessDayWrapper WellneessDay { get; set; }

        public MeasureWrapper Measure { get; set; }

        public PlicometryWrapper Plicometry{ get; set; }

        public CircumferenceWrapper Circumference { get; set; }
        
        public BiaWrapper Bia { get; set; }
        
        public DietPlanWrapper DietPlan { get; set; }
        
        public DietPlanUnitWrapper DietPlanUnit { get; set; }
        
        public DietPlanDayWrapper DietPlanDay { get; set; }

        public TrainingPlanWrapper Plan { get; set; }

        public TrainingPlanRelationWrapper PlanRelation { get; set; }

        public TrainingPhaseWrapper PlanPhase { get; set; }

        public TrainingTargetProficiencyWrapper PlanProficiency { get; set; }

        public WeekTemplateWrapper WeekTemplate { get; set; }

        public WorkoutTemplateWrapper WorkoutTemplate { get; set; }

        public WorkUnitTemplateWrapper WorkUnitTemplate { get; set; }

        public SetTemplateWrapper SetTemplate { get; set; }

        public SetTemplateIntensityTechniqueWrapper SetTemplateIntTech { get; set; }

        public TrainingScheduleWrapper Schedule { get; set; }

        public TrainingWeekWrapper Week { get; set; }

        public WorkoutSessionWrapper Workout { get; set; }

        public WorkUnitWrapper WorkUnit { get; set; }

        public LinkedWUWrapper LinkedWUWrapper { get; set; }

        public LinkedWUTemplateWrapper LinkedWUTemplateWrapper { get; set; }

        public WorkingSetWrapper WorkingSet { get; set; }

        public WorkingSetIntensityTechniqueWrapper WorkingSetIntTech { get; set; }

        public DatabaseObjectWrapper PlanMessage { get; set; }

        public DatabaseObjectWrapper PlanNote { get; set; }

        public DatabaseObjectWrapper WUTemplateNote { get; set; }

        #endregion







        #region Ctors
        public DbWrapper(SQLiteConnection connection)
        {
            SqlConnection = connection;

            User = new UserWrapper(SqlConnection);
            Post = new PostWrapper(SqlConnection);

            // Fitness Day
            FitnessDay = new FitnessDayWrapper(SqlConnection);
            Weight = new WeightWrapper(SqlConnection);
            WellneessDay = new WellnessDayWrapper(SqlConnection);
            ActivityDay = new ActivityDayWrapper(SqlConnection);

            // Measures
            Measure = new MeasureWrapper(SqlConnection);
            Plicometry = new PlicometryWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);             // TODO
            Circumference = new CircumferenceWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);       // TODO
            Bia = new BiaWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);                           // TODO
            DietPlan = new DietPlanWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);                 // TODO
            DietPlanUnit = new DietPlanUnitWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);         // TODO
            DietPlanDay = new DietPlanDayWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId, 1, DietDayTypeStatic.GetMaxId());

            // Training
            PlanNote = new DatabaseObjectWrapper(SqlConnection, "TrainingPlanNote");
            Plan = new TrainingPlanWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);
            PlanMessage = new DatabaseObjectWrapper(SqlConnection, "TrainingPlanMessage");
            PlanRelation = new TrainingPlanRelationWrapper(SqlConnection, 1, (int)Plan.MaxId);
            PlanPhase = new TrainingPhaseWrapper(SqlConnection);
            PlanProficiency = new TrainingTargetProficiencyWrapper(SqlConnection);
            WUTemplateNote = new DatabaseObjectWrapper(SqlConnection, "WorkUnitTemplateNote");
            WeekTemplate = new WeekTemplateWrapper(SqlConnection);
            WorkoutTemplate = new WorkoutTemplateWrapper(SqlConnection);
            WorkUnitTemplate = new WorkUnitTemplateWrapper(SqlConnection);
            SetTemplate = new SetTemplateWrapper(SqlConnection);
            SetTemplateIntTech = new SetTemplateIntensityTechniqueWrapper(SqlConnection);
            Schedule = new TrainingScheduleWrapper(SqlConnection);
            Week = new TrainingWeekWrapper(SqlConnection);
            Workout = new WorkoutSessionWrapper(SqlConnection);
            WorkUnit = new WorkUnitWrapper(SqlConnection);
            LinkedWUTemplateWrapper = new LinkedWUTemplateWrapper(SqlConnection);
            LinkedWUWrapper = new LinkedWUWrapper(SqlConnection);
            WorkingSet = new WorkingSetWrapper(SqlConnection);
            WorkingSetIntTech = new WorkingSetIntensityTechniqueWrapper(SqlConnection);
        }
        #endregion



        #region Public Methods

        public void InsertUserData(int userId, string scriptFilePath)
        {

        }


        /// <summary>
        /// For each table specified creates a SQL script file which stores the insert statements.
        /// This algorithm creates and uses temporary files to optimize memory consumption.
        /// </summary>
        /// <param name="rowNum">Number of rows to be inserted</param>
        /// <param name="scriptFilePath">Path of the script file to be created</param>
        /// <param name="tmpFilePath">Path of the temporary files (automatically deleted before returning)</param>
        /// <param name="dbTableWrappers">Wrappers of the tables to be processed</param>
        public void BasicSqlScriptGenerator(uint rowNum, string scriptFilePath, string tmpFilePath, List<DatabaseObjectWrapper> dbTableWrappers)
        {

            // Create variables for each table to be processed
            List<StringBuilder> sqlStrBuilders = new List<StringBuilder>(
                dbTableWrappers.Select(x => new StringBuilder()));

            List<string> tmpFileNames = new List<string>(
                dbTableWrappers.Select((x, i) => $"{tmpFilePath}{i.ToString()}"));

            // Create a temporay file for each table
            List<StreamWriter> tempFileWriters;

            try
            {
                tempFileWriters = new List<StreamWriter>(
                    dbTableWrappers.Select((x, i) => new StreamWriter(File.OpenWrite(tmpFileNames[i]))));
            }
            catch (Exception)
            {
                throw new IOException("Error while opening the temporary files");
            }

            try
            {
                // Init insert statements
                foreach (KeyValuePair<int, DatabaseObjectWrapper> table in dbTableWrappers.Select((x, i) => new KeyValuePair<int, DatabaseObjectWrapper>(i, x)))
                {
                    StreamWriter w = tempFileWriters.Where((x, i) => i == table.Key).First();

                    w.WriteLine($@";{Environment.NewLine} INSERT INTO {table.Value.TableName} ({string.Join(", ", table.Value.Entry.Select(x => x.Name))}) VALUES");
                }


                // Process the rows
                for (long iRow = 0; iRow < rowNum; iRow++)
                {

                    // Insert one row for each table
                    foreach (KeyValuePair<int, DatabaseObjectWrapper> table in dbTableWrappers.Select((x, i) => new KeyValuePair<int, DatabaseObjectWrapper>(i, x)))
                    {
                        StreamWriter w = tempFileWriters.Where((x, i) => i == table.Key).First();

                        Console.WriteLine(table.Value.TableName);

                        // Generate the random fields and write them on the file
                        table.Value.GenerateRandomEntry();

                        w.Write($@" ( {string.Join(", ", table.Value.ToSqlString())} ), ");

                        CurrentRow++;
                    }
                }

                // Close temporay files
                tempFileWriters.ForEach(x => x.Close());
            }
            catch (Exception exc)
            {
                throw exc;
            }


            // Write the SQL script
            try
            {
                StreamWriter destWriter = new StreamWriter(File.Open(scriptFilePath, FileMode.Create, FileAccess.Write));

                foreach (string tmpFilename in tmpFileNames)
                {
                    using (FileStream fs = File.Open(tmpFilename, FileMode.Open, FileAccess.ReadWrite))
                    {
                        // Each temp file has an exceeding ', ' at the end: remove it
                        fs.SetLength(fs.Length - 2);

                        // Copy to the main file
                        fs.CopyTo(destWriter.BaseStream);
                        destWriter.WriteLine(";");
                        destWriter.WriteLine("");

                        fs.Flush();
                    }
                }
                destWriter.Close();
            }
            catch (IOException)
            {
                throw new IOException("Error while merging the temporary files into the destination file.");
            }
            finally
            {
                try
                {
                    // Delete temp files
                    tmpFileNames.ForEach(x => File.Delete(x));
                }
                catch (Exception)
                {
                    throw new IOException("Error while merging the temporary files into the destination file.");
                }
            }

            NewRows += dbTableWrappers.Sum(x => x.GeneratedEntryNumber);
        }


        /// <summary>
        /// Populate the tables related to the user selected for the period specified. 
        /// This algorithm exploits temporary files which is the best method memory-wise
        /// </summary>
        /// <param name="connection">An opened SQLite connection</param>
        /// <param name="scriptFile">Stream to the file to be generated which will store the bulk inserts</param>
        /// <param name="startDate">Start date of the data to be inserted</param>
        /// <param name="endDate">End date of the data to be inserted</param>
        /// <returns>True if ended correctly, false otherwise</returns>
        //public bool InsertUserData(SQLiteConnection connection, StreamWriter scriptFile, DateTime startDate, DateTime endDate)
        //{
        //    int tempFileCounter = 0;

        //    string PostTableTemplate = "";

        //    // Write temp file instead of using big StringBuilders in order to save memory
        //    StreamWriter tempFileW1;
        //    StreamWriter tempFileW2;
        //    StringBuilder sqlStr1 = new StringBuilder();
        //    StringBuilder sqlStr2 = new StringBuilder();
        //    string fname1 = Path.Combine(GymAppSQLiteConfig.SqlTempFilePath + $"{(tempFileCounter++).ToString()}");
        //    string fname2 = Path.Combine(GymAppSQLiteConfig.SqlTempFilePath + $"{(tempFileCounter++).ToString()}");

        //    try
        //    {
        //        tempFileW1 = new StreamWriter(File.OpenWrite(fname1));
        //        tempFileW2 = new StreamWriter(File.OpenWrite(fname2));
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }

        //    List<string> parentColumns = new List<string>();
        //    List<string> childColumns = new List<string>();
        //    Dictionary<string, TypeAffinity> parentColTypes = new Dictionary<string, TypeAffinity>();
        //    Dictionary<string, TypeAffinity> childColTypes = new Dictionary<string, TypeAffinity>();

        //    try
        //    {
        //        int maxUserId = DatabaseUtility.GetTableMaxId(connection, "User");
        //        int minUserId = 1;
        //        int maxPhaseId = DatabaseUtility.GetTableMaxId(connection, "Phase");
        //        int minPhaseId = 1;
        //        int maxPhaseAnnotationId = DatabaseUtility.GetTableMaxId(connection, "UserPhaseAnnotation");
        //        int minPhaseAnnotationId = 1;

        //        // Get columns definition
        //        (parentColumns, parentColTypes) = DatabaseUtility.GetColumnsDefinition(connection, parentTableName);
        //        (childColumns, childColTypes) = DatabaseUtility.GetColumnsDefinition(connection, childTableName);


        //        SqlLogEntries += $@"Processing {parentTableName.ToUpper()} table [{GetFormattedNumber(rowNum)} rows]" + Environment.NewLine;
        //        SqlLogEntries += $@"Processing {childTableName.ToUpper()} table [{GetFormattedNumber(rowNum)} rows]" + Environment.NewLine;

        //        tempFileW1.WriteLine($@";{Environment.NewLine} INSERT INTO {parentTableName} ({string.Join(",", parentColumns)}) VALUES");
        //        tempFileW2.WriteLine($@";{Environment.NewLine} INSERT INTO {childTableName} ({string.Join(",", childColumns)}) VALUES");


        //        // Prepare values to be inserted
        //        for (long i = firstId; i < firstId + rowNum; i++)
        //        {

        //            // Set the template for the values to be inserted (to avoid duplicate rows)
        //            if (i < 9999)
        //                postTemplate = $"{PostTableTemplate}_{i.ToString("d4")}";
        //            else if (i < 99999999)
        //                postTemplate = $"{PostTableTemplate}_2_{(i - 9999).ToString("d4")}";
        //            else if (i < 999999999999)
        //                postTemplate = $"{PostTableTemplate}_3_{(i - 99999999).ToString("d4")}";
        //            else
        //            {
        //                SqlLogEntries = $"Too many rows already inserted in the {parentTableName} table";
        //                return i;
        //            }

        //            postTemplate = $"{PostTableTemplate}_{i.ToString("d4")}";

        //            tempFileW1.Write($@"(");
        //            tempFileW2.Write($@"(");

        //            // POST
        //            foreach (string colName in parentColumns)
        //            {
        //                switch (colName)
        //                {
        //                    case "Caption":

        //                        colValue = $@"'{postTemplate}{RandomFieldGenerator.RandomTextValue(new Random().Next(10, 250))}'";
        //                        break;

        //                    case "IsPublic":

        //                        colValue = $@"'{RandomFieldGenerator.RandomBoolWithProbability(0.7f)}'";
        //                        break;

        //                    case "CreatedOn":

        //                        colValue = $@"{RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound).ToString()}";
        //                        break;

        //                    case "LastUpdate":

        //                        colValue = $@"{RandomFieldGenerator.RandomDateTimeNullAllowed(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound, 0.6f)}";
        //                        break;

        //                    case "UserId":

        //                        colValue = RandomFieldGenerator.RandomInt(minUserId, maxUserId).ToString();
        //                        break;

        //                    default:

        //                        if (parentColTypes == null)
        //                            SqlFail = $"Table {parentTableName}: not possible to auto-detect columns affinity";
        //                        else
        //                            RandomFieldGenerator.GenerateRandomField(parentColTypes[colName]);
        //                        break;
        //                }

        //                sqlStr1.Append($@"{colValue},");
        //            }

        //            // CHILD
        //            foreach (string colName1 in childColumns)
        //            {
        //                switch (colName1)
        //                {
        //                    case "OwnerNote":

        //                        colValue = $@"'{postTemplate}{RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.Rand.Next(10, 250))}'";
        //                        break;

        //                    case "Rating":

        //                        colValue = $@"{RandomFieldGenerator.RandomInt(0, 5)}";
        //                        break;

        //                    case "Date":

        //                        colValue = $@"{RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound).ToString()}";
        //                        break;


        //                    case "LastUpdate":

        //                        colValue = $@"{RandomFieldGenerator.RandomDateTimeNullAllowed(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound, 0.6f)}";
        //                        break;

        //                    case "CreatedOn":

        //                        colValue = $@"{RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound).ToString()}";
        //                        break;

        //                    case "StartDate":

        //                        startDate = RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound).Value;
        //                        colValue = $@"{startDate.ToString()}";
        //                        break;

        //                    case "EndDate":

        //                        if (startDate > 0)
        //                        {
        //                            colValue = $@"{RandomFieldGenerator.RandomUnixDate(startDate, DatabaseUtility.UnixTimestampOneMonthDelta, DatabaseUtility.UnixTimestampSixMonthsDelta).ToString()}";
        //                            startDate = 0;
        //                        }
        //                        else
        //                            colValue = $@"{RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound).ToString()}";
        //                        break;

        //                    case "OwnerId":

        //                        colValue = RandomFieldGenerator.RandomInt(minUserId, maxUserId).ToString();
        //                        break;

        //                    case "PhaseId":

        //                        colValue = RandomFieldGenerator.RandomInt(minPhaseId, maxPhaseId).ToString();
        //                        break;

        //                    case "UserPhaseAnnotationId":

        //                        colValue = RandomFieldGenerator.RandomIntNullAllowed(minPhaseAnnotationId, maxPhaseAnnotationId, 0.6f);
        //                        break;

        //                    case "Name":

        //                        colValue = $"'{RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.Rand.Next(5, 25))}'";
        //                        break;

        //                    case "WeeklyFreeMealsNumber":

        //                        colValue = RandomFieldGenerator.RandomIntNullAllowed(0, 3, 0.3f);
        //                        break;


        //                    default:

        //                        if (childColTypes == null)
        //                            SqlFail = $"Table {childTableName}: not possible to auto-detect columns affinity";
        //                        else
        //                            RandomFieldGenerator.GenerateRandomField(childColTypes[colName1]);
        //                        break;
        //                }

        //                sqlStr2.Append($@"{colValue},");
        //            }

        //            tempFileW1.Write(sqlStr1.Remove(sqlStr1.Length - 1, 1).Append("),"));
        //            tempFileW2.Write(sqlStr2.Remove(sqlStr2.Length - 1, 1).Append("),"));
        //            sqlStr1.Clear();
        //            sqlStr2.Clear();

        //            ProcessedRowsNumber++;
        //        }

        //        tempFileW1.Close();
        //        tempFileW2.Close();
        //    }
        //    catch (Exception exc)
        //    {
        //        SqlFail = exc.Message;
        //        return -1;
        //    }

        //    // Write the SQL script
        //    try
        //    {
        //        // Need to open the file again to remove last char
        //        using (FileStream fs = File.Open(fname1, FileMode.Open, FileAccess.ReadWrite))
        //        {
        //            fs.SetLength(fs.Length - 1);

        //            // Copy to script file
        //            fs.CopyTo(scriptFile.BaseStream);
        //            fs.Flush();
        //        }

        //        scriptFile.WriteLine(";");

        //        // Need to open the file again to remove last char
        //        using (FileStream fs = File.Open(fname2, FileMode.Open, FileAccess.ReadWrite))
        //        {
        //            fs.SetLength(fs.Length - 1);

        //            fs.CopyTo(scriptFile.BaseStream);
        //            fs.Flush();
        //        }
        //        scriptFile.WriteLine(";");
        //    }
        //    catch (Exception exc)
        //    {
        //        SqlFail = exc.Message;
        //        return -1;
        //    }
        //    finally
        //    {
        //        // Delete temp files
        //        File.Delete(fname1);
        //        File.Delete(fname2);
        //    }

        //    // Update the counter
        //    _insertedRows += (rowNum * 2);

        //    return firstId + rowNum;
        //}
        #endregion

    }
}
