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
using System.Reflection;
using SQLiteUtils.Util;
using SQLiteUtils.Model.Wrappers;

namespace SQLiteUtils.Model
{



    public class DbWrapper : INotifyPropertyChanged, IDisposable
    {


        #region Static

        public static uint YearlyAverageRowsPerUser = 10000;
        #endregion


        #region Enums
        #endregion


        #region Private Fields

        private bool _isDisposed = false;
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

        private long _totalRows = 0;
        /// <summary>
        /// Total number of rows to be processed in the current execution.
        /// </summary>
        public long TotalRows
        {
            get => _totalRows;
            set
            {
                if (_totalRows != value)
                {
                    _totalRows = value;
                    RaisePropertyChanged();
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
        /// Average user activity level [0, 1], where 1 means that all the users inserted will be considered as "Very Active".
        /// <seealso cref="DbWrapperUserProfile"/>
        /// </summary>
        public float UserActivityLevel { get; set; } = 0.5f;

        /// <summary>
        /// Database writer manager
        /// </summary>
        public IDbWriter DbWriter { get; set; }

        /// <summary>
        /// An already opened SQLite connection
        /// </summary>
        public SQLiteConnection SqlConnection { get; set; }

        #endregion



        #region Table Wrappers Properties
        public UserWrapper User { get; set; }

        public UserRelationWrapper UserRelation{ get; set; }

        public PostWrapper Post { get; set; }

        public UserPhaseWrapper UserPhase { get; set; }

        public FitnessDayWrapper FitnessDay { get; set; }

        public WeightWrapper Weight { get; set; }

        public ActivityDayWrapper ActivityDay{ get; set; }

        public DietDayWrapper DietDay { get; set; }

        public WellnessDayWrapper WellnessDay { get; set; }

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

        public LinkedWUWrapper LinkedWorkUnit { get; set; }

        public LinkedWUTemplateWrapper LinkedWUTemplate { get; set; }

        public WorkingSetWrapper WorkingSet { get; set; }

        public WorkingSetIntensityTechniqueWrapper WorkingSetIntTech { get; set; }

        public DatabaseObjectWrapper PlanMessage { get; set; }

        public DatabaseObjectWrapper PlanNote { get; set; }

        public DatabaseObjectWrapper WUTemplateNote { get; set; }

        #endregion



        #region Ctors
        public DbWrapper(IDbWriter dbWriter)
        {
            DbWriter = dbWriter;

            SqlConnection = DbWriter.SqlConnection;

            #region Table Wrappers Initialization
            User = new UserWrapper(SqlConnection);
            Post = new PostWrapper(SqlConnection);
            UserRelation = new UserRelationWrapper(SqlConnection);

            // Fitness Day
            FitnessDay = new FitnessDayWrapper(SqlConnection);
            Weight = new WeightWrapper(SqlConnection);
            WellnessDay = new WellnessDayWrapper(SqlConnection);
            ActivityDay = new ActivityDayWrapper(SqlConnection);
            DietDay = new DietDayWrapper(SqlConnection);

            // Phase
            UserPhase = new UserPhaseWrapper(SqlConnection);

            // Measures
            Measure = new MeasureWrapper(SqlConnection);
            Plicometry = new PlicometryWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);             // TODO
            Circumference = new CircumferenceWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);       // TODO
            Bia = new BiaWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);                           // TODO
            DietPlan = new DietPlanWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);                 // TODO
            DietPlanUnit = new DietPlanUnitWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId);         // TODO
            DietPlanDay = new DietPlanDayWrapper(SqlConnection, GymAppSQLiteConfig.ReservedUserIds + 1, (int)User.MaxId, 1, DietDayTypeStatic.GetMaxId());

            // Training Plan
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
            LinkedWUTemplate = new LinkedWUTemplateWrapper(SqlConnection);
            SetTemplate = new SetTemplateWrapper(SqlConnection);
            SetTemplateIntTech = new SetTemplateIntensityTechniqueWrapper(SqlConnection);

            // Training Schedule
            Schedule = new TrainingScheduleWrapper(SqlConnection);
            Week = new TrainingWeekWrapper(SqlConnection);
            Workout = new WorkoutSessionWrapper(SqlConnection);
            WorkUnit = new WorkUnitWrapper(SqlConnection);
            LinkedWorkUnit = new LinkedWUWrapper(SqlConnection);
            WorkingSet = new WorkingSetWrapper(SqlConnection);
            WorkingSetIntTech = new WorkingSetIntensityTechniqueWrapper(SqlConnection);
            #endregion

            // Set the tables to be processed
            DbWriter.TableWrappers = GetTableList();
        }
        #endregion


        #region IDisposable Pattern

        ~DbWrapper()
        {
            Dispose();
        }
        

        public virtual void Dispose()
        {
            if(!_isDisposed)
            {
                DbWriter.Dispose();
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion


        #region Public Methods


        /// <summary>
        /// Populates all the tables storing notes or messages, which are made up of two columns: Id, Body
        /// </summary>
        /// <param name="rowNum">Number of rows to be inserted</param>
        public void PopulateNotesTables(long rowNum)
        {
            // Tables to be processed
            List<DatabaseObjectWrapper> tableWrappers = DatabaseUtility.GetNotesTables(GetTableList());
            DbWriter.TableWrappers = tableWrappers;

            TotalRows = rowNum * tableWrappers.Count;

            DbWriter.ProcessTransaction("Message", ProcessNotesTables, rowNum);
        }


        /// <summary>
        /// Generates and populates all the tables for a specific number of users.
        /// </summary>
        /// <param name="startDate">Data will be inserted starting from this date</param>
        /// <param name="endDate">Data will be inserted until this date</param>
        /// <param name="usersNumber">Number of users to be processed</param>
        public void InsertUsers(DateTime startDate, DateTime endDate, ushort usersNumber)
        {
            // Start operation
            DbWriter.StartTransaction();

            for (ushort i = 0; i < usersNumber; i++)
            {
                DbWrapperUserProfile userProfile = new DbWrapperUserProfile(UserActivityLevel);

                InsertUser(startDate, endDate, userProfile);
            }

            // End operation
            DbWriter.EndTransaction();
        }


        /// <summary>
        /// Generates and populates all the tables for the specific user.
        /// Warning: StartTransaction and EndTransaction must be called before / after the process.
        /// </summary>
        /// <param name="startDate">Data will be inserted starting from this date</param>
        /// <param name="endDate">Data will be inserted until this date</param>
        /// <param name="userProfile">UserProfile for the user to be processed</param>
        /// <param name="userId">The User ID to be inserted (a new user will be inserted otherwise)</param>
        public void InsertUser(DateTime startDate, DateTime endDate, DbWrapperUserProfile userProfile, long userId = 0)
        {
            if (userId == 0)
            {
                User.Create();
                DbWriter.Write(User);
                userId = User.MaxId + 1;
            }
            else
                throw new NotImplementedException();

                    
            // Process each day separately
            for (DateTime date = startDate.Date; date < endDate.Date; date = date.AddDays(1))
            {
                // Normal Posts
                for(int iPost = 0; iPost < userProfile.GetTodayPostsNumber(); iPost++ )
                {
                    Post.CreatedOnDate = date.AddSeconds(iPost);        // Ensure no collision
                    Post.Create();
                    DbWriter.Write(Post);
                }

                userProfile.BuildUserWeight();

                InsertFitnessDay(date, userProfile.Weight, userProfile.IsTrackingDiet(), userProfile.IsTrackingWeight(), 
                    userProfile.IsTrackingActivity(), userProfile.IsTrackingWellness());

                if (userProfile.IsUserPhaseExpired(date))
                    InsertUserPhase(date, date.AddDays(userProfile.PhasePeriod * DbWrapperUserProfile.DaysPerPeriod));

                if (userProfile.IsMeasureTime(date))
                    InsertMeasures(date);

                if (userProfile.IsDietPlanExpired(date))
                    InsertDietPlan(date, date.AddDays(userProfile.DietPeriod * DbWrapperUserProfile.DaysPerPeriod));

                //DbWrapperTrainingProfile training = userProfile.Training;

                if (userProfile.IsTrainingPlanExpired(date))
                {
                    userProfile.Training = new DbWrapperTrainingProfile(date, WorkUnit.ExcerciseMaxId);
                    InsertTrainingPlan(date, date.AddDays(userProfile.Training.TrainingPlanPeriod * DbWrapperUserProfile.DaysPerPeriod), userProfile.Training);
                }
            }
        }


        /// <summary>
        /// For each table specified creates a SQL script file which stores the insert statements.
        /// This algorithm creates and uses temporary files to optimize memory consumption.
        /// This is the fastest method but does not implement any logic and thus can violate some table constraints.
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

            TotalRows = rowNum * dbTableWrappers.Count;

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

                    //w.WriteLine($@";{Environment.NewLine} INSERT INTO {table.Value.TableName} ({string.Join(", ", table.Value.Entry.Select(x => x.Name))}) VALUES");
                    w.WriteLine(GetInsertStatement(table.Value));
                }


                // Process the rows
                for (long iRow = 0; iRow < rowNum; iRow++)
                {

                    // Insert one row for each table
                    foreach (KeyValuePair<int, DatabaseObjectWrapper> table in dbTableWrappers.Select((x, i) => new KeyValuePair<int, DatabaseObjectWrapper>(i, x)))
                    {
                        StreamWriter w = tempFileWriters.Where((x, i) => i == table.Key).First();

                        // Generate the random fields and write them on the file
                        table.Value.Create();

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

            // Combine the temporary files into the final SQL script file
            TempFilesToDestinationScript(scriptFilePath, tmpFileNames);

            CurrentRow = dbTableWrappers.Sum(x => x.GeneratedEntryNumber);
        }

        #endregion


        #region Private Methods

        private long ProcessNotesTables(long rowNum)
        {
            // Start operation
            DbWriter.StartTransaction();

            for (long i = 0; i < rowNum; i++)
            {
                foreach (DatabaseObjectWrapper tableWrapper in DbWriter.TableWrappers)
                {
                    tableWrapper.Create();
                    DbWriter.Write(tableWrapper);
                    CurrentRow++;
                }
            }

            // End operation
            DbWriter.EndTransaction();

            // DbWriter needs an Action returning the number of processed rows
            return rowNum * DbWriter.TableWrappers.Count;
        }

        private void InsertFitnessDay(DateTime date, ushort weight, bool trackDiet, bool trackWeight, bool trackActivity, bool trackWellness)
        {
            if(trackDiet || trackWeight || trackActivity || trackWellness)
            {
                Post.CreatedOnDate = date;
                Post.Create();
                DbWriter.Write(Post);

                // PostId is the FK of all the child entries
                long parentId = Post.MaxId;

                FitnessDay.FitnessDayDate = date;
                FitnessDay.Create(parentId);
                DbWriter.Write(FitnessDay);

                if (trackDiet)
                {
                    DietDay.Create(parentId);
                    DbWriter.Write(DietDay);
                }

                if (trackWeight)
                {
                    Weight.Kg = weight;
                    Weight.Create(parentId);
                    DbWriter.Write(Weight);
                }

                if (trackWellness)
                {
                    WellnessDay.Create(parentId);
                    DbWriter.Write(WellnessDay);
                }

                if (trackActivity)
                {
                    ActivityDay.Create(parentId);
                    DbWriter.Write(ActivityDay);
                }
            }
        }


        /// <summary>
        /// Insert one row in MEasuresEntry table and populate the childs accordingly
        /// </summary>
        /// <param name="date">Measure date</param>
        private void InsertMeasures(DateTime date)
        {
            Post.CreatedOnDate = date;
            Post.Create();
            DbWriter.Write(Post);

            // PostId is the FK of all the child entries
            long parentId = Post.MaxId;

            Measure.MeasureDate = date;
            Measure.Create(parentId);
            DbWriter.Write(Measure);

            Bia.Create(parentId);
            DbWriter.Write(Bia);

            Circumference.Create(parentId);
            DbWriter.Write(Circumference);

            Plicometry.Create(parentId);
            DbWriter.Write(Plicometry);
        }


        /// <summary>
        /// Insert one row in UserPhase table
        /// </summary>
        /// <param name="startDate">UserPhase start date</param>
        /// <param name="endDate">UserPhase end date</param>
        private void InsertUserPhase(DateTime startDate, DateTime endDate)
        {
            Post.CreatedOnDate = startDate;
            Post.Create();
            DbWriter.Write(Post);

            UserPhase.StartDate = startDate;
            UserPhase.EndDate = endDate;
            UserPhase.Create(Post.MaxId);
            DbWriter.Write(UserPhase);
        }


        /// <summary>
        /// Insert one row in DietPlan table and populate the childs accordingly
        /// </summary>
        /// <param name="startDate">DietPlan start date</param>
        /// <param name="endDate">DietPlan end date</param>
        /// <param name="dietUnitsNum">Number of DietPlanUnit which the Plan is split into</param>
        private void InsertDietPlan(DateTime startDate, DateTime endDate, int dietUnitsNum = 0)
        {
            Post.CreatedOnDate = startDate;
            Post.Create();
            DbWriter.Write(Post);

            // PostId is the FK of the diet plan
            long parentId = Post.MaxId;


            DietPlan.CreatedOnDate = startDate;
            DietPlan.OwnerId = (int)User.MaxId;
            DietPlan.Create(parentId);
            DbWriter.Write(DietPlan);

            // Plan total duration as random number [weeks]
            //int planLength = RandomFieldGenerator.RandomInt(4, 11);
            int planWeeks = (int)(endDate.Subtract(startDate).TotalDays / 7);

            // Number of units which the plan is split into
            if(dietUnitsNum == 0)
                dietUnitsNum = RandomFieldGenerator.RandomInt(1, planWeeks + 1);

            int unitWeeks = planWeeks / dietUnitsNum;     // Truncate, check for reminder later

            // Generate child tables
            for (int iUnit = 0; iUnit < dietUnitsNum; iUnit++)
            {
                // Start / End date according to the length
                DietPlanUnit.StartDate = startDate.AddDays(iUnit * unitWeeks * DbWrapperUserProfile.DaysPerPeriod);
                DietPlanUnit.EndDate = startDate.AddDays((iUnit + 1) * unitWeeks * DbWrapperUserProfile.DaysPerPeriod);

                // Saturate if reminder
                if (iUnit == dietUnitsNum - 1 && DietPlanUnit.EndDate < endDate)
                    DietPlanUnit.EndDate = endDate;

                DietPlanUnit.Create(DietPlan.MaxId);
                DbWriter.Write(DietPlanUnit);

                int dietDays = RandomFieldGenerator.RandomInt(1, 8);

                // Link Diet Days for each Unit
                for (int iDay = 0; iDay < dietDays; iDay++)
                {
                    DietPlanDay.Create(DietPlanUnit.MaxId);
                    DbWriter.Write(DietPlanDay);
                }
            }
        }


        /// <summary>
        /// Insert one row in DietPlan table and populate the childs accordingly
        /// </summary>
        /// <param name="startDate">DietPlan start date</param>
        /// <param name="endDate">DietPlan end date</param>
        /// <param name="dietUnitsNum">Number of DietPlanUnit which the Plan is split into</param>
        private void InsertTrainingPlan(DateTime startDate, DateTime endDate, DbWrapperTrainingProfile trainingProfile, int dietUnitsNum = 0)
        {

            Plan.CreatedOnDate = startDate;
            Plan.Create(User.MaxId);
            DbWriter.Write(Plan);

            if (trainingProfile.RelationType != TrainingPlanRelationWrapper.RelationType.None)
            {
                // Prevent infinite loop
                if(Plan.MaxId > 2)
                {
                    PlanRelation.RelationTypeId = trainingProfile.RelationType;
                    PlanRelation.Create(Plan.MaxId);
                    DbWriter.Write(PlanRelation);
                }
            }

            Schedule.StartDate = startDate;
            Schedule.EndDate = endDate;
            Schedule.Create(Plan.MaxId);
            DbWriter.Write(Schedule);

            // Either one week for the whole plane, or one PlanWeek per week
            //byte planWeeks = (byte)(RandomFieldGenerator.ChooseAmong(new List<int?>() { 1, (int?)endDate.Subtract(startDate).TotalDays / 7 }).Value);


            // Process the weeks
            for (byte iWeek = 0; iWeek < trainingProfile.WeeksNum; iWeek++)
            {
                WeekTemplate.OrderNumber = iWeek;
                WeekTemplate.WeekTypeId = trainingProfile.GetWeekType(iWeek);
                WeekTemplate.Create(Plan.MaxId);
                DbWriter.Write(WeekTemplate);

                Week.OrderNumber = iWeek;
                Week.Create(Schedule.MaxId);
                DbWriter.Write(Week);

                // Process the Workouts
                foreach (DbWrapperWorkoutProfile wo in trainingProfile.Workouts[iWeek])
                {
                    WorkoutTemplate.OrderNumber = wo.OrderNumber;
                    WorkoutTemplate.Create(WeekTemplate.MaxId);
                    DbWriter.Write(WorkoutTemplate);

                    Workout.StartTime = wo.StartTime;
                    Workout.Create(Week.MaxId);
                    DbWriter.Write(Workout);

                    // Process the Work Units
                    foreach (DbWrapperWorkUnitProfile wUnit in wo.WorkUnits)
                    {
                        WorkUnitTemplate.OrderNumber = wUnit.OrderNumber;
                        WorkUnitTemplate.Create(WorkoutTemplate.MaxId);
                        DbWriter.Write(WorkUnitTemplate);

                        WorkUnit.OrderNumber = wUnit.OrderNumber;
                        WorkUnit.Create(Workout.MaxId);
                        DbWriter.Write(WorkUnit);

                        foreach (KeyValuePair<byte, ushort> set in wUnit.WorkingSets)
                        {
                            SetTemplate.OrderNumber = set.Key;
                            SetTemplate.EffortType = wUnit.EffortType;
                            SetTemplate.Effort = wUnit.EffortValue.Value;
                            SetTemplate.Repetitions = set.Value;
                            SetTemplate.Create(WorkUnitTemplate.MaxId);
                            DbWriter.Write(SetTemplate);

                            WorkingSet.OrderNumber = set.Key;
                            WorkingSet.EffortType = wUnit.EffortType;
                            WorkingSet.Effort = wUnit.EffortValue.Value;
                            WorkingSet.TargetReps = set.Value;
                            WorkingSet.Create(WorkUnit.MaxId);
                            DbWriter.Write(WorkingSet);
                        }
                    }
                }
            }
        }


        private string GetInsertStatement(DatabaseObjectWrapper table)
        {
            return $@";{Environment.NewLine} INSERT INTO {table.TableName} ({string.Join(", ", table.Entry.Select(x => x.Name))}) VALUES";
        }



        /// <summary>
        /// Combine the temporary files into the destination file. 
        /// </summary>
        /// <param name="destinationPath">Path of the file to be created</param>
        /// <param name="tmpFilePaths">Path of the temporary files to be combined</param>
        private void TempFilesToDestinationScript(string destinationPath, List<string> tmpFilePaths, bool deleteTempFiles = true)
        {
            try
            {
                StreamWriter destWriter = new StreamWriter(File.Open(destinationPath, FileMode.Create, FileAccess.Write));

                foreach (string tmpFilename in tmpFilePaths)
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
                    // Delete temp files if required
                    if (deleteTempFiles)
                        tmpFilePaths.ForEach(x => File.Delete(x));
                }
                catch (Exception)
                {
                    throw new IOException("Error while merging the temporary files into the destination file.");
                }
            }
        }

        /// <summary>
        ///  Get the list of the tables to be processed according to the Class Properties
        /// </summary>
        /// <returns>The tables list as object wrapper</returns>
        private List<DatabaseObjectWrapper> GetTableList()
        {
            List<DatabaseObjectWrapper> tables = new List<DatabaseObjectWrapper>();

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (prop.PropertyType.BaseType == typeof(DatabaseObjectWrapper) || prop.PropertyType == typeof(DatabaseObjectWrapper))
                {
                    string tableName = string.Empty;

                    try
                    {
                        tableName = (prop.GetValue(this) as DatabaseObjectWrapper).TableName;
                    }
                    catch
                    {
                        System.Diagnostics.Debugger.Break();
                        return null;
                    }
                    tables.Add(prop.GetValue(this) as DatabaseObjectWrapper);
                }
            }

            return tables;
        }


        private void InsertEntry(DatabaseObjectWrapper entry, int parentId = 0)
        {
            if (parentId == 0)
                entry.Create();
            else
                entry.Create(parentId);

            DbWriter.Write(entry);
        }

        #endregion


    }
}
