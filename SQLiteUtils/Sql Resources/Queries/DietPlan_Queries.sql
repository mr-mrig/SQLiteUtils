



-- TODO LIST


--	DietPlanUnit: Wrong index -> Add DietPlanId, StartDate -> For queries on periods
--	DietPlanUnit: New filtered index -> (DietPlanId, EndDate is null) -> For queries on current diet plan. This can be removed if storing  the current plan somewhere
--  FitnessDayEntry: Wrong index -> New index needed for benchmark: (DayDate, UserId). This must be removed if no denormalization
--  UserPhase: Wrong index -> Add FKs and StartDate
--  Post: New index needed -> Index for UserId, CreatedOn

-- 06/05/2019
-- Image: Add index -> PostId (for Post queries)
-- Image Add index -> CreatedOn, IsProgressPicture, PostId (for Image search queries)
-- UserLiked: Add index -> PostId, UserId

--	10/05/2019
-- TrainingPlanRelation: it might be wise to index for RelationTypeId (used in TRAINING_VARIANTS_RECURSIVE)
-- TrainingPlan: Add index -> (OwnerId, NoteId) - IsBookmarked shouldn't be necessary as the ORDER BY should always work on very few records (however worth to be investigated)

--	10/05/2019
-- WorkoutTemplate: it might be wise to add an index including Name (used when fetching all the WOs of the Plan - TRAINING_PLAN_WORKOUTS_0)

-- 14/05/201
-- UserCurrentTraining table: probabily the best place to store the ongoing training (see ERD diagram)

-- 16/05/201
-- WorkUnit.QuickRating: Constraint should be changed -> BETWEEN 0 AND 2 (Difficult - Normal - Easy)
-- PerformanceType: new table
-- PersonalRecord: new table
-- Excercise: new column PerformanceTypeId - FK


;



-- DIET_PERIOD_0


-- Caloric Intake Vs Planned Calories according to the DietDay on a specific period  + Other FitnessDayEntries
-- PERIOD
-- SUBQUERY

-- PERFORMANCE: This query is faster than the following one. [[This is due to the filters on the Subquery which reduce the row scanned on the Post table
-- As a matter of fact, should the filters be removed, the query becomes slower than the other one (yet, by a small margin)] --> Removing the filters cause wrong results!!]

-- NOTE: If DietDay.Type = null, then CaloriesPlan = NULL (which probably is the correct behavior).
--       -> If a DietDay is added without its type, then the caloric target is not shown to the user

-- NOTE2: Pay attention to StartDate/EndDate boundaries. The current query works if EndDate(k) = StartDate(k+1) - 1. 
--			Benchmark data doesn't ensure this, hence it must be carefully tested according to the start/end strategy.

SELECT Date(DayDate, 'unixepoch') AS Day,
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake,
DPTemp.CaloriesPlanned, DPTemp.DietWeekNumber, DDT.Name, Kg / 10.0 AS Weight,
A.CaloriesOut, A.Steps, WD.Temperature, P.Caption
FROM Post P
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
-- The following might be exported to a view
LEFT JOIN
(
	SELECT StartDate, EndDate, DDT.Id AS TypeId,
	4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned,
	cast((julianday(date(enddate, 'unixepoch')) - julianday(date(startdate, 'unixepoch'))) / 7 as int) + 1 as DietWeekNumber
	FROM Post P
	JOIN DietPlan DP
	ON P.Id = DP.Id
	JOIN DietPlanUnit DPU
	ON DP.Id = DPU.DietPlanId
	JOIN DietPlanDay DPD
	ON DPU.Id = DPD.DietPlanUnitId
	LEFT JOIN DietDayType DDT
	ON DPD.DietDayTypeId = DDT.Id
	LEFT JOIN DietHashtag DH						-- Whatch out
	ON DH.Id = DP.DietHashtagId
	WHERE P.UserId = 12
	AND DPU.StartDate BETWEEN 1514764800 AND 1546128000
	AND DH.IsApproved IN (True, Null)				-- Watch out
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate < DPTemp.EndDate				-- Pay attention to the boundaries. Query might change wether Start/End dates are overlapping or not.
AND DD.DietDayTypeId = DPTemp.TypeId
-- FitnessDay data
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id
WHERE P.UserId = 12
--AND P.CreatedOn BETWEEN 1551744000 AND 1554162000 -- March 2019
AND P.CreatedOn BETWEEN 1514764800 AND 1546128000 -- Whole 2018
ORDER BY P.CreatedOn


;



-- DIET_PERIOD_1


-- Caloric Intake Vs Planned Calories according to the DietDay on a specific period  + Other FitnessDayEntries
-- PERIOD
-- JOINs

-- PERFORMANCE: This query is (100x) slower than the previous one, because the P1 Post table is unfiltered
--				0.01s

-- NOTE: If DietDay.Type = null, then multiple rows are fetched, one for each DayType of the ongoing diet plan. The behavior is not correct, unless the average target cal are supposed to be shown (post-processing).
--       -> If a DietDay is added without its type, and the plan has ON and OFF days, then two rows are fetched with the target calories for the ON and OFF days

-- NOTE2: Pay attention to StartDate/EndDate boundaries. The current query works if EndDate(k) = StartDate(k+1) - 1. 
--			Benchmark data doesn't ensure this, hence it must be carefully tested according to the start/end strategy.

SELECT Date(F.DayDate, 'unixepoch') AS Day, 
4*(DD.CarbGrams + DD.ProteinGrams) + 9 * DD.FatGrams AS CaloricIntake, 
4*(DPD.CarbGrams + DPD.ProteinGrams) + 9 * DPD.FatGrams AS CaloriesPlanned, 
DDT.Name, Kg / 10.0 AS Weight,
A.CaloriesOut, A.Steps, WD.Temperature, P.Caption

-- Get Intake    
FROM Post P
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id


-- Get Plan
LEFT JOIN Post P1
ON P.UserId = P1.UserId
JOIN DietPlan DP
ON P1.Id = DP.Id
JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
AND F.DayDate >= DPU.StartDate
AND F.DayDate < DPU.EndDate				-- Pay attention to the boundaries. Query might change wether Start/End dates are overlapping or not.
JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
AND (DD.DietDayTypeId = DPD.DietDayTypeId
 OR DD.DietDayTypeId IS NULL)				-- Otherwise NULL DateDay.Types wouldn't be fetched

-- Get other data
LEFT JOIN Weight W
ON F.Id = W.Id 
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id
WHERE P.UserId = 12
--AND P.CreatedOn BETWEEN 1551744000 AND 1554162000    -- March 2019
AND P.CreatedOn  BETWEEN 1514764800 AND 1546128000  -- Whole 2018
--AND DPU.StartDate BETWEEN 1514764800 AND 1546128000

ORDER BY P.CreatedOn



;




-- DIET_PERIOD_3 -> Same results of DIET_PERIOD_0 on Denormalized tables


-- Caloric Intake Vs Planned Calories according to the DietDay on a specific period  + Other FitnessDayEntries
-- PERIOD
-- SUBQUERY

-- PERFORMANCE: This query is 2x faster than the DIET_PERIOD_0 - Normalized version. 
--				Denormalization is achieved by adding a UserId column on FitnessDayEntry and exploiting the OwnerId field in DietPlan (yet, this is not correct in as OwnerId has a different meaning from UserId and must be changed if going to production)
--				0.2s

-- NOTE2: Pay attention to StartDate/EndDate boundaries. The current query works if EndDate(k) = StartDate(k+1) - 1. 
--			Benchmark data doesn't ensure this, hence it must be carefully tested according to the start/end strategy.


SELECT Date(DayDate, 'unixepoch') AS Day,
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake,
DPTemp.CaloriesPlanned, DDT.Name, Kg / 10.0 AS Weight,
A.CaloriesOut, A.Steps, WD.Temperature
FROM FitnessDayEntry F
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
-- The following might be exported to a view
LEFT JOIN
(
	SELECT StartDate, EndDate, DDT.Id AS TypeId,
	4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned
	FROM DietPlan DP
	JOIN DietPlanUnit DPU
	ON DP.Id = DPU.DietPlanId
	JOIN DietPlanDay DPD
	ON DPU.Id = DPD.DietPlanUnitId
	LEFT JOIN DietDayType DDT
	ON DPD.DietDayTypeId = DDT.Id
	WHERE DP.OwnerId = 12
	AND DPU.StartDate BETWEEN 1514764800 AND 1546128000
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate < DPTemp.EndDate				-- Pay attention to the boundaries. Query might change wether Start/End dates are overlapping or not.
AND DD.DietDayTypeId = DPTemp.TypeId
-- FitnessDay data
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id
WHERE F.UserId = 12
--AND P.CreatedOn BETWEEN 1551744000 AND 1554162000 -- March 2019
AND F.DayDate BETWEEN 1514764800 AND 1546128000 -- Whole 2018
ORDER BY F.DayDate




;


-- DIET_PHASE_0

-- Caloric Intake Vs Planned Calories according to the DietDay on the current Phase + Other FitnessDayEntries 
-- PHASE
-- SUBQUERY

-- Same notes of -- DIET_PERIOD_0 hold

-- PERFORMANCE: doesnt finish


SELECT Date(DayDate, 'unixepoch') AS Day, DietWeekNumber,
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake,
DPTemp.CaloriesPlanned, DDT.Name, Kg / 10.0 AS Weight,
A.CaloriesOut, A.Steps, WD.Temperature, P.Caption

-- Get Phase
FROM Post P1
JOIN UserPhase UP
ON P1.Id = UP.Id
JOIN Phase PH
ON PH.Id = UP.PhaseId

-- Get DietDay
JOIN Post P
ON P.CreatedOn BETWEEN UP.StartDate AND UP.EndDate
AND P.UserId = P1.UserId
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id

-- The following might be exported to a view
LEFT JOIN
(
	SELECT StartDate, EndDate, DDT.Id AS TypeId,
	4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned,
	cast((julianday(date(enddate, 'unixepoch')) - julianday(date(startdate, 'unixepoch'))) / 7 as int) + 1 as DietWeekNumber
	FROM Post P
	JOIN DietPlan DP
	ON P.Id = DP.Id
	JOIN DietPlanUnit DPU
	ON DP.Id = DPU.DietPlanId
	JOIN DietPlanDay DPD
	ON DPU.Id = DPD.DietPlanUnitId
	LEFT JOIN DietDayType DDT
	ON DPD.DietDayTypeId = DDT.Id
	WHERE P.UserId = 12
	-- AND 	AND DPU.StartDate BETWEEN 1514764800 AND 1546128000				-- Adding the filter leads to a -5/-10% timing decrease but requires additional input
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate < DPTemp.EndDate				-- Pay attention to the boundaries. Query might change wether Start/End dates are overlapping or not.
AND DD.DietDayTypeId = DPTemp.TypeId

-- Get FitnessDay data
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id
WHERE P1.UserId = 12
AND UP.EndDate >= strftime('%s',current_timestamp)

ORDER BY P.CreatedOn


;


-- DIET_PHASE_1

-- Caloric Intake Vs Planned Calories according to the DietDay on the current Phase + Other FitnessDayEntries 
-- PHASE
-- JOINs

-- Same notes of -- DIET_PERIOD_1 hold

-- PERFORMANCE: doesnt finish 


SELECT Date(DayDate, 'unixepoch') AS Day, Ph.Name,
4*(DD.CarbGrams + DD.ProteinGrams) + 9 * DD.FatGrams AS CaloricIntake,
4*(DPD.CarbGrams + DPD.ProteinGrams) + 9 * DPD.FatGrams AS CaloricIntake,
DDT.Name, Kg / 10.0 AS Weight, A.CaloriesOut, A.Steps, WD.Temperature, P.Caption

-- Get Phase
FROM Post P1
JOIN UserPhase UP
ON P1.Id = UP.Id
JOIN Phase PH
ON PH.Id = UP.PhaseId

-- Get DietDay
JOIN Post P
ON P.CreatedOn BETWEEN UP.StartDate AND UP.EndDate
AND P.UserId = P1.UserId
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id

-- Get Plan
LEFT JOIN Post P2
ON P.UserId = P2.UserId
JOIN DietPlan DP
ON P2.Id = DP.Id
JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
AND F.DayDate >= DPU.StartDate
AND F.DayDate < DPU.EndDate				-- Pay attention to the boundaries. Query might change wether Start/End dates are overlapping or not.
JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
AND (DD.DietDayTypeId = DPD.DietDayTypeId
 OR DD.DietDayTypeId IS NULL)				-- Otherwise NULL DateDay.Types wouldn't be fetched
 
-- Get FitnessDay data
LEFT JOIN Weight W
ON F.Id = W.Id 
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id
WHERE P1.UserId = 12
AND UP.EndDate >= strftime('%s',current_timestamp)

ORDER BY P.CreatedOn



;


-- DIET_PHASE_2

-- Caloric Intake Vs Planned Calories according to the DietDay on the current Phase + Other FitnessDayEntries 
-- PHASE
-- SUBQUERY + SUBQUERY

-- PERFORMANCE: 20x faster than DIET_PHASE_0, 23x faster than DIET_PHASE_1
--				0,01s


SELECT Date(FTemp.DayDate, 'unixepoch') AS Day,
DietWeekNumber, CaloricIntake,
DPTemp.CaloriesPlanned, Ftemp.Name, Kg / 10.0 AS Weight,
CaloriesOut, Steps, Temperature, Ftemp.Caption


FROM Post P
JOIN UserPhase UP
ON P.Id = UP.Id
JOIN Phase PH
ON PH.Id = UP.PhaseId

JOIN
(
SELECT F.Id, F.DayDate, DDT.Name, CreatedOn, P.UserId, DietDayTypeId, W.Kg,
CaloriesOut, Steps, Temperature, Caption,
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake
FROM Post P
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

WHERE P.UserId = 12
) FTemp
ON FTemp.CreatedOn BETWEEN UP.StartDate AND UP.EndDate
AND Ftemp.UserId = P.UserId


LEFT JOIN
(
SELECT StartDate, EndDate, DDT.Id AS TypeId,
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned,
cast((julianday(date(EndDate, 'unixepoch')) - julianday(date(StartDate, 'unixepoch'))) / 7 as int) + 1 as DietWeekNumber
FROM Post P
JOIN DietPlan DP
ON P.Id = DP.Id
JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT
ON DPD.DietDayTypeId = DDT.Id
WHERE P.UserId = 12
--AND DPU.StartDate BETWEEN 1514764800 AND 1546128000
) DPTemp
ON FTemp.DayDate >= DPTemp.StartDate
AND FTemp.DayDate < DPTemp.EndDate
AND FTemp.DietDayTypeId = DPTemp.TypeId


WHERE P.UserId = 12
AND UP.EndDate >= strftime('%s',current_timestamp)

ORDER BY P.CreatedOn




;




-- DIET_PHASE_4 -> DIET_PHASE_2 + Denormalized

-- Caloric Intake Vs Planned Calories according to the DietDay on the current Phase + Other FitnessDayEntries 
-- PHASE
-- SUBQUERY + SUBQUERY

-- PERFORMANCE: Same as DIET_PHASE_2 (needs to be benchmarked on a bigger database)
--				0,01s


SELECT Date(FTemp.DayDate, 'unixepoch') AS Day,
CaloricIntake,
DPTemp.CaloriesPlanned, Ftemp.Name, Kg / 10.0 AS Weight,
CaloriesOut, Steps, Temperature, Ftemp.Caption


FROM Post P
JOIN UserPhase UP
ON P.Id = UP.Id
JOIN Phase PH
ON PH.Id = UP.PhaseId

JOIN
(
SELECT F.Id, F.DayDate, DDT.Name, CreatedOn, P.UserId, DietDayTypeId, W.Kg,
CaloriesOut, Steps, Temperature, Caption,
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake
FROM Post P
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

WHERE P.UserId = 12
) FTemp
ON FTemp.CreatedOn BETWEEN UP.StartDate AND UP.EndDate
AND Ftemp.UserId = P.UserId


LEFT JOIN
(
SELECT StartDate, EndDate, DDT.Id AS TypeId,
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned
FROM Post P
JOIN DietPlan DP
ON P.Id = DP.Id
JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT
ON DPD.DietDayTypeId = DDT.Id
WHERE P.UserId = 12
--AND DPU.StartDate BETWEEN 1514764800 AND 1546128000
) DPTemp
ON FTemp.DayDate >= DPTemp.StartDate
AND FTemp.DayDate < DPTemp.EndDate
AND FTemp.DietDayTypeId = DPTemp.TypeId


WHERE P.UserId = 12
AND UP.EndDate >= strftime('%s',current_timestamp)

ORDER BY P.CreatedOn




;




-- DIET_PHASE_3 -> DIET_PHASE_0 ON Denormalized tables

-- Caloric Intake Vs Planned Calories according to the DietDay on the current Phase + Other FitnessDayEntries 
-- PHASE
-- SUBQUERY + SUBQUERY

-- PERFORMANCE: 30x faster than DIET_PHASE_0, 33x faster than DIET_PHASE_1


SELECT Date(DayDate, 'unixepoch') AS Day,
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake,
DPTemp.CaloriesPlanned, DDT.Name, Kg / 10.0 AS Weight,
A.CaloriesOut, A.Steps, WD.Temperature

-- Get Phase
FROM UserPhase UP
JOIN Phase PH
ON PH.Id = UP.PhaseId

-- Get DietDay
JOIN FitnessDayEntry F
ON F.DayDate BETWEEN UP.StartDate AND UP.EndDate
AND F.UserId = UP.OwnerId
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id

-- The following might be exported to a view
LEFT JOIN
(
 SELECT StartDate, EndDate, DDT.Id AS TypeId,
 4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned
 FROM DietPlan DP
 JOIN DietPlanUnit DPU
 ON DP.Id = DPU.DietPlanId
 JOIN DietPlanDay DPD
 ON DPU.Id = DPD.DietPlanUnitId
 LEFT JOIN DietDayType DDT
 ON DPD.DietDayTypeId = DDT.Id
 WHERE DP.OwnerId = 12
 -- AND  AND DPU.StartDate BETWEEN 1514764800 AND 1546128000    -- Adding the filter leads to a -5/-10% timing decrease but requires additional input
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate < DPTemp.EndDate    -- Pay attention to the boundaries. Query might change wether Start/End dates are overlapping or not.
AND DD.DietDayTypeId = DPTemp.TypeId

-- Get FitnessDay data
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id
WHERE F.UserId = 12
AND UP.EndDate >= strftime('%s',current_timestamp)

ORDER BY F.DayDate





;




-- DIET_DELTA_0

-- Weekly Caloric difference Plan Vs Intake

-- PERFORMANCE: 0,01s


-- To get the current week:

select current_date AS Today, strftime('%s', current_date) AS UnixToday, Date(current_date, '-8 day') as WeekBefore, strftime('%s', Date(current_date, '-8 day')) AS UnixWeekBefore    -- -8 days as the current one is not included




-- Start Query

SELECT cast( Avg(4*(CarbGrams + ProteinGrams) + 9 * FatGrams) as int) as IntakeAvg, Avg(CaloriesPlanned) as PlanAvg

-- Get DietDay
FROM Post P
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id


JOIN
(
	SELECT StartDate, EndDate, DietDayTypeId
	DietDayTypeId, 4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned


	FROM Post P
	JOIN DietPlan DP
	ON P.Id = DP.Id
	JOIN DietPlanUnit DPU
	ON DP.Id = DPU.DietPlanId
	JOIN DietPlanDay DPD
	ON DPU.Id = DPD.DietPlanUnitId
	LEFT JOIN DietDayType DDT
	ON DPD.DietDayTypeId = DDT.Id
	LEFT JOIN DietHashtag DH						-- Whatch out
	ON DH.Id = DP.DietHashtagId
	WHERE P.UserId = 12
	AND DPU.StartDate < 1553645800
	AND DPU.EndDate > 1553040000
	AND DH.IsApproved IN (True, NULL)				-- Whatch out

) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate < DPTemp.EndDate
AND DD.DietDayTypeId = DPTemp.DietDayTypeId

WHERE P.UserId = 12
AND F.DayDate < 1553645800
AND F.DayDate > 1553040000





;





-- FITNESSDAY_LAST_MONTH_0

-- Get the FitnessDayEntry data over the last month. To be used when the user is selected from the trainee list.

-- PERFORMANCE: By denormalizing the tables like DIET_PERIOD_3 the execution gains a 3x decrease in timing




select P.Id, F.Id, DPTemp.Id, Date(DayDate, 'unixepoch'), --date(1553645800, 'unixepoch'), (julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')))
 W.Kg / 10.0, Mus.Name as MusName, AD.Steps, AD.CaloriesOut, AD.Stairs, AD.SleepMinutes, AD.SleepQuality, AD.HeartRateRest, AD.HeartRateMax,
 4*(CarbGrams + ProteinGrams) + 9 * FatGrams as IntakeAvg,CaloriesPlanned as PlanAvg,
 case
    when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 7 then 0
    when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 14 then 1
        when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 21 then 2
        when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 28 then 3
    else 4
end as WeekId    -- to be extended to support multiple weeks

FROM Post P

-- Get FitnessDayEntries
JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN Weight W
ON W.Id = F.Id
LEFT JOIN WellnessDay WD
ON WD.Id = F.Id
LEFT JOIN WellnessDayHasMus WDHM
ON WDHM.WellnessDayId = WD.Id
LEFT JOIN Mus
ON Mus.Id = WDHM.MusId
LEFT JOIN ActivityDay AD
ON AD.Id = F.Id

-- Get Diet
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id

LEFT JOIN
(
	SELECT P.Id, StartDate, EndDate, DietDayTypeId
	DietDayTypeId, 4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned

	FROM Post P
	JOIN DietPlan DP
	ON P.Id = DP.Id
	JOIN DietPlanUnit DPU
	ON DP.Id = DPU.DietPlanId
	JOIN DietPlanDay DPD
	ON DPU.Id = DPD.DietPlanUnitId
	LEFT JOIN DietDayType DDT
	ON DPD.DietDayTypeId = DDT.Id
	LEFT JOIN DietHashtag DH						-- Whatch out
	ON DH.Id = DP.DietHashtagId
	WHERE P.UserId = 12
	AND DPU.StartDate < 1553645800
	AND DPU.EndDate >  strftime('%s', Date(Date(1553645800,'unixepoch'), '-28 day'))
	AND DH.IsApproved IN (True, NULL)				-- Whatch out
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate < DPTemp.EndDate
AND DD.DietDayTypeId = DPTemp.DietDayTypeId


WHERE P.UserId = 12
AND F.DayDate < 1553645800
AND F.DayDate > strftime('%s', Date(Date(1553645800,'unixepoch'), '-28 day'))		-- to be extended to support multiple weeks

order by daydate



;






-- FITNESSDAY_LAST_MONTH_1 -> 2 Subqueries

-- Get the FitnessDayEntry data over the last month. This version might use two views instead of the two subqueries, hence it might be preferable.

-- PERFORMANCE: Same execution time as FITNESSDAY_LAST_MONTH_0! Denormalizing this query leads to the same imporvement aas FITNESSDAY_LAST_MONTH_1




SELECT FTemp.Id as FitId, DPTemp.Id as DietUnitId, Date(FTemp.DayDate, 'unixepoch'), FTemp.*, DPTemp.CaloriesPlanned,

 
CASE
    when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 7 then 0
    when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 14 then 1
        when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 21 then 2
        when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 28 then 3
    else 4
end as WeekId    -- to be extended to support multiple weeks


FROM
(
    SELECT F.Id, F.DayDate, W.Kg / 10.0 as WeightKg, P.UserId,
    Mus.Name as MusName, AD.Steps, AD.CaloriesOut, AD.Stairs, AD.SleepMinutes, AD.SleepQuality, AD.HeartRateRest, AD.HeartRateMax, DD.DietDayTypeId,
     4*(CarbGrams + ProteinGrams) + 9 * FatGrams as IntakeAvg
            
    FROM Post P
    
    LEFT JOIN FitnessDayEntry F
    ON P.Id = F.Id
    LEFT JOIN Weight W
    ON W.Id = F.Id
    LEFT JOIN WellnessDay WD
    ON WD.Id = F.Id
    LEFT JOIN WellnessDayHasMus WDHM
    ON WDHM.WellnessDayId = WD.Id
    LEFT JOIN Mus
    ON Mus.Id = WDHM.MusId
    LEFT JOIN ActivityDay AD
    ON AD.Id = F.Id
    LEFT JOIN DietDay DD
    ON F.Id = DD.Id
    LEFT JOIN DietDayType DDT
    ON DD.DietDayTypeId = DDT.Id
    
    WHERE P.UserId = 12
    AND F.DayDate < 1553645800
    AND F.DayDate > strftime('%s', Date(Date(1553645800,'unixepoch'), '-28 day'))
) FTemp

LEFT JOIN
(
	SELECT P.Id, StartDate, EndDate, DietDayTypeId, P.UserId as UserId,
	DietDayTypeId, 4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned

	FROM Post P
	JOIN DietPlan DP
	ON P.Id = DP.Id
	JOIN DietPlanUnit DPU
	ON DP.Id = DPU.DietPlanId
	JOIN DietPlanDay DPD
	ON DPU.Id = DPD.DietPlanUnitId
	LEFT JOIN DietDayType DDT
	ON DPD.DietDayTypeId = DDT.Id
	WHERE P.UserId = 12
	AND DPU.StartDate < 1553645800
	AND DPU.EndDate >  strftime('%s', Date(Date(1553645800,'unixepoch'), '-28 day'))
) DPTemp
ON FTemp.DayDate >= DPTemp.StartDate
AND FTemp.DayDate < DPTemp.EndDate
AND FTemp.DietDayTypeId = DPTemp.DietDayTypeId


order by FTemp.DayDate



;





-- TDEE_CALCULATION_0

-- PArtial data for TDEE calculation

SELECT --F.Id, Date(F.DayDate, 'unixepoch') as Day, F.DayDate, W.Kg / 10.0 as WeightKg, P.UserId, DD.DietDayTypeId,

CASE
    when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 14 then 0
    when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 28 then 1
        --when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 21 then 2
        --when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 28 then 3
    else 2
end as TwoWeeksId    -- to be extended to support multiple weeks
, AVG(W.Kg) / 10.0 as WeightKg
, CAST(AVG(4*(CarbGrams + ProteinGrams) + 9 * FatGrams) AS int) as IntakeAvg
            
FROM Post P
    
LEFT JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN Weight W
ON W.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id

WHERE P.UserId = 12
AND F.DayDate < 1553645800
AND F.DayDate > strftime('%s', Date(Date(1553645800,'unixepoch'), '-28 day'))
 
GROUP BY TwoWeeksId





;


-- CURRENT_DIET_WEEK

-- Get the current week of the current diet plan Vs the total plan weeks



SELECT date(StartDate, 'unixepoch'), date(EndDate, 'unixepoch'),
 cast((julianday(date(EndDate, 'unixepoch')) - julianday(date(StartDate, 'unixepoch'))) / 7 as int) + 1 as DietWeekNumber,
 cast((julianday(date(EndDatePlanned, 'unixepoch')) - julianday(date(StartDate, 'unixepoch'))) / 7 as int) + 1 as TotalDietWeeks
FROM DietPlan DP
JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId

--WHERE DP.Id = 13114
where ownerid = 12
and DPU.EndDate is null

ORDER BY DPU.StartDate DESC			-- Should be superflous, needed just in case there are some records with EndDate = NULL hich are not the int current diet plan
LIMIT(1)							-- Should be superflous, needed just in case there are some records with EndDate = NULL hich are not the int current diet plan


;



-- CURRENT_PHASE_WEEK

-- Get the current week of the current phase.
-- This is useful when the user wants to know how long a phase has been carried on for (IE: Diet Panel -> Diet Week Number might be the Week number since the phase has changed)



SELECT date(UP.StartDate, 'unixepoch'), date(UP.EndDate, 'unixepoch'),
 cast((julianday(date('now')) - julianday(date(UP.StartDate, 'unixepoch'))) / 7 as int) + 1 as PhaseWeekNumber
FROM Post P
JOIN UserPhase UP
ON P.Id = UP.Id

--WHERE DP.Id = 13114
where ownerid = 12
and UP.EndDate is null

ORDER BY UP.StartDate DESC			-- Should be superflous, needed just in case there are some records with EndDate = NULL hich are not the int current diet plan
LIMIT(1)	


;



-- MEASURES_LAST_MONTHS_0

-- Get the MeasuresEntry data over the last 4 months. To be used when the user is selected from the trainee list.
-- 0,01s


SELECT Date(1553645800, 'unixepoch') as RefDate, Date(MeasureDate, 'unixepoch') as MeasDate, 
    strftime('%m',  Date(MeasureDate, 'unixepoch')) as MeasMonth,
CASE 
    WHEN B.Bf IS NULL THEN AVG(PL.Bf)
    ELSE AVG(B.Bf)
END / 100.0 as BodyFat,

 PL.*, C.*, B.*		-- These measures must be averaged


-- Measures
FROM Post P
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Plicometry PL
ON M.Id = PL.Id
LEFT JOIN Circumference C
ON M.Id = C.Id
LEFT JOIN BiaEntry B
ON M.Id = B.Id

WHERE P.UserId = 12
AND M.MeasureDate < 1553645800
AND M.MeasureDate > strftime('%s', Date(Date(1553645800,'unixepoch'), '-4 month'))		-- to be extended to support multiple weeks

GROUP BY MeasMonth
order by MeasureDate





;





-- WEIGHT_WEEKLY_AVG_0

-- Weekly average weight among different weeks -> Should be made more generic by replacing the Case When with something that detects the weeks
-- 0,01s


select Date(DayDate, 'unixepoch'), date(1553645800, 'unixepoch'), (julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')))
, strftime('%W',  Date(1553645800, 'unixepoch')) - strftime('%W',  Date(DayDate, 'unixepoch')), Kg / 10.0
, case
    when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 7 then 0
    when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 14 then 1
        when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 21 then 2
        when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 28 then 3
    else 4
end as WeekId    -- to be extended to support multiple weeks

FROM Post P
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN Weight W
ON W.Id = P.Id

WHERE P.UserId = 12
AND F.DayDate < 1553645800
AND F.DayDate > strftime('%s', Date(Date(1553645800,'unixepoch'), '-21 day'))		-- to be extended to support multiple weeks

order by daydate








;





-- Caloric Intake Vs AVG Planned Calories on a specific period [March 2019 - May 2019]
-- 0,01s

SELECT Date(DayDate, 'unixepoch') AS Day, 4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake, DPTemp.CaloriesAvg
FROM Post P
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN
(
    SELECT StartDate, EndDate, DietDayTypeId,
        SUM(4*(CarbGrams + ProteinGrams) + 9 * FatGrams) / COUNT(DPU.Id) AS CaloriesAvg,
        MIN(4*(CarbGrams + ProteinGrams) + 9 * FatGrams) AS CaloriesMin,
        MAX(4*(CarbGrams + ProteinGrams) + 9 * FatGrams) AS CaloriesMax
    FROM Post P
    JOIN DietPlan DP
    ON P.Id = DP.Id
    JOIN DietPlanUnit DPU
    ON DP.Id = DPU.DietPlanId
    JOIN DietPlanDay DPD
    ON DPU.Id = DPD.DietPlanUnitId
    LEFT JOIN DietDayType DDT
    ON DPD.DietDayTypeId = DDT.Id
    WHERE P.UserId = 12
    AND DPU.StartDate BETWEEN 1551744000 AND 1554162000
    GROUP BY DPU.Id
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate <= DPTemp.EndDate
WHERE P.UserId = 12
AND P.CreatedOn BETWEEN 1551744000 AND 1554162000
ORDER BY P.CreatedOn
;



-- Daily AVG Caloric Plan [over the week] on specific period [March 2019 - May 2019]
-- 0,01s

SELECT Date(StartDate, 'unixepoch') AS StartDate, Date(EndDate, 'unixepoch') AS EndDate, COUNT(DPU.Id)  AS DaysNumber, Date(P.CreatedOn, 'unixepoch') AS CreatedDt, StartDate, EndDate,
    SUM(4*(CarbGrams + ProteinGrams) + 9 * FatGrams) / COUNT(DPU.Id) AS CaloriesAvg,
    MIN(4*(CarbGrams + ProteinGrams) + 9 * FatGrams) AS CaloriesMin,
    MAX(4*(CarbGrams + ProteinGrams) + 9 * FatGrams) AS CaloriesMax
FROM Post P
JOIN DietPlan DP
ON P.Id = DP.Id
JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT
ON DPD.DietDayTypeId = DDT.Id
WHERE P.UserId = 12
AND DPU.StartDate BETWEEN 1551744000 AND 1554162000
GROUP BY DPU.Id
--ORDER BY P.CreatedOn

;


-- Caloric Days Planned on specific period [March 2019 - May 2019]
-- 0,01s

SELECT Date(StartDate, 'unixepoch') AS StartDt, Date(EndDate, 'unixepoch') AS EndDt, DDT.Name,
    4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned, StartDate, EndDate
FROM Post P
JOIN DietPlan DP
ON P.Id = DP.Id
JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT
ON DPD.DietDayTypeId = DDT.Id
WHERE P.UserId = 12
--AND DPU.StartDate BETWEEN 1515370000 AND 1546645000
--ORDER BY P.CreatedOn






;






