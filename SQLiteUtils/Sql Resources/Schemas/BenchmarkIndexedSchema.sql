--
-- File generated with SQLiteStudio v3.2.1 on dom apr 28 19:23:48 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: AccountStatusType
DROP TABLE IF EXISTS AccountStatusType;
CREATE TABLE AccountStatusType (Id INTEGER PRIMARY KEY AUTOINCREMENT, Description TEXT (25) NOT NULL UNIQUE);

-- Table: ActivityDay
DROP TABLE IF EXISTS ActivityDay;
CREATE TABLE ActivityDay (Id INTEGER CONSTRAINT PK_ActivityDay_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_ActivityDay_FitnessDayEntry_Id REFERENCES FitnessDayEntry (Id) ON DELETE CASCADE ON UPDATE CASCADE, Steps INTEGER CONSTRAINT CK_ActivityDay_Steps_Positive CHECK (Steps >= 0), CaloriesOut INTEGER CONSTRAINT CK_ActivityDay_CaloriesOut_Positive CHECK (CaloriesOut >= 0), Stairs INTEGER CONSTRAINT CK_ActivityDay_Stairs_Positive CHECK (Stairs >= 0), SleepMinutes INTEGER CONSTRAINT CK_ActivityDay_SleepMinutes_BetweenValues CHECK (SleepMinutes BETWEEN 0 AND 1440), SleepQuality INTEGER CONSTRAINT CK_ActivityDay_SleepQuality_BetweenValues CHECK (SleepQuality BETWEEN 0 AND 4), HeartRateRest INTEGER CONSTRAINT CK_ActivityDay_HeartRateRest_BetweenValues CHECK (HeartRateRest BETWEEN 0 AND 255), HeartRateMax INTEGER CONSTRAINT CK_ActivityDay_HeartRateMax_BetweenValues CHECK (HeartRateMax BETWEEN 0 AND 255));

-- Table: BiaDevice
DROP TABLE IF EXISTS BiaDevice;
CREATE TABLE BiaDevice (Id INTEGER CONSTRAINT PK_BiaDevice_Id PRIMARY KEY AUTOINCREMENT, Model TEXT CONSTRAINT UQ_BiaDevice_Model UNIQUE CONSTRAINT CK_BiaDevice_Model_NotNull NOT NULL, Name TEXT, BrandId INTEGER CONSTRAINT FK_BiaDevice_BiaDeviceBrand_BrandId REFERENCES BiaDeviceBrand (Id) ON UPDATE CASCADE CONSTRAINT FK_BiaDevice_BrandId_NotNull NOT NULL, DeviceTypeId INTEGER CONSTRAINT FK_BiaDevice_BiaDeviceType_Id REFERENCES BiaDeviceType (Id) ON UPDATE CASCADE CONSTRAINT CK_BiaDevice_BiaDeviceTypeId_NotNull NOT NULL);

-- Table: BiaDeviceBrand
DROP TABLE IF EXISTS BiaDeviceBrand;
CREATE TABLE BiaDeviceBrand (Id INTEGER CONSTRAINT PK_BiaDeviceBrand_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_BiaDeviceBrand_Name UNIQUE CONSTRAINT CK_BiaDeviceBrand_Name_NotNull NOT NULL);

-- Table: BiaDeviceType
DROP TABLE IF EXISTS BiaDeviceType;
CREATE TABLE BiaDeviceType (Id INTEGER CONSTRAINT PK_BiaDeviceType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_BiaDevice_Name UNIQUE ON CONFLICT ROLLBACK CONSTRAINT CK_BiaDevice_Name_NotNull NOT NULL, Description TEXT);

-- Table: BiaEntry
DROP TABLE IF EXISTS BiaEntry;
CREATE TABLE BiaEntry (Id INTEGER CONSTRAINT PK_BiaEntry_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_BiaEntry_MeasuresEntry_Id REFERENCES MeasuresEntry (Id) ON DELETE CASCADE ON UPDATE CASCADE, Kg INTEGER CONSTRAINT CK_BiaEntry_Kg_BetweenValues CHECK (Kg BETWEEN 0 AND 5000), Bf INTEGER CONSTRAINT CK_BiaEntry_Bf_BetweenValues CHECK (Bf BETWEEN 0 AND 1000), Tbw INTEGER CONSTRAINT CK_BiaEntry_Tbw_BetweenValues CHECK (Tbw BETWEEN 0 AND 1000), Ecw INTEGER CONSTRAINT CK_BiaEntry_Ecw_BetweenValues CHECK (Ecw BETWEEN 0 AND 1000), EcMatrix INTEGER CONSTRAINT CK_BiaEntry_EcMatrix_BetweenValues CHECK (EcMatrix BETWEEN 0 AND 1000), Bmr INTEGER CONSTRAINT CK_BiaEntry_Bmr_IsPositive CHECK (Bmr > 0), Hpa INTEGER CONSTRAINT CK_BiaEntry_Hpa_BetweenValues CHECK (Hpa BETWEEN 0 AND 1000), BiaDeviceId INTEGER CONSTRAINT FK_BiaEntry_BiaDevice_Id REFERENCES BiaDevice (Id) ON UPDATE CASCADE, OwnerId INTEGER CONSTRAINT FK_BiaEntry_User_OwnerId REFERENCES User (Id) ON DELETE SET NULL ON UPDATE CASCADE);

-- Table: Circumference
DROP TABLE IF EXISTS Circumference;
CREATE TABLE Circumference (Id INTEGER CONSTRAINT PK_Circumferences_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_Circumferences_MeasuresEntry_Id REFERENCES MeasuresEntry (Id) ON DELETE CASCADE ON UPDATE CASCADE, Neck INTEGER CONSTRAINT CK_Circumference_Neck_IsPositive CHECK (Neck >= 0), Chest INTEGER CONSTRAINT CK_Circumference_Chest_IsPositive CHECK (Chest >= 0), Shoulders INTEGER CONSTRAINT CK_CircumferenceShoulders_IsPositive CHECK (Shoulders >= 0), LeftForearm INTEGER CONSTRAINT CK_Circumference_LeftForearm_IsPositive CHECK (LeftForearm >= 0), RightForearm INTEGER CONSTRAINT CK_Circumference_RightForearm_IsPositive CHECK (RightForearm >= 0), LeftArm INTEGER CONSTRAINT CK_Circumference_LeftArm_IsPositive CHECK (LeftArm >= 0), RightArm INTEGER CONSTRAINT CK_Circumference_RightArm_IsPositive CHECK (RightArm >= 0), Waist INTEGER CONSTRAINT CK_Circumference_Waist_IsPositive CHECK (Waist >= 0), Hips INTEGER CONSTRAINT CK_Circumference_Hips_IsPositive CHECK (Hips >= 0), LeftLeg INTEGER CONSTRAINT CK_Circumference_LeftLeg_IsPositive CHECK (LeftLeg >= 0), RightLeg INTEGER CONSTRAINT CK_Circumference_RightLeg_IsPositive CHECK (RightLeg >= 0), LeftCalf INTEGER CONSTRAINT CK_Circumference_LeftCalf_IsPositive CHECK (LeftCalf >= 0), RightCalf INTEGER CONSTRAINT CK_Circumference_RightLeg_IsPositive CHECK (RightCalf >= 0), OwnerId INTEGER CONSTRAINT FK_Circumferences_User_OwnerId REFERENCES User (Id) ON DELETE SET NULL ON UPDATE CASCADE);

-- Table: Comment
DROP TABLE IF EXISTS Comment;
CREATE TABLE Comment (Id INTEGER CONSTRAINT PK_Comment_Id PRIMARY KEY AUTOINCREMENT, Body TEXT (1000) CONSTRAINT CK_Comment_Body_NotNull NOT NULL, CreatedOn INTEGER CONSTRAINT CK_Comment_CreatedOn_NotNull NOT NULL CONSTRAINT DF_Comment_CreatedOn DEFAULT (strftime('%s', 'now')), LastUpdate CONSTRAINT DF_Comment_LastUpdate DEFAULT (strftime('%s', 'now')), UserId INTEGER CONSTRAINT FK_Comment_User_UserId REFERENCES User (Id) CONSTRAINT CK_Comment_UserId_NotNull NOT NULL CONSTRAINT DF_Comment_UserId_DummyUser DEFAULT (1), PostId BIGINT CONSTRAINT FK_Comment_Post_PostId REFERENCES Post (Id) CONSTRAINT CK_Comment_PostId_NotNull NOT NULL);

-- Table: DietDay
DROP TABLE IF EXISTS DietDay;
CREATE TABLE DietDay (Id INTEGER CONSTRAINT PK_DietDay_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_DietDay_FitnessDayEntry_Id REFERENCES FitnessDayEntry (Id) ON DELETE CASCADE ON UPDATE CASCADE, CarbGrams INTEGER CONSTRAINT CK_DietDay_CarbGrams_Positive CHECK (CarbGrams >= 0), FatGrams INTEGER CONSTRAINT CK_DietDay_FatGrams_Positive CHECK (FatGrams >= 0), ProteinGrams INTEGER CONSTRAINT CK_DietDay_ProteinGrams_Positive CHECK (ProteinGrams >= 0), SaltGrams INTEGER CONSTRAINT CK_DietDay_SodiumMg_Positive CHECK (SaltGrams >= 0), WaterLiters INTEGER CONSTRAINT CK_DietDay_WaterLiters_Positive CHECK (WaterLiters >= 0), IsFreeMeal INTEGER CONSTRAINT DF_DietDay_IsFreeMeal DEFAULT (0) CONSTRAINT CK_DietDay_IsFreeMeal_IsBoleean CHECK (IsFreeMeal BETWEEN 0 AND 1), DietDayTypeId INTEGER CONSTRAINT FK_DietDay_DietDayType_Id REFERENCES DietDayType (Id) ON UPDATE CASCADE);

-- Table: DietDayType
DROP TABLE IF EXISTS DietDayType;
CREATE TABLE DietDayType (Id INTEGER CONSTRAINT PK_DietDayType_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT UQ_DietDayType_Id UNIQUE CONSTRAINT CK_DietDayType_Id_NotNull NOT NULL, Name TEXT CONSTRAINT CK_DietDayType_Name_NotNull NOT NULL, Description TEXT);

-- Table: DietHasHashtag
DROP TABLE IF EXISTS DietHasHashtag;
CREATE TABLE DietHasHashtag (DietPlanId INTEGER CONSTRAINT FK_DietHasHashtag_DietPlan_Id REFERENCES DietPlan (Id) ON DELETE CASCADE ON UPDATE CASCADE, DietHashtagId INTEGER CONSTRAINT FK_DietHasHashtag_DietHashtag_Id REFERENCES DietHashtag (Id) ON DELETE CASCADE ON UPDATE CASCADE, CONSTRAINT PK_DietHasHashtag_DietPlanId_DietHashtagId PRIMARY KEY (DietPlanId, DietHashtagId));

-- Table: DietHashtag
DROP TABLE IF EXISTS DietHashtag;
CREATE TABLE DietHashtag (Id INTEGER CONSTRAINT PK_DietHashtag_Id PRIMARY KEY AUTOINCREMENT, Body TEXT CONSTRAINT CK_DietHashtag_Body_NotNull NOT NULL CONSTRAINT UQ_DietHashtag_Body UNIQUE, IsApproved INTEGER CONSTRAINT CK_DietHashtag_IsApproved_BetweenValues CHECK (IsApproved BETWEEN 0 AND 1), CreatedOn INTEGER CONSTRAINT CK_DietHashtag_CreatedOn_NotNull NOT NULL CONSTRAINT DF_DietHashtag_CreatedOn DEFAULT (strftime('%s', 'now')), OwnerId INTEGER CONSTRAINT FK_DietHashtag_User_OwnerId REFERENCES User (Id) ON DELETE SET NULL ON UPDATE CASCADE CONSTRAINT CK_DietHashtag_OwnerId_NotNull NOT NULL);

-- Table: DietPlan
DROP TABLE IF EXISTS DietPlan;
CREATE TABLE DietPlan (Id INTEGER CONSTRAINT PK_DietPlan_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_DietPlan_Post_Id REFERENCES Post (Id) ON UPDATE CASCADE, Name TEXT CONSTRAINT CK_DietPlan_Name_NotNull NOT NULL CONSTRAINT DF_DietPlan_Name DEFAULT ('DietPlan'), WeeklyFreeMealsNumber INTEGER CONSTRAINT CK_DietPlan_WeeklyFreeMealsNumber_IsWeekNumber CHECK (WeeklyFreeMealsNumber BETWEEN 0 AND 7), CreatedOn INTEGER CONSTRAINT CK_DietPlan_CreatedOn_NotNull NOT NULL CONSTRAINT DF_DietPlan_CreatedOn DEFAULT (strftime('%s', 'now')), LastUpdate INTEGER, OwnerId INTEGER CONSTRAINT FK_DietPlan_User_Id REFERENCES User (Id) ON UPDATE CASCADE);

-- Table: DietPlanDay
DROP TABLE IF EXISTS DietPlanDay;
CREATE TABLE DietPlanDay (Id INTEGER CONSTRAINT PK_DietPlanDay_Id PRIMARY KEY AUTOINCREMENT, Name TEXT, CarbGrams INTEGER CONSTRAINT CK_DietPlanDay_CarbGrams_Positive CHECK (CarbGrams >= 0), FatGrams INTEGER CONSTRAINT CK_DietPlanDay_FatGrams_Positive CHECK (FatGrams >= 0), ProteinGrams INTEGER CONSTRAINT CK_DietPlanDay_ProteinGrams_Positive CHECK (ProteinGrams >= 0), SaltGrams INTEGER CONSTRAINT CK_DietPlanDay_SodiumMg_Positive CHECK (SaltGrams >= 0), WaterLiters INTEGER CONSTRAINT CK_DietPlanDay_WaterLiters_Positive CHECK (WaterLiters >= 0), SpecificWeekDay INTEGER CONSTRAINT CK_DietPlanDay_SpecificWeekDay_BetweenValues CHECK (SpecificWeekDay BETWEEN 1 AND 7), DietPlanUnitId INTEGER CONSTRAINT FK_DietPlanDay_DietPlanUnit_Id REFERENCES DietPlanUnit (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_DietPlanDay_Id_NotNull NOT NULL, DietDayTypeId INTEGER CONSTRAINT FK_DietPlanDay_DietDayType_Id REFERENCES DietDayType (Id) ON UPDATE CASCADE CONSTRAINT CK_DietPlanDay_DietDayTypeId_NotNull NOT NULL);

-- Table: DietPlanUnit
DROP TABLE IF EXISTS DietPlanUnit;
CREATE TABLE DietPlanUnit (Id INTEGER CONSTRAINT PK_DietPlanUnit_Id PRIMARY KEY AUTOINCREMENT, StartDate INTEGER CONSTRAINT CK_DietPlanUnit_StartDate_NotNull NOT NULL, EndDatePlanned INTEGER, EndDate INTEGER, DietPlanId INTEGER CONSTRAINT FK_DietPlanUnit_DietPlan_Id REFERENCES DietPlan (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_DietPlanUnit_DietPlanId_NotNull NOT NULL);

-- Table: EffortType
DROP TABLE IF EXISTS EffortType;
CREATE TABLE EffortType (Id INTEGER CONSTRAINT PK_EffortType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_EffortType_Name UNIQUE CONSTRAINT CK_EffortType_Name_NotNull NOT NULL, Abbreviation TEXT CONSTRAINT UQ_EffortType_Abbreviation UNIQUE CONSTRAINT CK_EffortType_Abbreviation_NotNull NOT NULL, Description TEXT);

-- Table: Excercise
DROP TABLE IF EXISTS Excercise;
CREATE TABLE Excercise (Id INTEGER CONSTRAINT PK_Exercise_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_Exercise_Name UNIQUE CONSTRAINT CK_Exercise_Name_NotNull NOT NULL, Description TEXT, ExecutionGuide TEXT, CriticalPointsDescription TEXT, ImageUrl TEXT, IsApproved INTEGER CONSTRAINT CK_Exercise_IsApproved_IsBoolean CHECK (IsApproved BETWEEN 0 AND 1), CreatedOn INTEGER CONSTRAINT CK_Exercise_CreatedOn_NotNull NOT NULL CONSTRAINT DF_Exercise_CreatedOn DEFAULT (STRFTIME('%s', 'now')), LastUpdate INTEGER, MuscleId INTEGER CONSTRAINT FK_Excercise_Muscle_MuscleId REFERENCES Muscle (Id) ON UPDATE CASCADE CONSTRAINT CK_Exercise_MuscleId_NotNull NOT NULL, TrainingEquipmentId INTEGER CONSTRAINT FK_Exercise_TrainingEquipment_Id REFERENCES TrainingEquipment (Id) ON UPDATE CASCADE CONSTRAINT CK_Exercise_TrainingEquipmentId_NotNull NOT NULL, OwnerId INTEGER CONSTRAINT FK_Exercise_User_OwnerId REFERENCES User (Id) ON UPDATE CASCADE CONSTRAINT CK_Exercise_OwnerId_NotNull NOT NULL, ExcerciseDifficultyId INTEGER CONSTRAINT FK_Exercise_ExerciseDifficulty_Id REFERENCES ExerciseDifficulty (Id) ON UPDATE CASCADE CONSTRAINT CK_Exercise_ExcerciseDifficulty_NotNull NOT NULL, PerformanceTypeId         INTEGER CONSTRAINT FK_Excercise_PerformanceType_Id REFERENCES PerformanceType (Id) ON UPDATE CASCADE);

-- Table: ExcerciseSecondaryTarget
DROP TABLE IF EXISTS ExcerciseSecondaryTarget;
CREATE TABLE ExcerciseSecondaryTarget (ExcerciseId INTEGER CONSTRAINT FK_ExcerciseSecondaryTarget_Exercise_Id REFERENCES Excercise (Id) ON UPDATE CASCADE CONSTRAINT CK_ExcerciseSecondaryTarget_ExerciseId_NotNull NOT NULL, MuscleId INTEGER CONSTRAINT FK_ExcerciseSecondaryTarget_Muscle_Id REFERENCES Muscle (Id) ON UPDATE CASCADE CONSTRAINT CK_ExcerciseSecondaryTarget_MuscleId_NotNull NOT NULL, MuscleWorkTypeId INTEGER CONSTRAINT FK_ExcerciseSecondaryTarget_MuscleWorkType_ID REFERENCES MuscleWorkType (Id) ON UPDATE CASCADE, CONSTRAINT PK_ExcerciseSecondaryTarget_ExcerciseId_MuscleId PRIMARY KEY (ExcerciseId, MuscleId));

-- Table: ExerciseDifficulty
DROP TABLE IF EXISTS ExerciseDifficulty;
CREATE TABLE ExerciseDifficulty (Id INTEGER CONSTRAINT PK_ExerciseDifficulty_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_ExerciseDifficulty_Name UNIQUE CONSTRAINT CK_ExerciseDifficulty_Name_NotNull NOT NULL, Description TEXT);

-- Table: ExerciseFocus
DROP TABLE IF EXISTS ExerciseFocus;
CREATE TABLE ExerciseFocus (
    PerformanceFocusId INTEGER CONSTRAINT FK_ExcerciseFocus_PerformanceFocus_Id REFERENCES PerformanceFocus (Id) ON UPDATE CASCADE
                               CONSTRAINT CK_ExcerciseFocus_PerformanceFocusId_NotNull NOT NULL,
    ExerciseId         INTEGER CONSTRAINT FK_ExcerciseFocus_Excercise_ExcerciseId REFERENCES Excercise (Id) ON DELETE CASCADE
                                                                                                            ON UPDATE CASCADE
                               CONSTRAINT CK_ExcerciseFocus_NotNull NOT NULL,
                               CONSTRAINT PK_ExcerciseFocus_PerformanceFocusId_ExcerciseId PRIMARY KEY (PerformanceFocusId, ExerciseId)
);

-- Table: UserHasProficiency
CREATE TABLE UserHasProficiency (
    UserId        INTEGER CONSTRAINT FK_UserHasProficiency_User_Id REFERENCES User (Id) ON DELETE CASCADE
                                                                                        ON UPDATE CASCADE,
    ProficiencyId INTEGER CONSTRAINT FK_UserHasProficiency_TrainingProficiency_Id REFERENCES User (Id) ON UPDATE CASCADE,
    OwnerId       INTEGER CONSTRAINT FK_UserHasProficiency_User_OwnerId REFERENCES User (Id) ON DELETE CASCADE
                                                                                             ON UPDATE CASCADE,
    StartDate     INTEGER CONSTRAINT CK_UserHasProficiency_StartDate_NotNull NOT NULL
                          CONSTRAINT DF_UserHasProficiency_StartDate DEFAULT (strftime('%s', 'now') ),
    EndDate       INTEGER CONSTRAINT CK_UserHasProficiency_EndDate_BiggerThanStart CHECK (EndDate > StartDate),
    CONSTRAINT PK_UserHasProficiency_UserId_ProficiencyId_OwnerId UNIQUE (
        UserId,
        ProficiencyId,
        OwnerId,
        StartDate
    )
);



-- Table: FitnessDayEntry
DROP TABLE IF EXISTS FitnessDayEntry;
CREATE TABLE FitnessDayEntry (Id INTEGER CONSTRAINT PK_FitnessDayEntry_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_FitnessDayEntry_Post_Id REFERENCES Post (Id) ON UPDATE CASCADE, DayDate INTEGER NOT NULL DEFAULT (strftime('%s', CURRENT_DATE)), Rating INTEGER CHECK (Rating BETWEEN 0 AND 5), FOREIGN KEY (Id) REFERENCES Post (Id) ON UPDATE CASCADE);

-- Table: Hashtag
DROP TABLE IF EXISTS Hashtag;
CREATE TABLE Hashtag (Id INTEGER CONSTRAINT PK_Hashtag_Id PRIMARY KEY AUTOINCREMENT, Body TEXT (100) CONSTRAINT UQ_Hashtag_Body UNIQUE CONSTRAINT CK_Hashtag_Body_NotNull NOT NULL);

-- Table: Image
DROP TABLE IF EXISTS Image;
CREATE TABLE Image (Id INTEGER CONSTRAINT PK_Image_Id PRIMARY KEY AUTOINCREMENT, Url TEXT CONSTRAINT CK_Image_Url_NotNull NOT NULL, CreatedOn INTEGER CONSTRAINT CK_Image_CreatedOn_NotNull NOT NULL CONSTRAINT DF_Image_CreatedOn DEFAULT (STRFTIME('%s', 'now')) CONSTRAINT CK_Image_CreatedOn_Positive CHECK (CreatedOn >= 0), IsProgressPicture INTEGER CONSTRAINT CK_Image_IsProgressPicture_IsBoolean CHECK (IsProgressPicture BETWEEN 0 AND 1) CONSTRAINT CK_Image_IsProgressPicture_NotNull NOT NULL CONSTRAINT DF_Image_IsProgressPicture DEFAULT (0), PostId INTEGER CONSTRAINT FK_Image_Post_PostId REFERENCES Post (Id) ON DELETE CASCADE ON UPDATE CASCADE);

-- Table: IntensityTechnique
DROP TABLE IF EXISTS IntensityTechnique;
CREATE TABLE IntensityTechnique (Id INTEGER CONSTRAINT PK_IntensityTechnique_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_IntensityTechnique_Name UNIQUE CONSTRAINT CK_IntensityTechnique_Name_NotNull NOT NULL, Abbreviation TEXT CONSTRAINT UQ_IntensityTechnique_Abbreviation UNIQUE CONSTRAINT CK_IntensityTechnique_Abbreviation_NotNull NOT NULL, Description TEXT, IsLinkingTechnique INTEGER CONSTRAINT CK_IntensityTechnique_IsLinkingTechnique_NotNull NOT NULL CONSTRAINT DF_IntensityTechnique_IsLinkingTechnique DEFAULT (0) CONSTRAINT CK_IntensityTechnique_IsLinkingTechnique_IsBoolean CHECK (IsLinkingTechnique BETWEEN 0 AND 1), RPE INTEGER, IsApproved INTEGER CONSTRAINT CK_IntensityTechnique_IsApproveed_IsBoolean CHECK (IsApproved BETWEEN 0 AND 1), CreatedOn INTEGER CONSTRAINT CK_IntensityTechnique_CreatedOn_NotNull NOT NULL CONSTRAINT DF_IntensityTechnique_CreatedOn DEFAULT (STRFTIME('%s', 'now')), OwnerId INTEGER CONSTRAINT FK_IntensityTechnique_User_OwnerId REFERENCES User (Id) ON UPDATE CASCADE NOT NULL);

-- Table: UserLike
DROP TABLE IF EXISTS UserLike;
CREATE TABLE UserLike (PostId INTEGER CONSTRAINT FK_UserLike_Post_Id REFERENCES Post (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_UserLike_PostId_NotNull NOT NULL, UserId INTEGER CONSTRAINT FK_UserLike_User_Id REFERENCES User (Id) ON DELETE SET NULL ON UPDATE CASCADE CONSTRAINT CK_UserLike_UserId_NotNull NOT NULL, Value INTEGER, CreatedOn INTEGER CONSTRAINT CK_UserLike_Id_NotNull NOT NULL CONSTRAINT DF_UserLike_CreatedOn DEFAULT (strftime('%s', 'now')), CONSTRAINT PK_UserLike_UserId_PostId PRIMARY KEY (PostId, UserId));

-- Table: LinkedWorkUnit
DROP TABLE IF EXISTS LinkedWorkUnit;
CREATE TABLE LinkedWorkUnit (FirstWorkUnitId INTEGER CONSTRAINT FK_LinkedWorkUnit__WorkUnit_FirstWorkUnitId REFERENCES WorkUnit (Id) ON DELETE CASCADE ON UPDATE RESTRICT CONSTRAINT CK_LinkedWorkUnit_FirstWorkUnitId_NotNull NOT NULL, SecondWorkUnitId INTEGER CONSTRAINT FK_LinkedWorkUnit__WorkUnit_SecondWorkUnitId REFERENCES WorkUnit (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_LinkedWorkUnit_SecondWorkUnitId_FirstWorkUnitId_DifferentFrom CHECK (SecondWorkUnitId <> FirstWorkUnitId) CONSTRAINT CK_LinkedWorkUnit_SecondWorkUnitId_NotNull NOT NULL, IntensityTechniqueId INTEGER CONSTRAINT FK_LinkedWorkUnit_IntensityTechnique_Id REFERENCES IntensityTechnique (Id) ON UPDATE CASCADE CONSTRAINT CK_LinkedWorkUnit_IntensityTechniqueId_NotNull NOT NULL, CONSTRAINT PK_LinkedWorkUnit_FirstWorkUnit_NextWorkUnit PRIMARY KEY (FirstWorkUnitId, SecondWorkUnitId));

-- Table: LinkedWorkUnitTemplate
DROP TABLE IF EXISTS LinkedWorkUnitTemplate;
CREATE TABLE LinkedWorkUnitTemplate (FirstWorkUnitId INTEGER CONSTRAINT FK_LinkedWorkUnitTemplate_WorkUnitTemplate_Id1 REFERENCES WorkUnitTemplate (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_LinkedWorkUnitTemplate_FirstWorkUnitId_NotNull NOT NULL, SecondWorkUnitId INTEGER CONSTRAINT FK_LinkedWorkUnitTemplate_WorkUnitTemplate_SecondWorkUnitId REFERENCES WorkUnitTemplate (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_LinkedWorkUnitTemplate_SecondWorkUnitId_FirstWorkUnitId_Different CHECK (FirstWorkUnitId <> SecondWorkUnitId) CONSTRAINT CK_LinkedWorkUnitTemplate_SecondWorkUnitId_NotNull NOT NULL, IntensityTechniqueId INTEGER CONSTRAINT FK_LinkedWorkUnitTemplate_IntensityTechnique_Id REFERENCES IntensityTechnique (Id) ON UPDATE CASCADE CONSTRAINT CK_LinkedWorkUnitTemplate_IntensityTechniqueId_NotNull NOT NULL, CONSTRAINT PK_LinkedWorkUnitTemplate_WorkUnitId1_WorkUnitId2 PRIMARY KEY (FirstWorkUnitId, SecondWorkUnitId));

-- Table: MeasuresEntry
DROP TABLE IF EXISTS MeasuresEntry;
CREATE TABLE MeasuresEntry (Id INTEGER CONSTRAINT PK_MeasuresEntry_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_MeasuresEntry_Post_Id REFERENCES Post (Id) ON UPDATE CASCADE, MeasureDate INTEGER CONSTRAINT CK_MeasuresEntry_MeasuresDate_NotNull NOT NULL CONSTRAINT DF_MeasuresEntry_MeasuresDate DEFAULT (strftime('%s', CURRENT_DATE)) CONSTRAINT CK_MeasuresEntry_MeasuresDate_Positive CHECK (MeasureDate >= 0), OwnerNote TEXT, Rating INTEGER CONSTRAINT CK_MeasuresEntry_Rating_BetweenValues CHECK (Rating BETWEEN 0 AND 4));

-- Table: Mus
DROP TABLE IF EXISTS Mus;
CREATE TABLE Mus (Id INTEGER CONSTRAINT PK_Mus_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_Mus_Name UNIQUE CONSTRAINT CK_Mus_Name_NotNull NOT NULL, Descprition TEXT);

-- Table: Muscle
DROP TABLE IF EXISTS Muscle;
CREATE TABLE Muscle (Id INTEGER CONSTRAINT PK_Muscle_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_Muscle_Name UNIQUE CONSTRAINT CK_Muscle_Name_NotNull NOT NULL, Abbreviation TEXT CONSTRAINT UQ_Muscle_Abbreviation UNIQUE CONSTRAINT CK_Muscle_Abbreviation_NotNull NOT NULL);

-- Table: MuscleWorkType
DROP TABLE IF EXISTS MuscleWorkType;
CREATE TABLE MuscleWorkType (Id INTEGER CONSTRAINT PK_MuscleWorkType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_MuscleWorkType_Name UNIQUE CONSTRAINT CK_MuscleWorkType_Name_NotNull NOT NULL, Description TEXT, DefaultEffectivness INTEGER CONSTRAINT CK_MuscleWorkType_DefaultEffectivness_BetweenValues CHECK (DefaultEffectivness BETWEEN 0 AND 10) CONSTRAINT CK_MuscleWorkType_DefaultEffectivness_NotNull NOT NULL);

-- Table: PerformanceFocus
DROP TABLE IF EXISTS PerformanceFocus;
CREATE TABLE PerformanceFocus (Id INTEGER CONSTRAINT PK_ExerciseFocus_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_ExerciseFocus_Name UNIQUE CONSTRAINT CK_ExerciseFocus_Name_NotNull NOT NULL, Description TEXT);

-- Table: Phase
DROP TABLE IF EXISTS Phase;
CREATE TABLE Phase (Id INTEGER CONSTRAINT PK_Phase_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_Phase_Name UNIQUE CONSTRAINT CK_Phase_Name_NotNull NOT NULL, CreatedOn INTEGER CONSTRAINT DF_Phase_CreatedOn DEFAULT (strftime('%s', 'now')) CONSTRAINT CK_Phase_CreatedOn_NotNull NOT NULL, IsApproved INTEGER CONSTRAINT CK_Phase_IsApproved_IsBoolean CHECK (IsApproved BETWEEN 0 AND 1), OwnerId INTEGER CONSTRAINT FK_Phase_User_OwnerId REFERENCES User (Id) ON UPDATE CASCADE CONSTRAINT CK_Phase_OwnerId_NotNull NOT NULL);

-- Table: Plicometry
DROP TABLE IF EXISTS Plicometry;
CREATE TABLE Plicometry (Id INTEGER CONSTRAINT FK_Plicometry_MeasuresEntry_Id REFERENCES MeasuresEntry (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT PK_Plicometry_Id PRIMARY KEY AUTOINCREMENT, Chest INTEGER CONSTRAINT CK_Plicometry_Chest_IsPositive CHECK (Chest >= 0), Tricep INTEGER CONSTRAINT CK_Plicometry_Tricep_IsPositive CHECK (Tricep >= 0), Armpit INTEGER CONSTRAINT CK_Plicometry_Armpit_IsPositive CHECK (Armpit >= 0), Subscapular INTEGER CONSTRAINT CK_Plicometry_Subscapular_IsPositive CHECK (Subscapular >= 0), Suprailiac INTEGER CONSTRAINT CK_Plicometry_Suprailiac_IsPositive CHECK (Suprailiac >= 0), Abdomen INTEGER CONSTRAINT CK_Plicometry_Abdomen_IsPositive CHECK (Abdomen >= 0), Thigh INTEGER CONSTRAINT CK_Plicometry_Thigh_IsPositive CHECK (Thigh >= 0), Kg INTEGER CONSTRAINT CK_Plicometry_Kg_BetweenValues CHECK (Kg BETWEEN 0 AND 5000), Bf INTEGER CONSTRAINT CK_Plicometry_Bf_BetweenValues CHECK (Bf BETWEEN 0 AND 1000), OwnerId INTEGER CONSTRAINT FK_Plicometry_User_OwnerId REFERENCES User (Id) ON DELETE SET NULL ON UPDATE CASCADE);

-- Table: Post
DROP TABLE IF EXISTS Post;
CREATE TABLE Post (Id INTEGER PRIMARY KEY AUTOINCREMENT, Caption TEXT (1000), IsPublic BOOLEAN, CreatedOn INTEGER CONSTRAINT CK_Post_CreatedOn_NotNull NOT NULL CONSTRAINT DF_Post_CreatedOn DEFAULT (strftime('%s', 'now')), LastUpdate INTEGER CONSTRAINT DF_Post_LastUpdate DEFAULT (strftime('%s', 'now')), UserId INTEGER CONSTRAINT FK_Post_User_UserId REFERENCES User (Id) ON UPDATE CASCADE CONSTRAINT CK_Post_UserId_NotNull NOT NULL);

-- Table: PostHasHashtag
DROP TABLE IF EXISTS PostHasHashtag;
CREATE TABLE PostHasHashtag (PostId INTEGER CONSTRAINT FK_PostHasHashtag_Post_PostId REFERENCES Post (Id) ON UPDATE CASCADE CONSTRAINT CK_PostHasHashtag_PostId_NotNull NOT NULL, HashtagId INTEGER CONSTRAINT FK_PostHasHashtag_Hashtag_Id REFERENCES Hashtag (Id) ON UPDATE CASCADE CONSTRAINT CK_PostHasHashtag_HashtagId_NotNull CONSTRAINT CK_PostHasHashtag_HashtagId NOT NULL, CONSTRAINT PK_PostHasHashtag_PostId_HashtagId PRIMARY KEY (PostId, HashtagId));

-- Table: RelationStatus
DROP TABLE IF EXISTS RelationStatus;
CREATE TABLE RelationStatus (Id INTEGER CONSTRAINT PK_RelationStatus_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_RelationStatus_Name UNIQUE CONSTRAINT CK_RelationStatus_Name_NotNull NOT NULL, Description TEXT);

-- Table: SetTemplate
DROP TABLE IF EXISTS SetTemplate;
CREATE TABLE SetTemplate (Id INTEGER CONSTRAINT PK_SetTemplate_Id PRIMARY KEY AUTOINCREMENT, ProgressiveNumber INTEGER CONSTRAINT CK_SetTemplate_ProgressiveNumber_NotNull NOT NULL CONSTRAINT CK_SetTemplate_ProgressiveNumber_IsPositive CHECK (ProgressiveNumber >= 0), TargetRepetitions TEXT, Rest INTEGER CONSTRAINT CK_SetTemplate_Rest_Positive CHECK (Rest >= 0), Cadence TEXT, Effort INTEGER CONSTRAINT CK_SetTemplate_Effort_IsPositive CHECK (Effort >= 0), WorkUnitId INTEGER CONSTRAINT FK_SetTemplate_WorkUnit_Id REFERENCES WorkUnitTemplate (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_SetTemplate_WorkUnitId_NotNull NOT NULL, EffortTypeId INTEGER CONSTRAINT FK_SetTemplate_EffortType_Id REFERENCES EffortType (Id) ON UPDATE CASCADE);

-- Table: SetTemplateIntensityTechnique
DROP TABLE IF EXISTS SetTemplateIntensityTechnique;
CREATE TABLE SetTemplateIntensityTechnique (SetTemplateId INTEGER CONSTRAINT FK_SetTemplateIntensityTechnique_SetTemplate_SetTemplateId REFERENCES SetTemplate (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_SetTemplateIntensityTechnique_SetTemplateId_NotNull NOT NULL, IntensityTechniqueId INTEGER CONSTRAINT FK_SetTemplateIntensityTechnique_IntensityTechnique_Id REFERENCES IntensityTechnique (Id) ON UPDATE CASCADE CONSTRAINT CK_SetTemplateIntensityTechnique_IntensityTechniqueId_NotNull NOT NULL, LinkedSetTemplateId INTEGER CONSTRAINT FK_SetTemplateIntensityTechnique_SetTemplate_LinkedSetTemplateId REFERENCES SetTemplate (Id) ON DELETE CASCADE ON UPDATE CASCADE, CONSTRAINT PK_SetTemplateIntensityTechnique_SetTemplateId_IntensityTechniqueId PRIMARY KEY (SetTemplateId, IntensityTechniqueId));

-- Table: TrainingEquipment
DROP TABLE IF EXISTS TrainingEquipment;
CREATE TABLE TrainingEquipment (Id INTEGER CONSTRAINT PK_TrainingEquipment_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT CK_TrainingEquipment_Name_NotNull NOT NULL CONSTRAINT UQ_TrainingEquipment_Name UNIQUE, Description TEXT);

-- Table: TrainingHashtag
DROP TABLE IF EXISTS TrainingHashtag;
CREATE TABLE TrainingHashtag (Id INTEGER CONSTRAINT PK_TrainingHashtag_Id PRIMARY KEY AUTOINCREMENT, Body TEXT CONSTRAINT UQ_TrainingHashtag_Body UNIQUE CONSTRAINT CK_TrainingHashtag_Body_NotNull NOT NULL, IsApproved INTEGER CONSTRAINT CK_TrainingHashtag_IsApproved_IsBoolean CHECK (IsApproved BETWEEN 0 AND 1), CreatedOn INTEGER CONSTRAINT CK_TrainingHashtag_CreatedOn NOT NULL CONSTRAINT DF_TrainingHashtag_CreatedOn DEFAULT (strftime('%s', 'now')), OwnerId INTEGER CONSTRAINT FK_TrainingHashtag_User_OwnerId REFERENCES User (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingHashtag_OwnerId_NotNull NOT NULL);

-- Table: TrainingMuscleFocus
DROP TABLE IF EXISTS TrainingMuscleFocus;
CREATE TABLE TrainingMuscleFocus (TrainingPlanId INTEGER CONSTRAINT FK_TrainingFocus_TrainingPlanId REFERENCES TrainingPlan (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_TrainingFocus_TrainingPlanId_NotNull NOT NULL, MuscleId INTEGER CONSTRAINT FK_TrainingFocus_Muscle_MuscleId REFERENCES Muscle (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingFocus_MuscleId_NotNull NOT NULL, CONSTRAINT PK_TrainingFocus_TrainingPlanId_MuscleId PRIMARY KEY (TrainingPlanId, MuscleId));

-- Table: TrainingPlan
DROP TABLE IF EXISTS TrainingPlan;
CREATE TABLE TrainingPlan (Id INTEGER CONSTRAINT PK_TrainingPlan_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT CK_TrainingPlan_Name_NotNull NOT NULL, Description TEXT, IsBookmarked INTEGER CONSTRAINT CK_TrainingPlan_IsBookmarked_IsBoolean CHECK (IsBookmarked BETWEEN 0 AND 1) CONSTRAINT DF_TrainingPlan_IsBookmarked DEFAULT (0) CONSTRAINT CK_TrainingPlan_IsBookmarked_NotNull NOT NULL, IsTemplate INTEGER CONSTRAINT CK_TrainingPlan_IsTemplate_IsBoolean CHECK (IsTemplate BETWEEN 0 AND 1) CONSTRAINT CK_TrainingPlan_IsTemplate_NotNull NOT NULL CONSTRAINT DF_TrainingPlan_IsTemplate DEFAULT (0), CreatedOn INTEGER CONSTRAINT CK_TrainingPlan_CreatedOn_NotNull NOT NULL CONSTRAINT DF_TrainingPlan_CreatedOn DEFAULT (strftime('%s', CURRENT_DATE)), OwnerId INTEGER CONSTRAINT FK_TrainingPlan_User_OwnerId REFERENCES User (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingPlan_OwnerId_NotNull NOT NULL, TrainingPlanNoteId INTEGER CONSTRAINT FK_TrainingPlan_TrainingPlanNote_Id REFERENCES TrainingPlanNote (Id) ON DELETE SET NULL ON UPDATE CASCADE, CONSTRAINT UQ_TrainingPlan_Name_OwnerId UNIQUE (Name, OWnerId));

-- Table: TrainingPlanHasHashtag
DROP TABLE IF EXISTS TrainingPlanHasHashtag;
CREATE TABLE TrainingPlanHasHashtag (TrainingPlanId INTEGER CONSTRAINT FK_TrainingPlanHasHashtag_TrainingPlan_Id REFERENCES TrainingPlan (Id) ON DELETE CASCADE ON UPDATE CASCADE, TrainingHashtagId INTEGER CONSTRAINT FK_TrainingPlanHasHashtag_TrainingHashtag_Id REFERENCES TrainingHashtag (Id), CONSTRAINT PK_TrainingPlanHasHashtag_TrainingPlanId_TrainingHashtagId PRIMARY KEY (TrainingPlanId, TrainingHashtagId));

-- Table: TrainingPlanHasPhase
DROP TABLE IF EXISTS TrainingPlanHasPhase;
CREATE TABLE TrainingPlanHasPhase (PlanId INTEGER CONSTRAINT FK_TrainingPlanHasPhase_TrainingPlan_PlanId REFERENCES TrainingPlan (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_TrainingPlanHasPhase_PlanId_NotNull NOT NULL, PhaseId INTEGER CONSTRAINT FK_TainingPlanHasPhase_Phase_PhaseId REFERENCES Phase (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingPlanHasPhase_PhaseId_NotNull NOT NULL, CONSTRAINT PK_TrainingPlanHasPhase PRIMARY KEY (PlanId, PhaseId));

-- Table: TrainingPlanMessage
DROP TABLE IF EXISTS TrainingPlanMessage;
CREATE TABLE TrainingPlanMessage (Id INTEGER CONSTRAINT PK_TrainingPlanMessage_Id PRIMARY KEY AUTOINCREMENT, Body TEXT CONSTRAINT CK_TrainingPlanMessage_Body_NotNull NOT NULL);

-- Table: TrainingPlanNote
DROP TABLE IF EXISTS TrainingPlanNote;
CREATE TABLE TrainingPlanNote (Id INTEGER CONSTRAINT PK_TrainingPlanNote_Id PRIMARY KEY AUTOINCREMENT, Body TEXT CONSTRAINT PK_TrainingPlanNote_Body NOT NULL);

-- Table: TrainingPlanRelation
DROP TABLE IF EXISTS TrainingPlanRelation;
CREATE TABLE TrainingPlanRelation (ParentPlanId INTEGER CONSTRAINT FK_TrainingPlanRelation_TrainingPlan_ParentPlanId REFERENCES TrainingPlan (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingPlanRelation_ParentPlanId_NotNull NOT NULL, ChildPlanId INTEGER CONSTRAINT FK_TrainingPlanRelation_TrainingPlan_ChildPlanId REFERENCES TrainingPlan (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingPlanRelation_ChildPlanId_NotNull NOT NULL CONSTRAINT CK_TrainingPlanRelation_ChildPlanId_ParentPlanId_Different CHECK (ChildPlanId <> ParentPlanId), RelationTypeId INTEGER CONSTRAINT FK_TrainingPlanRelation_TrainingPlanRelationType_Id REFERENCES TrainingPlanRelationType (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingPlanRelation_RelationTypeId_NotNull NOT NULL, TrainingPlanMessageId INTEGER CONSTRAINT FK_TrainingPlanRelation_TrainingPlanMessage_Id REFERENCES TrainingPlanMessage (Id) ON UPDATE CASCADE, CONSTRAINT PK_TrainingPlanRealtion_ParentPlanId_ChildPlanId PRIMARY KEY (ParentPlanId, ChildPlanId));

-- Table: TrainingPlanRelationType
DROP TABLE IF EXISTS TrainingPlanRelationType;
CREATE TABLE TrainingPlanRelationType (Id INTEGER CONSTRAINT PK_TrainingPlanRelationType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_TrainingPlanRelationType_Name UNIQUE CONSTRAINT CK_TrainingPlanRelationType_Name_NotNull NOT NULL, Description TEXT CONSTRAINT CK_TrainingPlanRelationType_Description_NotNull NOT NULL);

-- Table: TrainingPlanTarget
DROP TABLE IF EXISTS TrainingPlanTarget;
CREATE TABLE TrainingPlanTarget (TrainingPlanId INTEGER CONSTRAINT FK_TrainingPlanTarget_TrainingPlan_PlanId REFERENCES TrainingPlan (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_TrainingPlanTarget_TrainingPlanId_NotNull NOT NULL, TrainingProficiencyId INTEGER CONSTRAINT FK_TrainingPlanTarget_TrainingProficiency_ REFERENCES TrainingProficiency (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingPlanTarget_TrainingProficiencyId_NotNull NOT NULL, CONSTRAINT PK_TrainingPlanTarget_PlanId_ProficiencyId PRIMARY KEY (TrainingPlanId, TrainingProficiencyId));

-- Table: TrainingProficiency
DROP TABLE IF EXISTS TrainingProficiency;
CREATE TABLE TrainingProficiency (Id INTEGER CONSTRAINT PK_TrainingProficiency_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_TrainingProficiency_Id UNIQUE CONSTRAINT CK_TrainingProficiency_NotNull NOT NULL, Description TEXT, CreatedOn INTEGER CONSTRAINT CK_TrainingProficiency_NotNull NOT NULL CONSTRAINT DF_TrainingProficiency_CreatedOn DEFAULT (strftime('%s', 'now')), IsApproved INTEGER CONSTRAINT DF_TrainingProficiency_IsApproved DEFAULT (0) CONSTRAINT CK_TrainingProficiency_IsApproved_IsBoolean CHECK (IsApproved BETWEEN 0 AND 1), OwnerId INTEGER CONSTRAINT FK_TrainingProficiency_User_OwnerId REFERENCES User (Id) ON DELETE SET NULL ON UPDATE CASCADE CONSTRAINT CK_TrainingProficiency_OwnerId_NotNull NOT NULL);

-- Table: TrainingSchedule
DROP TABLE IF EXISTS TrainingSchedule;
CREATE TABLE TrainingSchedule (Id INTEGER CONSTRAINT PK_TrainingSchedule_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_TrainingSchedule_Post_Id REFERENCES Post (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingSchedule_Id_NotNull NOT NULL, StartDate INTEGER CONSTRAINT CK_TrainingSchedule_StartDate_NotNull NOT NULL, EndDate INTEGER, PlannedEndDate INTEGER, TrainingPlanId INTEGER CONSTRAINT FK_TrainingSchedule_TrainingPlan_Id REFERENCES TrainingPlan (Id) ON DELETE SET NULL ON UPDATE CASCADE CONSTRAINT CK_TrainingSchedule_TrainingPlanId_NotNull NOT NULL, CurrentWeekId INTEGER CONSTRAINT FK_TrainingSchedule_TrainingWeek_CurrentWeekId REFERENCES TainingWeek (Id) ON UPDATE CASCADE, PhaseId INTEGER CONSTRAINT FK_TrainingSchedule_Phase_PhaseId REFERENCES Phase (Id) ON UPDATE CASCADE, TrainingProficiencyId INTEGER CONSTRAINT FK_TrainingSchdule_TrainingProficiency_Id REFERENCES TrainingProficiency (Id) ON UPDATE CASCADE);

-- Table: TrainingScheduleFeedback
DROP TABLE IF EXISTS TrainingScheduleFeedback;
CREATE TABLE TrainingScheduleFeedback (
    Id                 INTEGER CONSTRAINT PK_TrainingScheduleFeedback_Id PRIMARY KEY AUTOINCREMENT,
    Comment            TEXT,
    LastUpdate          INTEGER CONSTRAINT DF_TrainingScheduleFeedback_LastUpdate DEFAULT (STRFTIME('%s', 'now') ) 
                               CONSTRAINT CK_TrainingScheduleFeedback_LastUpdate_NotNull NOT NULL,
    Rating             INTEGER CONSTRAINT CK_TrainingScheduleFeedback_Rating_BetweenValues CHECK (Rating BETWEEN 1 AND 5),
    TrainingScheduleId INTEGER CONSTRAINT FK_TrainingScheduleFeedback_TrainingSchedule_Id REFERENCES TrainingSchedule (Id) ON DELETE CASCADE
                                                                                                                           ON UPDATE CASCADE
                               CONSTRAINT CK_TrainingScheduleFeedback_TrainingScheduleId_NotNull NOT NULL,
    UserId             INTEGER CONSTRAINT FK_TrainingScheduleFeedback_User_UserId REFERENCES User (Id) ON UPDATE CASCADE
                               CONSTRAINT CK_TrainingScheduleFeedback_UserId_NotNull NOT NULL,
                               CONSTRAINT UQ_TrainingScheduleFeedback_UserId_TrainingScheduleId UNIQUE (TrainingScheduleId, UserId)
);

-- Table: PerformanceType
CREATE TABLE PerformanceType (
    Id   INTEGER CONSTRAINT PK_PerformanceType_Id PRIMARY KEY AUTOINCREMENT,
    Name TEXT    CONSTRAINT CK_PerformanceType_Name_NotNull NOT NULL
                 CONSTRAINT UQ_PerformanceType_Name UNIQUE
);


-- Table: PersonalRecord
CREATE TABLE PersonalRecord (
    Id          INTEGER CONSTRAINT PK_PersonalRecord_Id PRIMARY KEY AUTOINCREMENT,
    UserId      INTEGER CONSTRAINT FK_PersonalRecord_User_Id REFERENCES User (Id) ON DELETE CASCADE
                                                                                  ON UPDATE CASCADE
                        CONSTRAINT CK_PersonalRecord_Id_NotNull NOT NULL,
    ExcerciseId INTEGER CONSTRAINT FK_PersonalRecord_Excercise_Id REFERENCES Excercise (Id) ON UPDATE CASCADE
                        CONSTRAINT CK_PersonalRecord_ExcerciseId_NotNull NOT NULL,
    RecordDate  INTEGER CONSTRAINT CK_PersonalRecord_RecordDate_NotNull NOT NULL
                        CONSTRAINT DF_PersonalRecord_RecordDate DEFAULT (strftime('%s', 'now') ),
    Value       INTEGER CONSTRAINT CK_PersonalRecord_Value_NotNull NOT NULL
                        CONSTRAINT CK_PersonalRecord_Value_Positive CHECK (Value > 0),
    CONSTRAINT UQ_PersonalRecord_UserId_ExcerciseId_RecordDate UNIQUE (
        UserId,
        ExcerciseId,
        RecordDate
    )
);


-- Table: TrainingScheduleHashtag
DROP TABLE IF EXISTS TrainingScheduleHashtag;
CREATE TABLE TrainingScheduleHashtag (TrainingScheduleId INTEGER CONSTRAINT FK_TrainingScheduleHashtag_TrainingSchedule_ID REFERENCES TrainingSchedule (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_TrainingScheduleHashtag_TrainingScheduleId_NotNull NOT NULL, TrainingHashtagId INTEGER CONSTRAINT FK_TrainingScheduleHashtag_TrainingHashtag_Id REFERENCES TrainingHashtag (Id) ON UPDATE CASCADE CONSTRAINT CK_TrainingScheduleHashtag_TrainingHashtagId_NotNull NOT NULL, DisplayOrder INTEGER CONSTRAINT CK_TrainingScheduleHashtag_Order_Positive CHECK (DisplayOrder >= 0), CONSTRAINT pK_TrainingScheduleHashtag_TrainingScheduleId_TrainingHashtagId PRIMARY KEY (TrainingScheduleId, TrainingHashtagId));

-- Table: TrainingWeek
DROP TABLE IF EXISTS TrainingWeek;
CREATE TABLE TrainingWeek (Id INTEGER CONSTRAINT PK_TrainingWeek_Id PRIMARY KEY AUTOINCREMENT, ProgressiveNumber INTEGER CONSTRAINT CK_TrainingWeek_ProgressiveNumber_Positive CHECK (ProgressiveNumber >= 0) CONSTRAINT CK_TrainingWeek_ProgressiveNumber_NotNull NOT NULL, TrainingScheduleId INTEGER CONSTRAINT FK_TrainingWeek_TrainingScheduleId REFERENCES TrainingSchedule (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_TrainingWeek_TrainingScheduleId_NotNull NOT NULL, TrainingWeekTypeId INTEGER CONSTRAINT FK_TrainingWeek_TrainingWeekType_Id REFERENCES TrainingWeekType (Id) ON UPDATE CASCADE, NextWorkoutId INTEGER CONSTRAINT FK_TrainingWeek_WorkoutSession_NextWorkoutId REFERENCES WorkoutSession (Id) ON UPDATE CASCADE);

-- Table: TrainingWeekTemplate
DROP TABLE IF EXISTS TrainingWeekTemplate;
CREATE TABLE TrainingWeekTemplate (Id INTEGER CONSTRAINT PK_TrainingWeekTemplate_Id PRIMARY KEY AUTOINCREMENT, ProgressiveNumber INTEGER CONSTRAINT CK_TrainingWeekTemplate_ProgressiveNumber_NotNull NOT NULL CONSTRAINT CK_TrainingWeekTemplate_ProgressiveNumber_IsPositive CHECK (ProgressiveNumber >= 0), TrainingWeekTypeId INTEGER CONSTRAINT FK_TrainingWeekTemplate_TrainingWeekTemplate_Id REFERENCES TrainingWeekType (Id) ON UPDATE CASCADE, TrainingPlanId INTEGER CONSTRAINT FK_TrainingWeekTemplate_TrainingPlan_Id REFERENCES TrainingPlan (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_TrainingWeekTemplate_Id_NotNull NOT NULL);

-- Table: TrainingWeekType
DROP TABLE IF EXISTS TrainingWeekType;
CREATE TABLE TrainingWeekType (Id INTEGER CONSTRAINT PK_TrainingWeekType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_TrainingWeekType_Name UNIQUE CONSTRAINT CK_TrainingWeekType_Name_NotNull NOT NULL, Description TEXT);

-- Table: User
DROP TABLE IF EXISTS User;
CREATE TABLE User (Id INTEGER CONSTRAINT PK_User_Id PRIMARY KEY AUTOINCREMENT, Email TEXT CONSTRAINT UQ_User_Email UNIQUE CONSTRAINT CK_User_Email_NotNull NOT NULL, Username TEXT CONSTRAINT UQ_User_Username UNIQUE CONSTRAINT CK_User_Username_NotNull NOT NULL, Password TEXT (128) CONSTRAINT CK_User_Password_NotNull NOT NULL, Salt TEXT (128) CONSTRAINT UQ_User_Salt UNIQUE CONSTRAINT CK_User_Salt_NotNull NOT NULL, AccountStatusTypeId INTEGER REFERENCES AccountStatusType (Id) ON DELETE SET NULL ON UPDATE CASCADE);

-- Table: UserPhase
DROP TABLE IF EXISTS UserPhase;
CREATE TABLE UserPhase (Id INTEGER CONSTRAINT PK_UserPhase_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_UserPhase_Post_Id REFERENCES Post (Id) ON UPDATE CASCADE CONSTRAINT CK_UserPhase_Id_NotNull NOT NULL, StartDate INTEGER CONSTRAINT CK_UserPhase_StartDate_NotNull NOT NULL CONSTRAINT DF_UserPhase_StartDate DEFAULT (strftime('%s', CURRENT_DATE)), EndDate INTEGER, CreatedOn INTEGER CONSTRAINT CK_UserPhase_CreatedOn_NotNull NOT NULL CONSTRAINT DF_UserPhase_CreatedOn DEFAULT (strftime('%s', 'now')), OwnerId INTEGER CONSTRAINT FK_UserPhase_User_Id REFERENCES User (Id) ON UPDATE CASCADE CONSTRAINT CK_UserPhase_OwnerId_NotNull NOT NULL, PhaseId INTEGER CONSTRAINT FK_UserPhase_Phase_Id REFERENCES Phase (Id) ON UPDATE CASCADE CONSTRAINT CK_UserPhase_PhaseId_NotNull NOT NULL, UserPhaseNoteId INTEGER CONSTRAINT FK_UserPhase_UserPhaseNote_Id REFERENCES UserPhaseNote (Id) ON UPDATE CASCADE);

-- Table: UserPhaseNote
DROP TABLE IF EXISTS UserPhaseNote;
CREATE TABLE UserPhaseNote (Id INTEGER CONSTRAINT PK_UserPhaseNote_Id PRIMARY KEY AUTOINCREMENT, Body TEXT CONSTRAINT CK_UserPhaseNote_NotNull NOT NULL);

-- Table: UserRelation
DROP TABLE IF EXISTS UserRelation;
CREATE TABLE UserRelation (SourceUserId INTEGER CONSTRAINT FK_UserRelation_User_UserId REFERENCES User (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_UserRelation_UserId_NotNull NOT NULL, TargetUserId INTEGER CONSTRAINT FK_UserRelation_User_UserId REFERENCES User (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_UserRelation_UserId_NotNull NOT NULL CONSTRAINT CK_UserRelation_TargetUserId_DifferentFromSourceUserId CHECK (TargetUserId <> SourceUserId), LastUpdate INTEGER CONSTRAINT CK_UserRelation_LastUpdate_NotNull NOT NULL CONSTRAINT DF_UserRelation_LastUpdate DEFAULT (strftime('%s', 'now')), RelationStatusId INTEGER CONSTRAINT FK_UserRelation_RelationStatusId REFERENCES RelationStatus (Id) ON UPDATE CASCADE CONSTRAINT CK_UserRelation_RelationStatusId_NotNull NOT NULL, CONSTRAINT PK_UserRelation_SourceUserId_TargetUserId PRIMARY KEY (SourceUserId, TargetUserId));

-- Table: Weight
DROP TABLE IF EXISTS Weight;
CREATE TABLE Weight (Id INTEGER PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_Weight_FitnessDayEntry_Id REFERENCES FitnessDayEntry (Id) ON DELETE CASCADE ON UPDATE CASCADE, Kg INTEGER CONSTRAINT CK_Weight_Kg_NotNull NOT NULL CONSTRAINT CK_Weight_Kg_BetweenValues CHECK (Kg BETWEEN 0 AND 5000), FOREIGN KEY (Id) REFERENCES Post (Id));

-- Table: WellnessDay
DROP TABLE IF EXISTS WellnessDay;
CREATE TABLE WellnessDay (Id INTEGER CONSTRAINT PK_WellnessDay_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT FK_WellnessDay_FitnessDay_Id REFERENCES FitnessDayEntry (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_WellnessDay_Id_NotNull NOT NULL, Temperature REAL CONSTRAINT CK_WellnessDay_Temperature_NotNull CHECK (Temperature BETWEEN 32 AND 45), Glycemia INTEGER CONSTRAINT CK_WellnessDay_Glycemia_BetweenValues CHECK (Glycemia BETWEEN 0 AND 300));

-- Table: WellnessDayHasMus
DROP TABLE IF EXISTS WellnessDayHasMus;
CREATE TABLE WellnessDayHasMus (WellnessDayId INTEGER CONSTRAINT FK_WellnessDayHasMus_WellnessDay_Id REFERENCES WellnessDay (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_WellnessDayHasMus_Id_NotNull NOT NULL, MusId INTEGER CONSTRAINT FK_WellnessDayHasMus_Mus_MusId REFERENCES Mus (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_WellnessDayHasMus_MusId_NotNull NOT NULL, CONSTRAINT PK_WellnessDayHasMus_Ids PRIMARY KEY (WellnessDayId, MusId));

-- Table: WorkingSet
DROP TABLE IF EXISTS WorkingSet;
CREATE TABLE WorkingSet (Id INTEGER CONSTRAINT PK_WorkingSet_Id PRIMARY KEY AUTOINCREMENT, ProgressiveNumber INTEGER CONSTRAINT CK_WorkingSet_ProgressiveNumber_IsPositive CHECK (ProgressiveNumber >= 0) CONSTRAINT CK_WorkingSet_ProgressiveNumber_NotNull NOT NULL, RepetitionsTarget TEXT, Repetitions INTEGER CONSTRAINT CK_WorkingSet_Repetitions_Smallint CHECK (Repetitions BETWEEN 0 AND 32767), Rest INTEGER CONSTRAINT CK_WorkingSet_Rest_Smallint CHECK (Rest BETWEEN 0 AND 32767), Cadence TEXT, Effort INTEGER CONSTRAINT CK_WorkingSet_Effort_IsPositive CHECK (Effort >= 0), KgTarget INTEGER CONSTRAINT CK_WorkingSet_KgTarget_IsPositive CHECK (KgTarget >= 0), Kg INTEGER CONSTRAINT CK_WorkingSet_Kg_IsPositive CHECK (Kg >= 0), WorkUnitId INTEGER CONSTRAINT FK_WorkingSet_WorkUnit_Id REFERENCES WorkUnit (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_WorkingSet_WorkUnit_NotNull NOT NULL, EffortTypeId INTEGER CONSTRAINT FK_WorkingSet_EffortType_Id REFERENCES EffortType (Id) ON UPDATE CASCADE);

-- Table: WorkingSetIntensityTechnique
DROP TABLE IF EXISTS WorkingSetIntensityTechnique;
CREATE TABLE WorkingSetIntensityTechnique (WorkingSetId INTEGER CONSTRAINT FK_WorkingSetIntensityTechnique_WorkingSet_WorkingSetId REFERENCES SetTemplate (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_WorkingSetIntensityTechnique_WorkingSetId_NotNull NOT NULL, IntensityTechniqueId INTEGER CONSTRAINT FK_WorkingSetIntensityTechnique_IntensityTechnique_Id REFERENCES IntensityTechnique (Id) ON UPDATE CASCADE CONSTRAINT CK_WorkingSetIntensityTechnique_IntensityTechniqueId_NotNull NOT NULL, LinkedWorkingSetId INTEGER CONSTRAINT FK_WorkingSetIntensityTechnique_WorkingSet_LinkedWorkingSetId REFERENCES WorkingSet (Id) ON DELETE CASCADE ON UPDATE CASCADE, CONSTRAINT PK_WorkingSetIntensityTechnique_WorkingSetId_IntensityTechniqueId PRIMARY KEY (WorkingSetId, IntensityTechniqueId));

-- Table: WorkingSetNote
DROP TABLE IF EXISTS WorkingSetNote;
CREATE TABLE WorkingSetNote (Id INTEGER CONSTRAINT PK_WorkingSetNote_Id PRIMARY KEY AUTOINCREMENT, Body TEXT CONSTRAINT CK_WorkingSetNote_Body_NotNull NOT NULL, WorkingSetId INTEGER CONSTRAINT FK_WorkingSetNote_WorkingSet_WorkingSetId REFERENCES WorkingSet (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_WorkingSetNote_WorkingSetId_NotNull NOT NULL);

-- Table: WorkoutSession
DROP TABLE IF EXISTS WorkoutSession;
CREATE TABLE WorkoutSession (Id INTEGER CONSTRAINT PK_WorkoutDay_Id PRIMARY KEY AUTOINCREMENT, PlannedDate INTEGER CONSTRAINT CK_WorkoutDay_PlannedDate_IsPositive CHECK (PlannedDate > 0), StartTime INTEGER CONSTRAINT CK_WorkoutDay_StartTime_NotNull NOT NULL CONSTRAINT DF_WorkoutDay_StartTime DEFAULT (STRFTIME('%s', 'now')) CONSTRAINT CK_WorkoutDay_StartTime_IsPositive CHECK (StartTime > 0), EndTime INTEGER CONSTRAINT CK_WorkoutDay_EndTime_IsPositive CHECK (EndTime > 0), Name TEXT, Rating INTEGER CONSTRAINT CK_WorkoutDay_Rating_BetweenValues CHECK (Rating BETWEEN 0 AND 4), TrainingWeekId INTEGER CONSTRAINT FK_WorkoutDay_TrainingWeek_Id REFERENCES TainingWeek (Id) ON UPDATE CASCADE);

-- Table: WorkoutTemplate
DROP TABLE IF EXISTS WorkoutTemplate;
CREATE TABLE WorkoutTemplate (Id INTEGER CONSTRAINT PK_WorkoutTemplate_Id PRIMARY KEY AUTOINCREMENT, Name TEXT, IsWeekDaySpecific INTEGER CONSTRAINT CK_WorkoutTemplate_IsWeekDaySpecific_IsBoolean CHECK (IsWeekDaySpecific BETWEEN 0 AND 1) CONSTRAINT DF_WorkoutTemplate_IsWeekDaySpecific DEFAULT (0) CONSTRAINT CK_WorkoutTemplate_IsWeekDaySpecific_NotNull NOT NULL, ProgressiveNumber INTEGER CONSTRAINT CK_WorkoutTemplate_ProgressiveNumber_NotNull NOT NULL, TrainingWeekId INTEGER CONSTRAINT FK_WorkoutTemplate_TrainingWeekTemplate_Id REFERENCES TrainingWeekTemplate (Id) ON DELETE CASCADE ON UPDATE CASCADE NOT NULL);

-- Table: WorkUnit
DROP TABLE IF EXISTS WorkUnit;
CREATE TABLE WorkUnit (Id INTEGER CONSTRAINT PK_WorkUnit_Id PRIMARY KEY AUTOINCREMENT, Comment TEXT, QuickRating INTEGER CONSTRAINT CK_WorkUnit_QuickRating_BetweenValues CHECK (QuickRating BETWEEN 0 AND 2), ProgressiveNumber INTEGER CONSTRAINT CK_WorkUnit_ProgressiveNumber_IsPositive CHECK (ProgressiveNumber >= 0) CONSTRAINT CK_WorkUnit_ProgressiveNumber_NotNull NOT NULL, WorkoutSessionId INTEGER CONSTRAINT FK_WorkUnit_WorkoutSession_Id REFERENCES WorkoutSession (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_WorkUnit_WorkoutSessionId_NotNull NOT NULL, ExcerciseId INTEGER CONSTRAINT FK_WorkUnit_Excercise_ExcerciseId REFERENCES Excercise (Id) ON UPDATE CASCADE CONSTRAINT CK_WorkUnit_ExcerciseId_NotNull NOT NULL);

-- Table: WorkUnitTemplate
DROP TABLE IF EXISTS WorkUnitTemplate;
CREATE TABLE WorkUnitTemplate (Id INTEGER CONSTRAINT PK_WorkUnitTemplate_Id PRIMARY KEY AUTOINCREMENT, ProgressiveNumber INTEGER CONSTRAINT CK_WorkUnitTemplate_ProgressiveNumber_NotNull NOT NULL, WorkoutTemplateId INTEGER CONSTRAINT FK_WorkUnitTemplate_WorkoutTemplate_Id REFERENCES WorkoutTemplate (Id) ON DELETE CASCADE ON UPDATE CASCADE CONSTRAINT CK_WorkUnitTemplate_WorkoutTemplateId_NotNull NOT NULL, ExcerciseId INTEGER CONSTRAINT FK_WorkUnitTemplate_Excercise_ExcerciseId REFERENCES Excercise (Id) ON UPDATE CASCADE CONSTRAINT CK_WorkUnitTemplate_ExcerciseId_NotNull NOT NULL, WorkUnitTemplateNoteId INTEGER CONSTRAINT FK_WorkUnitTemplate_WorkUnitTemplateNote_Id REFERENCES WorkUnitTemplateNote (Id) ON DELETE SET NULL ON UPDATE CASCADE);

-- Table: WorkUnitTemplateNote
DROP TABLE IF EXISTS WorkUnitTemplateNote;
CREATE TABLE WorkUnitTemplateNote (Id INTEGER CONSTRAINT PK_WorkUnitTemplateAnnotation_Id PRIMARY KEY AUTOINCREMENT, Body TEXT CONSTRAINT CK_WorkUnitTemplateNote_Body_NotNull NOT NULL);

-- Index: IDX_BiaEntry_OwnerId
DROP INDEX IF EXISTS IDX_BiaEntry_OwnerId;
CREATE INDEX IDX_BiaEntry_OwnerId ON BiaEntry (OwnerId);

-- Index: IDX_Circumference_OwnerId
DROP INDEX IF EXISTS IDX_Circumference_OwnerId;
CREATE INDEX IDX_Circumference_OwnerId ON Circumference (OwnerId);

-- Index: IDX_Comment_PostId_UserId_CreatedOn
DROP INDEX IF EXISTS IDX_Comment_PostId_UserId_CreatedOn;
CREATE INDEX IDX_Comment_PostId_UserId_CreatedOn ON Comment (PostId, UserId, CreatedOn);

-- Index: IDX_DietPlan_CreatedOn_OwnerId
DROP INDEX IF EXISTS IDX_DietPlan_CreatedOn_OwnerId;
CREATE INDEX IDX_DietPlan_CreatedOn_OwnerId ON DietPlan (CreatedOn, OwnerId);

-- Index: IDX_DietPlanDay_DietPlanUnitId_DietDayTypeId
DROP INDEX IF EXISTS IDX_DietPlanDay_DietPlanUnitId_DietDayTypeId;
CREATE INDEX IDX_DietPlanDay_DietPlanUnitId_DietDayTypeId ON DietPlanDay (DietPlanUnitId, DietDayTypeId);

-- Index: IDX_DIetPlanUnit_DietPlanId_StartDate
DROP INDEX IF EXISTS IDX_DIetPlanUnit_DietPlanId_StartDate;
CREATE INDEX IDX_DIetPlanUnit_DietPlanId_StartDate ON DietPlanUnit (DietPlanId, StartDate, EndDate);

-- Index: IDX_DietPlanUnit_EndDate_IsNull
DROP INDEX IF EXISTS IDX_DietPlanUnit_EndDate_IsNull;
CREATE INDEX IDX_DietPlanUnit_EndDate_IsNull ON DietPlanUnit (DietPlanId, EndDate) WHERE EndDate IS NULL;

-- Index: IDX_Excercise_MuscleId_ExcerciseDifficultyId_TrainingEquipmentId
DROP INDEX IF EXISTS IDX_Excercise_MuscleId_ExcerciseDifficultyId_TrainingEquipmentId;
CREATE INDEX IDX_Excercise_MuscleId_ExcerciseDifficultyId_TrainingEquipmentId ON Excercise (MuscleId, ExcerciseDifficultyId, TrainingEquipmentId);

-- Index: IDX_Excercise_OwnerId
DROP INDEX IF EXISTS IDX_Excercise_OwnerId;
CREATE INDEX IDX_Excercise_OwnerId ON Excercise (OwnerId);

-- Index: IDX_ExcerciseFocus_FullCovering
DROP INDEX IF EXISTS IDX_ExcerciseFocus_FullCovering;
CREATE INDEX IDX_ExcerciseFocus_FullCovering ON ExerciseFocus (ExerciseId, PerformanceFocusId);

-- Index: IDX_ExcerciseSecondaryTarget_FullCovering
DROP INDEX IF EXISTS IDX_ExcerciseSecondaryTarget_FullCovering;
CREATE INDEX IDX_ExcerciseSecondaryTarget_FullCovering ON ExcerciseSecondaryTarget (ExcerciseId, MuscleId, MuscleWorkTypeId);

-- Index: IDX_FitnessDayEntry_DayDate
DROP INDEX IF EXISTS IDX_FitnessDayEntry_DayDate;
CREATE INDEX IDX_FitnessDayEntry_DayDate ON FitnessDayEntry (DayDate);

-- Index: IDX_Image_PostId_CreatedOn_IsProgressPicture
DROP INDEX IF EXISTS IDX_Image_PostId_CreatedOn_IsProgressPicture;
CREATE INDEX IDX_Image_PostId_CreatedOn_IsProgressPicture ON Image (PostId, CreatedOn, IsProgressPicture);

-- Index: IDX_IntensityTechnique_OwnerId
DROP INDEX IF EXISTS IDX_IntensityTechnique_OwnerId;
CREATE INDEX IDX_IntensityTechnique_OwnerId ON IntensityTechnique (OwnerId);

-- Index: IDX_LinkedWorkUnit_FullCovering
DROP INDEX IF EXISTS IDX_LinkedWorkUnit_FullCovering;
CREATE INDEX IDX_LinkedWorkUnit_FullCovering ON LinkedWorkUnit (FirstWorkUnitId, SecondWorkUnitId, IntensityTechniqueId);

-- Index: IDX_LinkedWorkUnitTemplate_FullCovering
DROP INDEX IF EXISTS IDX_LinkedWorkUnitTemplate_FullCovering;
CREATE INDEX IDX_LinkedWorkUnitTemplate_FullCovering ON LinkedWorkUnitTemplate (FirstWorkUnitId, SecondWorkUnitId, IntensityTechniqueId);

-- Index: IDX_MeasureEntry_MeasureDate
DROP INDEX IF EXISTS IDX_MeasureEntry_MeasureDate;
CREATE INDEX IDX_MeasureEntry_MeasureDate ON MeasuresEntry (MeasureDate);

-- Index: IDX_Phase_OwnerId
DROP INDEX IF EXISTS IDX_Phase_OwnerId;
CREATE INDEX IDX_Phase_OwnerId ON Phase (OwnerId);

-- Index: IDX_Plicometry_OwnerId
DROP INDEX IF EXISTS IDX_Plicometry_OwnerId;
CREATE INDEX IDX_Plicometry_OwnerId ON Plicometry (OwnerId);

-- Index: IDX_Post_UserId_CreatedOn
DROP INDEX IF EXISTS IDX_Post_UserId_CreatedOn;
CREATE INDEX IDX_Post_UserId_CreatedOn ON Post (CreatedOn DESC, UserId);

-- Index: IDX_Post_UserId_CreatedOn
DROP INDEX IF EXISTS IDX_Post_UserId_CreatedOn;
CREATE INDEX IDX_Post_UserId_CreatedOn ON Post (UserId, CreatedOn);

-- Index: IDX_PostHasHashtag_FullCovering
DROP INDEX IF EXISTS IDX_PostHasHashtag_FullCovering;
CREATE INDEX IDX_PostHasHashtag_FullCovering ON PostHasHashtag (PostId, HashtagId);

-- Index: IDX_SetTemplate_WorkUnitId_ProgressiveNumber
DROP INDEX IF EXISTS IDX_SetTemplate_WorkUnitId_ProgressiveNumber;
CREATE INDEX IDX_SetTemplate_WorkUnitId_ProgressiveNumber ON SetTemplate (WorkUnitId, ProgressiveNumber);

-- Index: IDX_SetTemplateIntensityTechnique_FullCovering
DROP INDEX IF EXISTS IDX_SetTemplateIntensityTechnique_FullCovering;
CREATE INDEX IDX_SetTemplateIntensityTechnique_FullCovering ON SetTemplateIntensityTechnique (SetTemplateId, LinkedSetTemplateId, IntensityTechniqueId);

-- Index: IDX_TrainingMuscleFocus_FullCovering
DROP INDEX IF EXISTS IDX_TrainingMuscleFocus_FullCovering;
CREATE INDEX IDX_TrainingMuscleFocus_FullCovering ON TrainingMuscleFocus (TrainingPlanId, MuscleId);

-- Index: IDX_TrainingPlan_OwnerId_TrainingPlanNoteId
DROP INDEX IF EXISTS IDX_TrainingPlan_OwnerId_CreatedOn_IsBookmarked;
CREATE INDEX IDX_TrainingPlan_OwnerId_TrainingPlanNoteId ON TrainingPlan (OwnerId, TrainingPlanNoteId);

-- Index: IDX_TrainingPlanHasPhase_FullCovering
DROP INDEX IF EXISTS IDX_TrainingPlanHasPhase_FullCovering;
CREATE INDEX IDX_TrainingPlanHasPhase_FullCovering ON TrainingPlanHasPhase (PlanId, PhaseId);

-- Index: IDX_TrainingPlanRelation_ParentPlanId_ChildPlanId_TrainingPlanMessageId
DROP INDEX IF EXISTS IDX_TrainingPlanRelation_ParentPlanId_ChildPlanId_TrainingPlanMessageId;
CREATE INDEX IDX_TrainingPlanRelation_ParentPlanId_ChildPlanId_TrainingPlanMessageId ON TrainingPlanRelation (ParentPlanId, ChildPlanId, TrainingPlanMessageId);

-- Index: IDX_TrainingPlanTarget_FullCovering
DROP INDEX IF EXISTS IDX_TrainingPlanTarget_FullCovering;
CREATE INDEX IDX_TrainingPlanTarget_FullCovering ON TrainingPlanTarget (TrainingPlanId, TrainingProficiencyId);

-- Index: IDX_TrainingSchedule_StartDate_TrainingPlanId_CurrentWeekId_PhaseId_TrainingProficiencyId
DROP INDEX IF EXISTS IDX_TrainingSchedule_StartDate_TrainingPlanId_CurrentWeekId_PhaseId_TrainingProficiencyId;
CREATE INDEX IDX_TrainingSchedule_StartDate_TrainingPlanId_CurrentWeekId_PhaseId_TrainingProficiencyId ON TrainingSchedule (StartDate, TrainingPlanId, CurrentWeekId, PhaseId, TrainingProficiencyId);

-- Index: IDX_TrainingSchedule_TrainingPlanId_CurrentWeekId_PhaseId_TrainingProficiencyId
DROP INDEX IF EXISTS IDX_TrainingSchedule_TrainingPlanId_CurrentWeekId_PhaseId_TrainingProficiencyId;
CREATE INDEX IDX_TrainingSchedule_TrainingPlanId_CurrentWeekId_PhaseId_TrainingProficiencyId ON TrainingSchedule (TrainingPlanId, CurrentWeekId, PhaseId, TrainingProficiencyId);

-- Index: IDX_TrainingScheduleFeedback_TrainingScheduleId_UserId
DROP INDEX IF EXISTS IDX_TrainingScheduleFeedback_TrainingScheduleId_UserId;
CREATE INDEX IDX_TrainingScheduleFeedback_TrainingScheduleId_UserId ON TrainingScheduleFeedback (TrainingScheduleId, UserId);

-- Index: IDX_TrainingScheduleHashtag_TrainingScheduleId_TrainingHashtagId
DROP INDEX IF EXISTS IDX_TrainingScheduleHashtag_TrainingScheduleId_TrainingHashtagId;
CREATE INDEX IDX_TrainingScheduleHashtag_TrainingScheduleId_TrainingHashtagId ON TrainingScheduleHashtag (TrainingScheduleId, TrainingHashtagId);

-- Index: IDX_TrainingWeek_TrainingScheduleId_ProgressiveNumber
DROP INDEX IF EXISTS IDX_TrainingWeek_TrainingScheduleId_ProgressiveNumber;
CREATE INDEX IDX_TrainingWeek_TrainingScheduleId_ProgressiveNumber ON TrainingWeek (TrainingScheduleId, ProgressiveNumber);

-- Index: IDX_TrainingWeekTemplate_TrainingPlanId_ProgressiveNumber
DROP INDEX IF EXISTS IDX_TrainingWeekTemplate_TrainingPlanId_ProgressiveNumber;
CREATE INDEX IDX_TrainingWeekTemplate_TrainingPlanId_ProgressiveNumber ON TrainingWeekTemplate (TrainingPlanId, ProgressiveNumber);

-- Index: IDX_UserPhase_OwnerId_PhaseId_StartDate_UserPhaseNoteId
DROP INDEX IF EXISTS IDX_UserPhase_OwnerId_PhaseId_StartDate_UserPhaseNoteId;
CREATE INDEX IDX_UserPhase_OwnerId_PhaseId_StartDate_UserPhaseNoteId ON UserPhase (OwnerId, PhaseId, StartDate, UserPhaseNoteId);

-- Index: IDX_UserRelation_SourceUserId_TargetUserId_RelationStatusId
DROP INDEX IF EXISTS IDX_UserRelation_SourceUserId_TargetUserId_RelationStatusId;
CREATE INDEX IDX_UserRelation_SourceUserId_TargetUserId_RelationStatusId ON UserRelation (SourceUserId, TargetUserId, RelationStatusId);

-- Index: IDX_WellnessDayHasMus_WellnessDayId_MusId
DROP INDEX IF EXISTS IDX_WellnessDayHasMus_WellnessDayId_MusId;
CREATE INDEX IDX_WellnessDayHasMus_WellnessDayId_MusId ON WellnessDayHasMus (WellnessDayId, MusId);

-- Index: IDX_WorkingSet_WorkUnitId_ProgressiveNumber
DROP INDEX IF EXISTS IDX_WorkingSet_WorkUnitId_ProgressiveNumber;
CREATE INDEX IDX_WorkingSet_WorkUnitId_ProgressiveNumber ON WorkingSet (WorkUnitId, ProgressiveNumber);

-- Index: IDX_WorkingSetIntensityTechnique_FullCovering
DROP INDEX IF EXISTS IDX_WorkingSetIntensityTechnique_FullCovering;
CREATE INDEX IDX_WorkingSetIntensityTechnique_FullCovering ON WorkingSetIntensityTechnique (WorkingSetId, LinkedWorkingSetId, IntensityTechniqueId);

-- Index: IDX_WorkingSetNote_WorkingSetId
DROP INDEX IF EXISTS IDX_WorkingSetNote_WorkingSetId;
CREATE INDEX IDX_WorkingSetNote_WorkingSetId ON WorkingSetNote (WorkingSetId);

-- Index: IDX_WorkoutSession_TrainingWeekId_StartTime
DROP INDEX IF EXISTS IDX_WorkoutSession_TrainingWeekId_StartTime;
CREATE INDEX IDX_WorkoutSession_TrainingWeekId_StartTime ON WorkoutSession (TrainingWeekId, StartTime);

-- Index: IDX_WorkoutTemplate_TrainingWeekId_ProressiveNumber
DROP INDEX IF EXISTS IDX_WorkoutTemplate_TrainingWeekId_ProressiveNumber;
CREATE INDEX IDX_WorkoutTemplate_TrainingWeekId_ProressiveNumber ON WorkoutTemplate (TrainingWeekId, ProgressiveNumber);

-- Index: IDX_WorkUnit_WorkoutSessionId_ExcerciseId_ProgressiveNumber
DROP INDEX IF EXISTS IDX_WorkUnit_WorkoutSessionId_ExcerciseId_ProgressiveNumber;
CREATE INDEX IDX_WorkUnit_WorkoutSessionId_ExcerciseId_ProgressiveNumber ON WorkUnit (WorkoutSessionId, ExcerciseId, ProgressiveNumber);

-- Index: IDX_WorkUnitTemplate_FullCovering
DROP INDEX IF EXISTS IDX_WorkUnitTemplate_FullCovering;
CREATE INDEX IDX_WorkUnitTemplate_FullCovering ON WorkUnitTemplate (WorkoutTemplateId, ExcerciseId, ProgressiveNumber, WorkUnitTemplateNoteId);

-- Index: IDX_UserLike_PostId_UserId
DROP INDEX IF EXISTS IDX_UserLike_PostId_UserId;
CREATE INDEX IDX_UserLike_PostId_UserId ON UserLike (PostId, UserId);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
