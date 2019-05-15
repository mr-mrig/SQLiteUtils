

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



;




-- IntensityTechnique (with no link between sets)

insert into SetTemplateIntensityTechnique
(SetTemplateId, IntensityTechniqueId)

SELECT ST.Id, 10 + (ST.Id % 8 = 0)
FROM SetTemplate ST
WHERE ST.Id % 4 = 0
--AND ST.Id < 1000000



 ;



-- Workout Template test setup

UPDATE WorkoutTemplate
SET Name = 'Day D'
WHERE Id IN (2356 + 3, 2360 + 3, 2364 + 3)


UPDATE WorkoutTemplate
SET Name = 'Day E'
WHERE Id =  2364 + 3		-- Week with a different Workout

;



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


-- Reverse function: Intensity to RM

set intensity [lindex $argv 0]

expr { 324.206809067032 - 18.0137586362208 * $intensity + 0.722425494099458 * pow($intensity, 2) - 0.018674659779516 * pow($intensity, 3) + 0.00025787003728422 * pow($intensity, 04) - 1.65095582844966E-06 * pow($intensity, 5) + 2.75225269851 * pow(10,-9) * pow ($intensity, 6) + 8.99097867 * pow(10, -12) * pow($intensity, 7) }





-- 3) To be used on the web app: Mysql stored procedure / function

-- tbd...


;


-- EffortToRpe

--        set effort[lindex $argv 0]
--set effortType[lindex $argv 1]
--set targetReps[lindex $argv 2]

--if {$effortType == 1} {
--      set intensity[expr { $effort / 10.0 }]
--      set rm[expr { 324.206809067032 - 18.0137586362208 * $intensity + 0.722425494099458 * pow($intensity, 2) - 0.018674659779516 * pow($intensity, 3) + 0.00025787003728422 * pow($intensity, 04) - 1.65095582844966E-06 * pow($intensity, 5) + 2.75225269851 * pow(10, -9) * pow($intensity, 6) + 8.99097867 * pow(10, -12) * pow($intensity, 7) }]
--      set val[expr { 10 - ($rm - $targetReps) }]
--      set retval[expr round($val)]

--} elseif {$effortType == 2} {
--   set retval[expr { 10 - ($effort - $targetReps) }]
--} elseif {$effortType == 3} {
--   return $effort
--} else {
--   return 0
--}	

--if {$retval > 4} {
--   return $retval 
--} else {
--   return 4
--}

-- EffortToIntensityPerc

--set effort [lindex $argv 0]
--set effortType [lindex $argv 1]
--set targetReps [lindex $argv 2]

--if {$effortType == 1} {

--   expr { $effort / 10.0 }

--} elseif {$effortType == 2} {

--      set intensity [expr { 0.4167 * $effort - 14.2831 * pow($effort, 0.5) + 115.6122 }]
--      expr round($intensity * 10.0) / 10.0

--} elseif {$effortType == 3} {

--      set param [ expr { $targetReps + (10 - $effort) }]
--      set intensity [expr { 0.4167 * $param - 14.2831 * pow($param, 0.5) + 115.6122 }]
--      expr round($intensity)

--} else {
--   return 0
--}	









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






;







-- TRAINING_PLAN_WEEKS_NUM
-- Get the number of weeks for the training plan


--PERFORMANCE:

-- NOTE: 


SELECT COUNT(1)

FROM TrainingPlan TP
JOIN TrainingWeekTemplate TW
ON TP.Id = TW.TrainingPlanId

WHERE TP.Id = 325




;






-- TRAINING_PLAN_WORKOUTS_IDS_0
-- Get the Workouts of the training plan


--PERFORMANCE:

-- NOTE: All the WO must be fetched. IE: Week1 -> ABC, Week 2 -> ABC -> Week 3 -> ABCD, Week4 -> DEDE. Result -> A,B,C,D,E



SELECT WT.Name, MIN(WT.Id)    

FROM TrainingPlan TP
JOIN TrainingWeekTemplate TW
ON TP.Id = TW.TrainingPlanId
JOIN WorkoutTemplate WT
ON TW.Id = WT.TrainingWeekId

WHERE TP.Id = 325

GROUP BY  WT.Name







;





-- TRAINING_PLAN_WORKOUTS_0
-- Get Workouts full data


--PERFORMANCE:

-- NOTE: Check if WO Ids can be pre-fetched somewhere

-- NOTE: The SetTemplateIntensityTechnique STIT2 is not using the index, furthermore the SUBQUERY is performed on the full STIT2 table even if the WSs of interest are a very small subset.
--		TRAINING_PLAN_WORKOUTS_1 might be faster but requires more server roundtrips: additional investigation is required



SELECT  WT.Id as WorkoutId, WUT.Id as WorkUnitId, ST.Id as SetId,

WT.Name as WorkoutName,WT.IsWeekDaySpecific,

WUTN.Body as WorkUnitNote,

E.Id as ExcerciseId, E.Name as ExcerciseName, E.ImageUrl, E.MuscleId,

ST.ProgressiveNumber as SetNumber, ST.TargetRepetitions, ST.Rest, ST.Cadence, ST.Effort,

ET.Abbreviation as EffortName,

LWUT.FirstWorkUnitId, LWUT.SecondWorkUnitId, LWUT.IntensityTechniqueId, 

IT.Abbreviation as WorkUnitTechnique--, IT2.Abbreviation SetTechnique, STIT.LinkedSetTemplateId as LinkedSet
, TTech.Abbreviation SetTechnique, TTech.LinkedSetTemplateId as LinkedSet


FROM WorkoutTemplate WT
JOIN WorkUnitTemplate WUT
ON WT.Id = WUT.WorkoutTemplateId
LEFT JOIN WorkUnitTemplateNote WUTN
ON WUTN.Id = WUT.WorkUnitTemplateNoteId
JOIN Excercise E
ON E.Id = WUT.ExcerciseId
JOIN SetTemplate ST
ON WUT.Id = ST.WorkUnitId
LEFT JOIN EffortType ET
ON ST.EffortTypeId = ET.Id
LEFT JOIN LinkedWorkUnitTemplate LWUT
ON WUT.Id = LWUT.FirstWorkUnitId
LEFT JOIN IntensityTechnique IT
ON IT.Id = LWUT.IntensityTechniqueId

-- Fetch all the Techniques of the sets
LEFT JOIN
(
    select SetTemplateId, LinkedSetTemplateId, Abbreviation
    FROM SetTemplate ST
    JOIN SetTemplateIntensityTechnique STIT2
    ON STIT2.SetTemplateId = ST.Id
    LEFT JOIN IntensityTechnique IT2
    ON IT2.Id = STIT2.IntensityTechniqueId

) TTech
ON TTech.SetTemplateId = ST.Id

WHERE WT.Id IN
(
    SELECT MIN(WT.Id)    
    
    FROM TrainingPlan TP
    JOIN TrainingWeekTemplate TW
    ON TP.Id = TW.TrainingPlanId
    JOIN WorkoutTemplate WT
    ON TW.Id = WT.TrainingWeekId
    
    WHERE TP.Id = 325
    
    GROUP BY  WT.Name
)


ORDER BY WT.Id, WT.ProgressiveNumber, WUT.Id, ST.ProgressiveNumber




;







-- TRAINING_PLAN_WORKOUTS_1
-- Get Workouts full data


--PERFORMANCE: 1000x faster than TRAINING_PLAN_WORKOUTS_0

-- NOTE: The SetTemplateIntensityTechnique STIT2 uses the index, but requires more roundtrips than TRAINING_PLAN_WORKOUTS_0.
--		By using the index this query is way faster than the previous one, but may require more bandwith which might be an issue when remote DB.
--		The optimal solution would be to force TRAINING_PLAN_WORKOUTS_0 to use the index...


SELECT  WT.Id as WorkoutId, WUT.Id as WorkUnitId, ST.Id as SetId,

WT.Name as WorkoutName,WT.IsWeekDaySpecific,

WUTN.Body as WorkUnitNote,

E.Id as ExcerciseId, E.Name as ExcerciseName, E.ImageUrl, E.MuscleId,

ST.ProgressiveNumber as SetNumber, ST.TargetRepetitions, ST.Rest, ST.Cadence, ST.Effort,

ET.Abbreviation as EffortName,

LWUT.FirstWorkUnitId, LWUT.SecondWorkUnitId, LWUT.IntensityTechniqueId, 

IT.Abbreviation as WorkUnitTechnique


FROM WorkoutTemplate WT
JOIN WorkUnitTemplate WUT
ON WT.Id = WUT.WorkoutTemplateId
LEFT JOIN WorkUnitTemplateNote WUTN
ON WUTN.Id = WUT.WorkUnitTemplateNoteId
JOIN Excercise E
ON E.Id = WUT.ExcerciseId
JOIN SetTemplate ST
ON WUT.Id = ST.WorkUnitId
LEFT JOIN EffortType ET
ON ST.EffortTypeId = ET.Id
LEFT JOIN LinkedWorkUnitTemplate LWUT
ON WUT.Id = LWUT.FirstWorkUnitId
LEFT JOIN IntensityTechnique IT
ON IT.Id = LWUT.IntensityTechniqueId

WHERE WT.Id IN
(
    SELECT MIN(WT.Id)    
    
    FROM TrainingPlan TP
    JOIN TrainingWeekTemplate TW
    ON TP.Id = TW.TrainingPlanId
    JOIN WorkoutTemplate WT
    ON TW.Id = WT.TrainingWeekId
    
    WHERE TP.Id = 325
    
    GROUP BY  WT.Name
)


ORDER BY WT.Id, WT.ProgressiveNumber, WUT.Id, ST.ProgressiveNumber



;


-- Query 2
select SetTemplateId, LinkedSetTemplateId, Abbreviation
FROM SetTemplate ST
JOIN SetTemplateIntensityTechnique STIT2
ON STIT2.SetTemplateId = ST.Id
LEFT JOIN IntensityTechnique IT2
ON IT2.Id = STIT2.IntensityTechniqueId

WHERE SetTemplateId IN
(
91634,91635,91636,91637,91638,91639,91640,91641,91642,91643,91644,91645,91646
,91647,91648,91649,91650,91651,91652,91653,91654,91655,91656,91657,91658,91659
,91660,91661,91662,91663,91664,91665,91666,91667,91668,91669,91670,91671,91672
,91673,91674,91675,91676,91677,91678,91679,91680,91681,91682,91683,91684,91685
,91686,91687,91688,91689,91690,91691,91692,91693,91694,91695,91696,91697,91698
,91699,91700,91701,91702,91703,91704,91705,91706,91707,91708,91709,91710,91711
,91712,91713,91714,91715,91716,91717,91718,91719,91720,91721,91722,91723,91724
,91725,91726,91727,91728,91729,91730,91731,91732,91733,91734,91735,91736,91737
,91738,91739,91740,91741,91742,91743,91744,91745,91746,91747,91748,91749,91750
,91751,91752,91753,91754,91755,91756,91757,91758,91759,91760,91761,91762,91763
,91764,91765,91766,91767,91768,91769,91770,91771,91772,91773,91774,91775,91776
,91777,91778,91779,91780,91781,91782,91783,91784,91785,91786,91787,91788,91789
,92087,92088,92089,92090,92091,92092,92093,92094,92095,92096,92097,92098,92099
,92100,92101,92102,92103,92104,92105,92106,92107,92108,92109,92110,92111,92112
,92113,92114,92115,92116,92117,92118,92119,92120,92121,92122,92123,92124,92125
)







;







-- TRAINING_CURRENT_PLAN_0
-- Get the current plan

-- NOTE: The current plan Id and current Week Id should be stored somewhere
--		The best solution is likely to be a in a specific table, so there will be one entry for each user instead of one per training schedule (all NULLs except for the current schedule)
--		IE: 'UserCurrentTraining' (UserId, CurrentPlanId, CurrentWeekId, CurrentWorkoutId)



SELECT *
    
FROM TrainingPlan TP
JOIN TrainingWeekTemplate TW
ON TP.Id = TW.TrainingPlanId
JOIN WorkoutTemplate WT
ON TW.Id = WT.TrainingWeekId

WHERE TP.Id = 325		-- Fetched from current plan
AND TW.Id = 597			-- Fetched from current Week






;







-- WORKOUT_TRAINING_PARAMETERS_0
-- Get Volume, Intensity, Density of the specific WO


--PERFORMANCE:

-- NOTE:


SELECT WT.Name as WorkoutName, WT.Id as WorkoutId,

Count(ST.Id) as WorkingSets, Round(Avg(TargetRepetitions), 1) as AvgReps,
Round(Avg(Rest), 0) as AvgRest,
Sum
(
CASE 
    WHEN Cadence IS NULL THEN Coalesce(ST.TargetRepetitions * 3, 0)    -- Assume 102 TUT 
    ELSE Coalesce(Cast(Substr(Cadence, 1, 1) + Substr(Cadence, 2, 1) + Substr(Cadence, 3, 1) + Substr(Cadence, 4, 1) as int) * TargetRepetitions, 0)
END
) as TrainingTime,
Round(Sum(Rest), 0) as TotalRest,        -- Total time = TrainingTime + TotalRest
Round(Avg(EffortToIntensityPerc(Effort, EffortTypeId, TargetRepetitions)), 1) as AvgIntensityPerc,
Round(Avg(EffortToRpe(Effort, EffortTypeId, TargetRepetitions)), 1) as AvgRpe


FROM WorkoutTemplate WT
JOIN WorkUnitTemplate WUT
ON WT.Id = WUT.WorkoutTemplateId
JOIN Excercise E
ON E.Id = WUT.ExcerciseId
JOIN SetTemplate ST
ON WUT.Id = ST.WorkUnitId
LEFT JOIN EffortType ET
ON ST.EffortTypeId = ET.Id


WHERE WT.Id = 2356

GROUP BY WT.Id






;




-- WORKOUT_PARAMETERS_TOTAL_WEEK_0
-- Get Workout Training Parameters Vs Weekly Total Parameters


--PERFORMANCE:

-- NOTE: If Plan total Avg parameters are needed, then they have already been fetched from TRAINING_PLAN_USER_1, at least in the Training Plan App section
--		If Week total parameters are needed, then this query is mandatory



SELECT WT.Name as WorkoutName, WT.Id as AggregationId,

Count(ST.Id) as WorkingSets, Round(Avg(TargetRepetitions),1) as AvgReps,
Round(Avg(Rest), 0) as AvgRest,
Sum
(
CASE 
    WHEN Cadence IS NULL THEN Coalesce(ST.TargetRepetitions * 3, 0)    -- Assume 102 TUT 
    ELSE Coalesce(Cast(Substr(Cadence, 1, 1) + Substr(Cadence, 2, 1) + Substr(Cadence, 3, 1) + Substr(Cadence, 4, 1) as int) * TargetRepetitions, 0)
END
) as TrainingTime,
Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '1'
    ELSE Substr(Cadence, 1, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '0'
    ELSE Substr(Cadence, 2, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '2'
    ELSE Substr(Cadence, 3, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '0'
    ELSE Substr(Cadence, 4, 1)
END), 0) as int) as AvgTut,    -- This is different from TrainingTime / AvgWS / AvgReps
Round(Sum(Rest), 0) as TotalRest,        -- Total time = TrainingTime + TotalRest
Round(Avg
(
CASE
    WHEN EffortTypeId = 1 THEN Effort / 10.0     -- Intensity [%]
    WHEN EffortTypeId = 2 THEN Round(RmToIntensityPerc(Effort), 1)    -- RM
    WHEN EffortTypeId = 3 THEN Round(RmToIntensityPerc(TargetRepetitions + (10 - Effort)), 1)      -- RPE
    ELSE null
END
), 1) as AvgIntensityPerc,
Round(Avg(EffortToRpe(Effort, EffortTypeId, TargetRepetitions)), 1) as AvgRpe,
Count(STIT.SetTemplateId) + Count(LWUT.FirstWorkUnitId) as IntensitYTechniqueCounter

FROM WorkoutTemplate WT
JOIN WorkUnitTemplate WUT
ON WT.Id = WUT.WorkoutTemplateId
JOIN Excercise E
ON E.Id = WUT.ExcerciseId
JOIN SetTemplate ST
ON WUT.Id = ST.WorkUnitId
LEFT JOIN EffortType ET
ON ST.EffortTypeId = ET.Id
LEFT JOIN SetTemplateIntensityTechnique STIT
ON STIT.SetTemplateId = ST.Id
LEFT JOIN LinkedWorkUnitTemplate LWUT
ON WUT.Id = LWUT.FirstWorkUnitId

WHERE WT.Id = 2356		-- Insert WorkoutId here




UNION ALL



SELECT 'WeekTotal', TW.Id,

Count(ST.Id) as WorkingSets, Round(Avg(TargetRepetitions), 1) as AvgReps,
Round(Avg(Rest), 0) as AvgRest,
Sum
(
CASE 
    WHEN Cadence IS NULL THEN Coalesce(ST.TargetRepetitions * 3, 0)    -- Assume 102 TUT 
    ELSE Coalesce(Cast(Substr(Cadence, 1, 1) + Substr(Cadence, 2, 1) + Substr(Cadence, 3, 1) + Substr(Cadence, 4, 1) as int) * TargetRepetitions, 0)
END
) as TrainingTime,
Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '1'
    ELSE Substr(Cadence, 1, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '0'
    ELSE Substr(Cadence, 2, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '2'
    ELSE Substr(Cadence, 3, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '0'
    ELSE Substr(Cadence, 4, 1)
END), 0) as int) as AvgTut,    -- This is different from TrainingTime / AvgWS / AvgReps
Round(Sum(Rest), 0) as TotalRest,        -- Total time = TrainingTime + TotalRest
Round(Avg(EffortToIntensityPerc(Effort, EffortTypeId, TargetRepetitions)), 1) as AvgIntensityPerc,
Round(Avg(EffortToRpe(Effort, EffortTypeId, TargetRepetitions)), 1) as AvgRpe,
Count(STIT.SetTemplateId) + Count(LWUT.FirstWorkUnitId) as IntensitYTechniqueCounter

FROM TrainingWeekTemplate TW
JOIN WorkoutTemplate WT
ON TW.Id = WT.TrainingWeekId
JOIN WorkUnitTemplate WUT
ON WT.Id = WUT.WorkoutTemplateId
JOIN Excercise E  
ON E.Id = WUT.ExcerciseId
JOIN SetTemplate ST
ON WUT.Id = ST.WorkUnitId
LEFT JOIN EffortType ET
ON ST.EffortTypeId = ET.Id
LEFT JOIN SetTemplateIntensityTechnique STIT
ON STIT.SetTemplateId = ST.Id
LEFT JOIN LinkedWorkUnitTemplate LWUT
ON WUT.Id = LWUT.FirstWorkUnitId


WHERE TW.Id = 596







;







-- WORKOUT_PARAMETERS_TOTAL_DAY_0
-- Get Training parameters progression over the weeks for the Workouts Days


--PERFORMANCE:

-- NOTE:



SELECT WT.Name as WorkoutName, WT.Id as AggregationId, TW.ProgressiveNumber as WeekNumber,

Count(ST.Id) as WorkingSets, Round(Avg(TargetRepetitions),1) as AvgReps,
Round(Avg(Rest), 0) as AvgRest,
Sum
(
CASE 
    WHEN Cadence IS NULL THEN Coalesce(ST.TargetRepetitions * 3, 0)    -- Assume 102 TUT 
    ELSE Coalesce(Cast(Substr(Cadence, 1, 1) + Substr(Cadence, 2, 1) + Substr(Cadence, 3, 1) + Substr(Cadence, 4, 1) as int) * TargetRepetitions, 0)
END
) as TrainingTime,
Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '1'
    ELSE Substr(Cadence, 1, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '0'
    ELSE Substr(Cadence, 2, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '2'
    ELSE Substr(Cadence, 3, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '0'
    ELSE Substr(Cadence, 4, 1)
END), 0) as int) as AvgTut,    -- This is different from TrainingTime / AvgWS / AvgReps
Round(Sum(Rest), 0) as TotalRest,        -- Total time = TrainingTime + TotalRest
Round(Avg
(
CASE
    WHEN EffortTypeId = 1 THEN Effort / 10.0     -- Intensity [%]
    WHEN EffortTypeId = 2 THEN Round(RmToIntensityPerc(Effort), 1)    -- RM
    WHEN EffortTypeId = 3 THEN Round(RmToIntensityPerc(TargetRepetitions + (10 - Effort)), 1)      -- RPE
    ELSE null
END
), 1) as AvgIntensityPerc,
Round(Avg(EffortToRpe(Effort, EffortTypeId, TargetRepetitions)), 1) as AvgRpe,
Count(STIT.SetTemplateId) + Count(LWUT.FirstWorkUnitId) as IntensitYTechniqueCounter


FROM TrainingPlan TP
JOIN TrainingWeekTemplate TW
ON TP.Id = TW.TrainingPlanId
JOIN WorkoutTemplate WT
ON TW.Id = WT.TrainingWeekId
JOIN WorkUnitTemplate WUT
ON WT.Id = WUT.WorkoutTemplateId
JOIN Excercise E
ON E.Id = WUT.ExcerciseId
JOIN SetTemplate ST
ON WUT.Id = ST.WorkUnitId
LEFT JOIN EffortType ET
ON ST.EffortTypeId = ET.Id
LEFT JOIN LinkedWorkUnitTemplate LWUT
ON WUT.Id = LWUT.FirstWorkUnitId
LEFT JOIN SetTemplateIntensityTechnique STIT
ON STIT.SetTemplateId = ST.Id

-- WeeksId should have already been fetched, making the TP/TW Joins redundant
WHERE TP.Id = 325
-- AND WT.Name = 'Day A'			-- Filter for a specific Workout here
-- GROUP TW.ProgressiveNumber

GROUP BY WT.Name, TW.ProgressiveNumber


;








-- WORKOUT_PARAMETERS_MUSCLE_0
-- Get Training parameters progression over the weeks for all the Muscle groups


--PERFORMANCE:

-- NOTE:



SELECT M.Name as MuscleName, M.Id as AggregationId, TW.ProgressiveNumber as WeekNumber, TW.Id,

Count(ST.Id) as WorkingSets, Round(Avg(TargetRepetitions),1) as AvgReps,
Round(Avg(Rest), 0) as AvgRest,
Sum
(
CASE 
    WHEN Cadence IS NULL THEN Coalesce(ST.TargetRepetitions * 3, 0)    -- Assume 102 TUT 
    ELSE Coalesce(Cast(Substr(Cadence, 1, 1) + Substr(Cadence, 2, 1) + Substr(Cadence, 3, 1) + Substr(Cadence, 4, 1) as int) * TargetRepetitions, 0)
END
) as TrainingTime,
Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '1'
    ELSE Substr(Cadence, 1, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '0'
    ELSE Substr(Cadence, 2, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '2'
    ELSE Substr(Cadence, 3, 1)
END), 0) as int)    -- 'X' will be casted to 0
|| Cast(Round(Avg(
CASE
    WHEN Cadence IS NULL THEN '0'
    ELSE Substr(Cadence, 4, 1)
END), 0) as int) as AvgTut,    -- This is different from TrainingTime / AvgWS / AvgReps
Round(Sum(Rest), 0) as TotalRest,        -- Total time = TrainingTime + TotalRest
Round(Avg
(
CASE
    WHEN EffortTypeId = 1 THEN Effort / 10.0     -- Intensity [%]
    WHEN EffortTypeId = 2 THEN Round(RmToIntensityPerc(Effort), 1)    -- RM
    WHEN EffortTypeId = 3 THEN Round(RmToIntensityPerc(TargetRepetitions + (10 - Effort)), 1)      -- RPE
    ELSE null
END
), 1) as AvgIntensityPerc,
Round(Avg(EffortToRpe(Effort, EffortTypeId, TargetRepetitions)), 1) as AvgRpe,
Count(STIT.SetTemplateId) + Count(LWUT.FirstWorkUnitId) as IntensitYTechniqueCounter


FROM TrainingPlan TP
JOIN TrainingWeekTemplate TW
ON TP.Id = TW.TrainingPlanId
JOIN WorkoutTemplate WT
ON TW.Id = WT.TrainingWeekId
JOIN WorkUnitTemplate WUT
ON WT.Id = WUT.WorkoutTemplateId

JOIN Excercise E
ON E.Id = WUT.ExcerciseId
JOIN Muscle M
ON M.Id = E.MuscleId
JOIN SetTemplate ST
ON WUT.Id = ST.WorkUnitId
LEFT JOIN EffortType ET
ON ST.EffortTypeId = ET.Id
LEFT JOIN LinkedWorkUnitTemplate LWUT
ON WUT.Id = LWUT.FirstWorkUnitId
LEFT JOIN SetTemplateIntensityTechnique STIT
ON STIT.SetTemplateId = ST.Id

-- WeeksId should have already been fetched, making the TP/TW Joins redundant
WHERE TP.Id = 325

GROUP BY M.Id, TW.ProgressiveNumber

ORDER BY M.Id




;






-- WORKOUT_PARAMETERS_MUSCLE_TOTAL_0
-- Get Avg Training parameters over the weeks for all the Muscle groups


--PERFORMANCE:

-- NOTE:


SELECT MuscleName, Round(Avg(WorkingSets)) -- And all other fields
FROM
(
	WORKOUT_PARAMETERS_MUSCLE_0
)


GROUP BY AggregationId






;









-- TRAINING_WORKLOAD_TREND
-- Get the workload over the last 4 weeks


--PERFORMANCE:

-- NOTE:











