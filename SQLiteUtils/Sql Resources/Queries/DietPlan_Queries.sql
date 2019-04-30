


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
DPTemp.CaloriesPlanned, DDT.Name, Kg / 10.0 AS Weight,
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


-- DIET_PHASE_0

-- Caloric Intake Vs Planned Calories according to the DietDay on the current Phase + Other FitnessDayEntries 
-- PHASE
-- SUBQUERY

-- Same notes of -- DIET_PERIOD_0 hold

SELECT Date(DayDate, 'unixepoch') AS Day,
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


-- DIET_DELTA_0

-- Weekly Caloric difference Plan Vs Intake

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
	SELECT --Date(1553040000, 'unixepoch') as Ref1, Date(1553645800, 'unixepoch') as Ref2,
	--Date(StartDate, 'unixepoch') as StartDt, Date(EndDate, 'unixepoch') as EndDt, StartDate, EndDate, DietDayTypeId as TypeId
	DietDayTypeId, 4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloriesPlanned
	--SUM(4*(CarbGrams + ProteinGrams) + 9 * FatGrams) / COUNT(DPU.Id) AS CaloriesPlanned,
	--julianday(date(EndDate,'unixepoch')) - julianday(date(1553644800,'unixepoch')) ,
	--julianday(date(StartDate,'unixepoch')) - julianday(date(1553040000,'unixepoch')),

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
	AND DPU.EndDate > 1553040000
	--AND DPU.EndDate > 1553644800 - 1000
	--GROUP BY DPU.Id
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate < DPTemp.EndDate
AND DD.DietDayTypeId = DPTemp.TypeId

WHERE P.UserId = 12
AND F.DayDate < 1553645800
AND F.DayDate > 1553040000





;



-- WEIGHT_WEEKLY_AVG_0

-- Weekly average weight among different weeks


SELECT Date(DayDate, 'unixepoch'), avg(Kg) / 10.0 as Weight,
case
    when julianday(date(1553645800,'unixepoch')) - julianday(Date(DayDate, 'unixepoch')) <= 7 then 0
    else 1
end as WeekId    -- to be extended to support multiple weeks
    
-- Get DietDay
FROM Post P
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN Weight W
ON W.Id = P.Id



WHERE P.UserId = 12
AND F.DayDate < 1553645800
AND F.DayDate > strftime('%s', Date(Date(1553645800,'unixepoch'), '-16 day'))		-- to be extended to support multiple weeks

GROUP BY WeekId








;





-- Caloric Intake Vs AVG Planned Calories on a specific period [March 2019 - May 2019]


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

