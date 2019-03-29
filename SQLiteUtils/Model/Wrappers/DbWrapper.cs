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



        #region Enums
        #endregion


        #region Private Fields
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
        /// Average user activity level [0, 1], where 1 means that all the users inserted will be considered as "Very Active".
        /// <seealso cref="DbWrapperUserProfile"/>
        /// </summary>
        public float UserActivityLevel { get; set; } = 0.5f;

        /// <summary>
        /// Database writer manager
        /// </summary>
        public IDbWriter DbWriter { get; protected set; }

        /// <summary>
        /// Number of processed rows
        /// </summary>
        public long NewRows { get; set; } = 0;

        /// <summary>
        /// An already opened SQLite connection
        /// </summary>
        public SQLiteConnection SqlConnection { get; set; }

        #endregion



        #region Table Wrappers Properties
        public UserWrapper User { get; set; }

        public UserRelationWrapper UserRelation{ get; set; }

        public PostWrapper Post { get; set; }

        UserPhaseWrapper UserPhase { get; set; }

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
            GC.SuppressFinalize(this);
        }
        

        public virtual void Dispose()
        {
            try
            {
                DbWriter.Dispose();
            }
            catch
            {

            }
        }
        #endregion


        #region Public Methods


        public void InsertUsers(DateTime startDate, DateTime endDate, int usersNumber)
        {
            // Start operation
            DbWriter.StartTransaction();

            for (int i = 0; i < usersNumber; i++)
            {
                DbWrapperUserProfile userProfile = new DbWrapperUserProfile(UserActivityLevel);

                InsertUser(startDate, endDate, userProfile);
            }

            // End operation
            DbWriter.EndTransaction();
        }


        public void InsertUser(DateTime startDate, DateTime endDate, DbWrapperUserProfile userProfile, long userId = 0)
        {
            if (userId == 0)
            {
                User.Create();
                userId = User.MaxId + 1;
            }
            else
                throw new NotImplementedException();

                    
            // Process each day separately
            for (DateTime date = startDate.Date; date < endDate.Date; date.AddDays(1))
            {
                // Normal Posts
                for(int iPost = 0; iPost < userProfile.GetTodayPostsNumber(); iPost++ )
                {
                    Post.CreatedOnDate = date.AddSeconds(iPost);        // Ensure no collision
                    Post.Create();
                }

                userProfile.BuildUserWeight();

                InsertFitnessDay(date, userProfile.Weight, userProfile.IsTrackingDiet(), userProfile.IsTrackingWeight(), 
                    userProfile.IsTrackingActivity(), userProfile.IsTrackingWellness());

                if (userProfile.IsUserPhaseExpired(date))
                    InsertUserPhase(date, date.AddDays(userProfile.PhasePeriod * DbWrapperUserProfile.DaysPerPeriod));

                if (userProfile.IsMeasureTime(date))
                    InsertMeasures(date);

                if (userProfile.IsDietPlanExpired(date))
                    InsertDietPlan(date, date.AddDays(userProfile.DietPeriod));

                DbWrapperTrainingProfile training = userProfile.Training;

                //if (training.IsExpired(date))
                //    InsertTrainingPlan(date, date.AddDays(training.TrainingPlanPeriod), training);
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

            NewRows += dbTableWrappers.Sum(x => x.GeneratedEntryNumber);
        }

        #endregion


        #region Private Methods


        private void InsertFitnessDay(DateTime date, ushort weight, bool trackDiet, bool trackWeight, bool trackActivity, bool trackWellness)
        {
            //bool trackDiet = userProfile.IsTrackingDiet();
            //bool trackWeight = userProfile.IsTrackingWeight();
            //bool trackActivity = userProfile.IsTrackingActivity();
            //bool trackWellness = userProfile.IsTrackingWellness();

            if(trackDiet || trackWeight || trackActivity || trackWellness)
            {
                Post.CreatedOnDate = date;
                Post.Create();

                // PostId is the FK of all the child entries
                long parentId = Post.MaxId;

                FitnessDay.FitnessDayDate = date;
                FitnessDay.Create(parentId);

                if (trackDiet)
                    DietDay.Create(parentId);

                if (trackWeight)
                {
                    Weight.Kg = weight;
                    Weight.Create(parentId);
                }
                if (trackWellness)
                    WellnessDay.Create(parentId);

                if (trackActivity)
                    ActivityDay.Create(parentId);
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

            // PostId is the FK of all the child entries
            long parentId = Post.MaxId;

            Measure.MeasureDate = date;
            Measure.Create(parentId);

            Bia.Create(parentId);
            Circumference.Create(parentId);
            Plicometry.Create(parentId);
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

            UserPhase.StartDate = startDate;
            UserPhase.EndDate = endDate;
            UserPhase.Create(Post.MaxId);
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

            // PostId is the FK of the diet plan
            long parentId = Post.MaxId;


            DietPlan.CreatedOnDate = startDate;
            DietPlan.Create(parentId);

            // Plan total duration as random number [weeks]
            //int planLength = RandomFieldGenerator.RandomInt(4, 11);
            int planLength = (int)(endDate.Subtract(startDate).TotalDays / 7);

            // Number of units which the plan is split into
            if(dietUnitsNum == 0)
                dietUnitsNum = RandomFieldGenerator.RandomInt(1, planLength + 1);

            int unitLength = planLength / dietUnitsNum;     // Truncate, check for reminder later

            // Generate child tables
            for (int iUnit = 0; iUnit < dietUnitsNum; iUnit++)
            {
                // Start / End date according to the length
                DietPlanUnit.StartDate = startDate.AddDays(iUnit * unitLength);
                DietPlanUnit.EndDate = startDate.AddDays((iUnit + 1) * unitLength);

                // Saturate if reminder
                if (iUnit == dietUnitsNum - 1 && DietPlanUnit.EndDate < endDate)
                    DietPlanUnit.EndDate = endDate;

                DietPlanUnit.Create(DietPlan.MaxId);

                // Link Diet Days for each Unit
                for(int iDay = 0; iDay < RandomFieldGenerator.RandomInt(1, 8); iDay++)
                    DietPlanDay.Create(DietPlanUnit.MaxId);
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

            if (trainingProfile.RelationType != TrainingPlanRelationWrapper.RelationType.None)
            {
                PlanRelation.RelationTypeId = trainingProfile.RelationType;
                PlanRelation.Create();
            }

            Schedule.StartDate = startDate;
            Schedule.EndDate = endDate;
            Schedule.Create(Plan.MaxId);

            // Either one week for the whole plane, or one PlanWeek per week
            //byte planWeeks = (byte)(RandomFieldGenerator.ChooseAmong(new List<int?>() { 1, (int?)endDate.Subtract(startDate).TotalDays / 7 }).Value);


            // Process the weeks
            for(byte iWeek = 0; iWeek < trainingProfile.WeeksNum; iWeek++)
            {
                WeekTemplate.OrderNumber = iWeek;
                WeekTemplate.WeekTypeId = trainingProfile.GetWeekType(iWeek);
                WeekTemplate.Create(Plan.MaxId);

                Week.OrderNumber = iWeek;
                Week.Create(Schedule.MaxId);

                // Process the Workouts
                foreach (DbWrapperWorkoutProfile wo in trainingProfile.Workouts[iWeek])
                {
                    WorkoutTemplate.OrderNumber = wo.OrderNumber;
                    WorkoutTemplate.Create(WeekTemplate.MaxId);

                    Workout.StartTime = wo.StartTime;
                    Workout.Create(Week.MaxId);

                    // Process the Work Units
                    foreach (DbWrapperWorkUnitProfile wUnit in wo.WorkUnits)
                    {
                        WorkUnitTemplate.OrderNumber = wUnit.OrderNumber;
                        WorkUnitTemplate.Create(WorkoutTemplate.MaxId);

                        WorkUnit.OrderNumber = wUnit.OrderNumber;
                        WorkUnit.Create(Workout.MaxId);

                        foreach(KeyValuePair<byte, ushort> set in wUnit.WorkingSets)
                        {
                            SetTemplate.OrderNumber = set.Key;
                            SetTemplate.EffortType = wUnit.EffortType;
                            SetTemplate.Effort = wUnit.EffortValue.Value;
                            SetTemplate.Repetitions = set.Value;
                            SetTemplate.Create(WorkUnitTemplate.MaxId);
                       
                            WorkingSet.OrderNumber = set.Key;
                            WorkingSet.EffortType = wUnit.EffortType;
                            WorkingSet.Effort = wUnit.EffortValue.Value;
                            WorkingSet.TargetReps = set.Value;
                            WorkingSet.Create(WorkUnit.MaxId);
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
                if (prop.PropertyType.BaseType == typeof(DatabaseObjectWrapper))
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
        #endregion


    }
}
