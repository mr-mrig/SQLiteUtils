

-- Populate training feedbacks : one feedback per owner (as if it were no trainer Vs trainee relationship)


INSERT INTO TrainingScheduleFeedback
(
	Comment,
	Rating,
	TrainingScheduleId,
	UserId
)

SELECT 'My Feedback ' || OwnerId || ' ' || TrainingPlanId, 
    2 + (OwnerId % 3 = 0) + (TrainingPlanId % 2 = 1),
    TS.Id,
    TP.OwnerId

FROM TrainingSchedule TS
JOIN TrainingPlan TP
ON TP.Id = TS.TrainingPlanId

;


-- 

UPDATE TrainingPlanRelation
SET RelationTypeId = 2
WHERE RelationTypeId = 1
AND rowid % 4 = 0


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

--PERFORMANCE: 2x Faster than TRAINING_PLAN_USER_0

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
-- AGGREGATES on JOIN - But Last WO TS

--PERFORMANCE: Similar to TRAINING_PLAN_USER_1 - Requires more benchmark

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

--PERFORMANCE: Slowest one - 5x slower than TRAINING_PLAN_USER_0 - Why? - Requires more benchmark

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










;






-- TRAINING_VARIANTS_RECURSIVE


-- Recursively fetch the variants of a specific plan (hence the variants of variants and so on)

--PERFORMANCE: 

-- NOTE: Might be wise not to search recursively but just to stop at the first level:
--		1. 100x faster then performing the recursive search (however if local DB it's instant as well)
--		2. Restricts the plans showed to the user, which might be preferable (if all the plans indirectily derive from the root one, is it correct to show them all?)
--		3. If the user wants more detail, then it can select a child and fetch its childs



WITH RECURSIVE IsVariantOf(Id, ChildId) AS 
(

    SELECT TP.Id, TPR.ChildPlanId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    
    WHERE RelationTypeId = 1
    
    UNION ALL
    
    SELECT TP.Id, IsVariantOf.ChildId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    JOIN IsVariantOf
    ON IsVariantOf.Id = TPR.ChildPlanId
    
    WHERE RelationTypeId = 1
    
  )
  
SELECT ChildId
FROM IsVariantOf
WHERE IsVariantOf.Id = 4

UNION ALL

VALUES(4)		-- Include the parent plan





;




-- TRAINING_FEEDBACKS_0
-- Phase-Schedule relation

-- Get the feedbacks of all the the plans 

--PERFORMANCE: 

-- NOTE: This query requires the presence of UserPhase linked to TrainingSchedule which can be seen as denormalization


SELECT TP.Id as PlanId, U.Username, 
TS.Id, TS.StartDate, TS.EndDate,
TProf.Name as Proficiency, Ph.Name as Phase,
TSF.Comment as FeedbackNote, TSF.Rating as FedbackRating

-- Get child plans (IE the ones which have been sent to trainees)
FROM TrainingPlan TPRoot
JOIN TrainingPlanRelation TPR
ON TPRoot.Id = TPR.ParentPlanId
JOIN TrainingPlan TP
ON TP.Id = TPR.ChildPlanId
JOIN User U
ON TP.OwnerId = U.Id

-- Proficiency
JOIN TrainingSchedule TS
ON TS.TrainingPlanId = TP.Id
LEFT JOIN TrainingProficiency TProf
ON TS.TrainingProficiencyId = TProf.Id

-- Phase
LEFT JOIN UserPhase UP
ON TS.PhaseId = UP.Id
LEFT JOIN Phase Ph
ON UP.PhaseId = Ph.Id

-- Feedback
LEFT JOIN TrainingScheduleFeedback TSF
ON TSF.TrainingScheduleId = TS.Id

--WHERE TPRoot.Id IN (4,2,3,5,7,10,11)		-- If too slow use these instead of the Recursive Query
--AND TPR.RelationTypeId = 2

-- Variants of main plan
WHERE TPRoot.Id IN
(
WITH RECURSIVE
  IsVariantOf(Id, ChildId) AS 
  (

    --VALUES(4,4)
    SELECT TP.Id, TPR.ChildPlanId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    
    WHERE RelationTypeId = 1
    
    UNION ALL
    
    SELECT TP.Id, IsVariantOf.ChildId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    JOIN IsVariantOf
    ON IsVariantOf.Id = TPR.ChildPlanId
    
        WHERE RelationTypeId = 1
    
  )
  
SELECT ChildId
FROM IsVariantOf
WHERE IsVariantOf.Id = 4		-- Insert root plan here

UNION ALL

VALUES(4)						-- Inert root plan here

)
 
AND TPR.RelationTypeId = 2





;





-- TRAINING_FEEDBACKS_1
-- NO Phase-Schedule relation

-- Get the feedbacks of all the the plans 

--PERFORMANCE: similar to TRAINING_FEEDBACKS_0, but is more normalized! More benchmark is suggested

-- NOTE: This query fetches the UserPhase joining with the User-Post-UserPhase





SELECT TP.Id as PlanId, U.Username, 
TS.Id, TS.StartDate, TS.EndDate,
TProf.Name as Proficiency, Ph.Name as Phase,
TSF.Comment as FeedbackNote, TSF.Rating as FedbackRating

-- Get child plans (IE the ones which have been sent to trainees)
FROM TrainingPlan TPRoot
JOIN TrainingPlanRelation TPR
ON TPRoot.Id = TPR.ParentPlanId
JOIN TrainingPlan TP
ON TP.Id = TPR.ChildPlanId
JOIN User U
ON TP.OwnerId = U.Id

-- Proficiency
JOIN TrainingSchedule TS
ON TS.TrainingPlanId = TP.Id
LEFT JOIN TrainingProficiency TProf
ON TS.TrainingProficiencyId = TProf.Id

-- Phase

LEFT JOIN Post P
ON P.UserId = U.Id
JOIN UserPhase UP
ON UP.Id = P.Id
AND UP.StartDate <= TS.StartDate
AND UP.EndDate >= TS.StartDate
JOIN Phase Ph
ON UP.PhaseId = Ph.Id

-- Feedback
LEFT JOIN TrainingScheduleFeedback TSF
ON TSF.TrainingScheduleId = TS.Id

--WHERE TPRoot.Id IN (4,2,3,5,7,10,11)		-- If too slow use these instead of the Recursive Query
--AND TPR.RelationTypeId = 2



-- Variants of main plan
WHERE TPRoot.Id IN
(
WITH RECURSIVE
  IsVariantOf(Id, ChildId) AS 
  (

    --VALUES(4,4)
    SELECT TP.Id, TPR.ChildPlanId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    
    WHERE RelationTypeId = 1
    
    UNION ALL
    
    SELECT TP.Id, IsVariantOf.ChildId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    JOIN IsVariantOf
    ON IsVariantOf.Id = TPR.ChildPlanId
    
        WHERE RelationTypeId = 1
    
  )
  
SELECT ChildId
FROM IsVariantOf
WHERE IsVariantOf.Id = 4

UNION ALL

VALUES(4)

)
 
AND TPR.RelationTypeId = 2




;




-- TRAINING_FEEDBACKS_2
-- NO Phase-Schedule and no Proficiency-Schedule relations

-- Get the feedbacks of all the the plans 

--PERFORMANCE: similar to TRAINING_FEEDBACKS_0 and TRAINING_FEEDBACKS_1, but is more normalized! More benchmark is suggested

-- NOTE: This query fetches the UserPhase joining with the User-Post-UserPhase
--		and the TrainingProficiency joining with UserHasProficiency

-- NOTE2: To test this some manual table manipulation is required



SELECT TP.Id as PlanId, U.Username, U.Id as UserId,
TS.Id, TS.StartDate, TS.EndDate,
TProf.Name as Proficiency, Ph.Name as Phase,
TSF.Comment as FeedbackNote, TSF.Rating as FedbackRating

-- Get child plans (IE the ones which have been sent to trainees)
FROM TrainingPlan TPRoot
JOIN TrainingPlanRelation TPR
ON TPRoot.Id = TPR.ParentPlanId
JOIN TrainingPlan TP
ON TP.Id = TPR.ChildPlanId
JOIN User U
ON TP.OwnerId = U.Id

-- Proficiency
JOIN TrainingSchedule TS
ON TS.TrainingPlanId = TP.Id
LEFT JOIN UserHasProficiency UHP
ON UHP.OwnerId = TPRoot.OwnerId
AND UHP.UserId = U.Id
AND (UHP.StartDate <= TS.Startdate
AND UHP.EndDate >= TS.Startdate
OR UHP.EndDate is null)
LEFT JOIN TrainingProficiency TProf
ON TProf.Id = UHP.ProficiencyId


-- Phase
LEFT JOIN Post P
ON P.UserId = U.Id
JOIN UserPhase UP
ON UP.Id = P.Id
AND UP.StartDate <= TS.StartDate
AND UP.EndDate >= TS.StartDate
JOIN Phase Ph
ON UP.PhaseId = Ph.Id

-- Feedback
LEFT JOIN TrainingScheduleFeedback TSF
ON TSF.TrainingScheduleId = TS.Id

--WHERE TPRoot.Id IN (4,2,3,5,7,10,11)		-- If too slow use these instead of the Recursive Query
--AND TPR.RelationTypeId = 2



-- Variants of main plan
WHERE TPRoot.Id IN
(
WITH RECURSIVE
  IsVariantOf(Id, ChildId) AS 
  (

    --VALUES(4,4)
    SELECT TP.Id, TPR.ChildPlanId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    
    WHERE RelationTypeId = 1
    
    UNION ALL
    
    SELECT TP.Id, IsVariantOf.ChildId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    JOIN IsVariantOf
    ON IsVariantOf.Id = TPR.ChildPlanId
    
        WHERE RelationTypeId = 1
    
  )
  
SELECT ChildId
FROM IsVariantOf
WHERE IsVariantOf.Id = 4

UNION ALL

VALUES(4)

)
 
AND TPR.RelationTypeId = 2




;




-- TRAINING_FEEDBACKS_3
-- Same as TRAINING_FEEDBACKS_2 but with no Post table - Denormalization
-- DENORMALIZED v ersion

-- Get the feedbacks of all the the plans 

--PERFORMANCE: similar to TRAINING_FEEDBACKS_1 and TRAINING_FEEDBACKS_2

-- NOTE: 






SELECT TP.Id as PlanId, U.Username, U.Id as UserId,
TS.Id, TS.StartDate, TS.EndDate,
TProf.Name as Proficiency, Ph.Name as Phase,
TSF.Comment as FeedbackNote, TSF.Rating as FedbackRating

-- Get child plans (IE the ones which have been sent to trainees)
FROM TrainingPlan TPRoot
JOIN TrainingPlanRelation TPR
ON TPRoot.Id = TPR.ParentPlanId
JOIN TrainingPlan TP
ON TP.Id = TPR.ChildPlanId
JOIN User U
ON TP.OwnerId = U.Id

-- Proficiency
JOIN TrainingSchedule TS
ON TS.TrainingPlanId = TP.Id
LEFT JOIN UserHasProficiency UHP
ON UHP.OwnerId = TPRoot.OwnerId
AND UHP.UserId = U.Id
AND (UHP.StartDate <= TS.Startdate
AND UHP.EndDate >= TS.Startdate
OR UHP.EndDate is null)
LEFT JOIN TrainingProficiency TProf
ON TProf.Id = UHP.ProficiencyId


-- Phase
--LEFT JOIN Post P
--ON P.UserId = U.Id
LEFT JOIN UserPhase UP
ON UP.OwnerId = TP.OwnerId
AND UP.StartDate <= TS.StartDate
AND UP.EndDate >= TS.StartDate
JOIN Phase Ph
ON UP.PhaseId = Ph.Id

-- Feedback
LEFT JOIN TrainingScheduleFeedback TSF
ON TSF.TrainingScheduleId = TS.Id

--WHERE TPRoot.Id IN (4,2,3,5,7,10,11)		-- If too slow use these instead of the Recursive Query
--AND TPR.RelationTypeId = 2



-- Variants of main plan
WHERE TPRoot.Id IN
(
WITH RECURSIVE
  IsVariantOf(Id, ChildId) AS 
  (

    --VALUES(4,4)
    SELECT TP.Id, TPR.ChildPlanId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    
    WHERE RelationTypeId = 1
    
    UNION ALL
    
    SELECT TP.Id, IsVariantOf.ChildId
    FROM TrainingPlan TP
    JOIN TrainingPlanRelation TPR
    ON TP.Id = TPR.ParentPlanId
    JOIN IsVariantOf
    ON IsVariantOf.Id = TPR.ChildPlanId
    
        WHERE RelationTypeId = 1
    
  )
  
SELECT ChildId
FROM IsVariantOf
WHERE IsVariantOf.Id = 4

UNION ALL

VALUES(4)

)
 
AND TPR.RelationTypeId = 2


