


-- EPLEY  Equation for estimating the 1RM


SELECT Kg, Repetitions, round(Kg * (1 + Repetitions / 30.0), 2) as RepMaxKg,  round(Kg * (1 + Repetitions / 30.0), 2) / 10.0 as RepMaxKg2
FROM WorkingSet



;





-- Regression approximant for RM concersion - 4th order 
-- NOTE: This is an user-defined function which must be present on the DB. Two ways of doing this


-- 1) SQLITEstudio SQL Function -> Tcl language
--	This is used as a testing tool but can't be deployed onto the local databases
set rm [lindex $argv 0]

expr { 0.4167 * $rm - 14.2831 * pow($rm, 0.5) + 115.6122 }

--  2) Register SQL function via SQLiteConnection in the Software.
--	This is the way to do it in the final SW. Example:
[SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
class MyRegEx : SQLiteFunction
{
   public override object Invoke(object[] args)
   {
      return System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(args[1]),Convert.ToString(args[0]));
   }
}


-- 3) To be used on the web app: Mysql stored procedure / function

-- tbd...


;










-- TRAINING_PLAN_USER_0


-- Get all training plan entries for the selected user. Should be used in the Training Schedules View (but requires post-processing).

--PERFORMANCE: 

-- NOTE: Needs post-processing to get the quantities needed (the ones in TRAINING_PLAN_USER_1).
--		Furthermore, there might be a huge number of duplicates (each plan will be duplicated foreach hashtag/phase/proficiency)
--		-> If a Training plan with N working sets, has M hashtags, L Targets and U phases the total rows fetched will be N*M*L*U



SELECT TP.Id, TP.Name as PlanName, TP.IsBookmarked, TP.IsTemplate, 
TH.Body As Hashtag, TProf.Name as Proficiency, Pha.Name as Phase,

WT.Id as WeekId, WOT.Id as WorkoutId, WUT.Id as WorkUnitId, ST.TargetRepetitions

FROM TrainingPlan TP
JOIN User U
ON TP.OwnerId = U.Id

-- Notes and hashtags
LEFT JOIN TrainingPlanHasHashtag TPHH
ON TPHH.TrainingPlanId = TP.Id
LEFT JOIN TrainingHashtag TH
ON TPHH.TrainingHashtagId = TH.Id
LEFT JOIN TrainingPlanTarget TPT
ON TPT.TrainingPlanId = TP.Id
LEFT JOIN TrainingProficiency TProf
ON TProf.Id = TPT.TrainingProficiencyId
LEFT JOIN TrainingPlanHasPhase TPHP
ON TPHP.PlanId = TP.Id
LEFT JOIN Phase Pha
ON TPHP.PhaseId = Pha.Id

-- Workouts
JOIN TrainingWeekTemplate WT
ON TP.Id = WT.TrainingPlanId
JOIN WorkoutTemplate WOT
ON WT.Id = WOT.TrainingWeekId
JOIN WorkUnitTemplate WUT
ON WOT.Id = WUT.WorkoutTemplateId
JOIN SetTemplate ST
ON WUT.Id = ST.WorkUnitId

WHERE U.Id = 12

ORDER BY TP.IsBookmarked DESC, TP.IsTemplate DESC







;





-- TRAINING_PLAN_USER_1


-- Get all training plan entries for the selected user. Should be used in the Training Schedules View.
-- AGGREGATE ON DIFFERENT SUBQUERIES

--PERFORMANCE: 

-- NOTE: 




SELECT TP.Id, TP.Name as PlanName, TP.IsBookmarked, TP.IsTemplate, 
TH.Body As Hashtag, TProf.Name as Proficiency, Pha.Name as Phase,

--WT.Id as WeekId, WOT.Id as WorkoutId, WUT.Id as WorkUnitId, ST.TargetRepetitions,

-- Average Workout Days per plan
(
    SELECT AVG(WoCount.Counter)
    FROM
    (
        SELECT TrainingPlanId, count(1) as Counter
        FROM TrainingWeekTemplate
        JOIN WorkoutTemplate
        ON TrainingWeekTemplate.Id = TrainingWeekId
        
        WHERE TrainingPlanId = TP.Id
        GROUP BY TrainingWeekTemplate.Id
    ) WoCount
) AS AvgWorkoutDays,

-- Average weekly working sets
(
    SELECT round(Avg(WsCount.Counter), 1)
    FROM
    (
        select TrainingWeekTemplate.Id, count(1) as Counter
        FROM TrainingWeekTemplate
        JOIN WorkoutTemplate
        ON TrainingWeekTemplate.Id = TrainingWeekId
        JOIN WorkUnitTemplate
        ON WorkoutTemplate.Id = WorkoutTemplateId
        JOIN SetTemplate
        ON WorkUnitTemplate.Id = WorkUnitId
    
        WHERE TrainingPlanId = TP.Id
        GROUP BY TrainingWeekTemplate.Id
    ) WsCount
) As AvgWorkingSets,

-- Average Intensity
(
    SELECT Round(Avg (
    CASE
        WHEN EffortTypeId = 1 THEN Effort / 10.0     -- Intensity [%]
        WHEN EffortTypeId = 2 THEN Round(RmToIntensityPerc(Effort), 1)    -- RM
        WHEN EffortTypeId = 3 THEN Round(RmToIntensityPerc(TargetRepetitions + (10 - Effort)), 1)      -- RPE
        ELSE null
    END), 1) as AvgIntensity
    
    FROM TrainingWeekTemplate
    JOIN WorkoutTemplate
    ON TrainingWeekTemplate.Id = TrainingWeekId
    JOIN WorkUnitTemplate
    ON WorkoutTemplate.Id = WorkoutTemplateId
    JOIN SetTemplate
    ON WorkUnitTemplate.Id = WorkUnitId
    
    WHERE TrainingPlanId = TP.Id
) As AvgIntensityPerc,


-- Last workout date
(
    SELECT Max(StartTime)
    FROM TrainingSchedule
    JOIN TrainingWeek
    ON TrainingSchedule.Id = TrainingScheduleId 
    JOIN WorkoutSession
    ON TrainingWeek.Id = TrainingWeekId

    WHERE TrainingPlanId = TP.Id
) As LastWorkoutTs




FROM TrainingPlan TP
JOIN User U
ON TP.OwnerId = U.Id

-- Notes and hashtags
LEFT JOIN TrainingPlanHasHashtag TPHH
ON TPHH.TrainingPlanId = TP.Id
LEFT JOIN TrainingHashtag TH
ON TPHH.TrainingHashtagId = TH.Id
LEFT JOIN TrainingPlanTarget TPT
ON TPT.TrainingPlanId = TP.Id
LEFT JOIN TrainingProficiency TProf
ON TProf.Id = TPT.TrainingProficiencyId
LEFT JOIN TrainingPlanHasPhase TPHP
ON TPHP.PlanId = TP.Id
LEFT JOIN Phase Pha
ON TPHP.PhaseId = Pha.Id


WHERE U.Id = 12

ORDER BY TP.IsBookmarked DESC, TP.IsTemplate DESC





;




-- TRAINING_PLAN_USER_2


-- Get all training plan entries for the selected user. Should be used in the Training Schedules View.
-- ALL AGGREGATES ON JOIN

--PERFORMANCE: 

-- NOTE: 


SELECT *

FROM 
(
    SELECT TP.Id, TP.Name as PlanName, TP.IsBookmarked, TP.IsTemplate, 
    TH.Body As Hashtag, TProf.Name as Proficiency, Pha.Name as Phase,

    -- Last workout date
    (
        SELECT Max(StartTime)
        FROM TrainingSchedule
        JOIN TrainingWeek
        ON TrainingSchedule.Id = TrainingScheduleId 
        JOIN WorkoutSession
        ON TrainingWeek.Id = TrainingWeekId
    
        WHERE TrainingPlanId = TP.Id
    ) As LastWorkoutTs
    
    
    
    FROM TrainingPlan TP
    JOIN User U
    ON TP.OwnerId = U.Id
    
    -- Notes and hashtags
    LEFT JOIN TrainingPlanHasHashtag TPHH
    ON TPHH.TrainingPlanId = TP.Id
    LEFT JOIN TrainingHashtag TH
    ON TPHH.TrainingHashtagId = TH.Id
    LEFT JOIN TrainingPlanTarget TPT
    ON TPT.TrainingPlanId = TP.Id
    LEFT JOIN TrainingProficiency TProf
    ON TProf.Id = TPT.TrainingProficiencyId
    LEFT JOIN TrainingPlanHasPhase TPHP
    ON TPHP.PlanId = TP.Id
    LEFT JOIN Phase Pha
    ON TPHP.PhaseId = Pha.Id
    
    
    WHERE U.Id = 12
    
    ORDER BY TP.IsBookmarked DESC, TP.IsTemplate DESC
) Q1

JOIN

(
    SELECT TrainingPlanId as PlanId, 
    AVG(AvgIntensity) as AvgPlanIntensity, Avg(WsCounter) as AvgPlanWorkingSets, Avg(WoCounter) as AvgPlanWorkouts
    
    FROM
    (
        SELECT Round(Avg (
        CASE
            WHEN EffortTypeId = 1 THEN Effort / 10.0     -- Intensity [%]
            WHEN EffortTypeId = 2 THEN Round(RmToIntensityPerc(Effort), 1)    -- RM
            WHEN EffortTypeId = 3 THEN Round(RmToIntensityPerc(TargetRepetitions + (10 - Effort)), 1)      -- RPE
            ELSE null
        END), 1) as AvgIntensity
        , Count(1) as WsCounter
        , Count(DISTINCT(WorkoutTemplate.Id)) as WoCounter
        , TrainingWeekTemplate.TrainingPlanId
        
        FROM TrainingPlan TP
        JOIN User U
        ON TP.OwnerId = U.Id
        JOIN TrainingWeekTemplate
        ON TP.Id = TrainingPlanId
        JOIN WorkoutTemplate
        ON TrainingWeekTemplate.Id = TrainingWeekId
        JOIN WorkUnitTemplate
        ON WorkoutTemplate.Id = WorkoutTemplateId
        JOIN SetTemplate
        ON WorkUnitTemplate.Id = WorkUnitId
        
        WHERE U.Id = 12
        --WHERE TrainingPlanId = TP.Id
        GROUP BY TrainingWeekTemplate.Id
        )
        GROUP BY TrainingPlanId
) Q2
ON Q2.PlanId = Q1.Id



;




-- TRAINING_PLAN_USER_3


-- Get all training plan entries for the selected user. Should be used in the Training Schedules View.
-- ALL AGGREGATES ON JOIN

--PERFORMANCE: 

-- NOTE: 



SELECT *
FROM 
(
    SELECT TP.Id, TP.Name as PlanName, TP.IsBookmarked, TP.IsTemplate, 
    TH.Body As Hashtag, TProf.Name as Proficiency, Pha.Name as Phase


    
    
    
    FROM TrainingPlan TP
    JOIN User U
    ON TP.OwnerId = U.Id
    
    -- Notes and hashtags
    LEFT JOIN TrainingPlanHasHashtag TPHH
    ON TPHH.TrainingPlanId = TP.Id
    LEFT JOIN TrainingHashtag TH
    ON TPHH.TrainingHashtagId = TH.Id
    LEFT JOIN TrainingPlanTarget TPT
    ON TPT.TrainingPlanId = TP.Id
    LEFT JOIN TrainingProficiency TProf
    ON TProf.Id = TPT.TrainingProficiencyId
    LEFT JOIN TrainingPlanHasPhase TPHP
    ON TPHP.PlanId = TP.Id
    LEFT JOIN Phase Pha
    ON TPHP.PhaseId = Pha.Id
    
    
    WHERE U.Id = 12
    
    ORDER BY TP.IsBookmarked DESC, TP.IsTemplate DESC
) Q1

JOIN

(
    SELECT TrainingPlanId as PlanId, 
    AVG(AvgIntensity) as AvgPlanIntensity, Avg(WsCounter) as AvgPlanWorkingSets, Avg(WoCounter) as AvgPlanWorkouts, Ts as LastWorkoutTs
    
    FROM
    (
        SELECT Round(Avg (
        CASE
            WHEN EffortTypeId = 1 THEN Effort / 10.0     -- Intensity [%]
            WHEN EffortTypeId = 2 THEN Round(RmToIntensityPerc(Effort), 1)    -- RM
            WHEN EffortTypeId = 3 THEN Round(RmToIntensityPerc(TargetRepetitions + (10 - Effort)), 1)      -- RPE
            ELSE null
        END), 1) as AvgIntensity
        , Count(1) as WsCounter
        , Count(DISTINCT(WO.Id)) as WoCounter
        , TWeek.TrainingPlanId
        , Max(StartTime) as Ts
        
        FROM TrainingPlan TP
        JOIN User U
        ON TP.OwnerId = U.Id
        JOIN TrainingWeekTemplate TWeek
        ON TP.Id = TWeek.TrainingPlanId
        JOIN WorkoutTemplate WO
        ON TWeek.Id = WO.TrainingWeekId
        JOIN WorkUnitTemplate 
        ON WO.Id = WorkoutTemplateId
        JOIN SetTemplate
        ON WorkUnitTemplate.Id = WorkUnitId
        
        -- Might have never been scheduled, Include it anyway
        LEFT JOIN TrainingSchedule TSched
        ON TP.Id = TSched.TrainingPlanId
        LEFT JOIN TrainingWeek WSched
        ON TSched.Id = WSched.TrainingScheduleId 
        LEFT JOIN WorkoutSession WOS
        ON WSched.Id = WOS.TrainingWeekId
        
        
        WHERE U.Id = 12
        --WHERE TrainingPlanId = TP.Id
        GROUP BY TWeek.Id
        )
        GROUP BY TrainingPlanId
) Q2
ON Q2.PlanId = Q1.Id

