




-- Caloric Intake Vs Planned Calories according to the DietDay on a specific period [March 2019 - May 2019]
-- Might have duplicate rows because of auto populated DB [IE: more than one ON days on the same DietUnitPlan are not prevented]

SELECT Date(DayDate, 'unixepoch') AS Day, 
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake, 
DPTemp.CaloriesPlanned, DDT.Name
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
    AND DPU.StartDate BETWEEN 1551744000 AND 1554162000
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate <= DPTemp.EndDate
AND DD.DietDayTypeId = DPTemp.TypeId
WHERE P.UserId = 12
AND P.CreatedOn BETWEEN 1551744000 AND 1554162000
ORDER BY P.CreatedOn


;



-- Caloric Intake Vs Planned Calories according to the DietDay on a specific period [March 2019 - May 2019] + Weight

SELECT Date(DayDate, 'unixepoch') AS Day, 
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake, 
DPTemp.CaloriesPlanned, DDT.Name, Kg / 10.0 AS Weight
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
    AND DPU.StartDate BETWEEN 1551744000 AND 1554162000
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate <= DPTemp.EndDate
AND DD.DietDayTypeId = DPTemp.TypeId
-- Weight
LEFT JOIN Weight W
ON F.Id = W.Id 
WHERE P.UserId = 12
AND P.CreatedOn BETWEEN 1551744000 AND 1554162000
ORDER BY P.CreatedOn


;



-- Caloric Intake Vs Planned Calories according to the DietDay on a specific period [March 2019 - May 2019] + Other FitnessDayEntries

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
    AND DPU.StartDate BETWEEN 1551744000 AND 1554162000
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate <= DPTemp.EndDate
AND DD.DietDayTypeId = DPTemp.TypeId
-- Weight
LEFT JOIN Weight W
ON F.Id = W.Id 
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id
WHERE P.UserId = 12
AND P.CreatedOn BETWEEN 1551744000 AND 1554162000    -- March 2019
--AND P.CreatedOn AND DPU.StartDate BETWEEN 1515370000 AND 1546645000  -- Whole 2018
ORDER BY P.CreatedOn

;



-- Caloric Intake Vs Planned Calories according to the DietDay on the current Phase + Other FitnessDayEntries 
-- JOINs

SELECT Date(DayDate, 'unixepoch') AS Day, 
4*(CarbGrams + ProteinGrams) + 9 * FatGrams AS CaloricIntake, 
DPTemp.CaloriesPlanned, DDT.Name, Kg / 10.0 AS Weight,
A.CaloriesOut, A.Steps, WD.Temperature, PH.Name, P.Caption
FROM Post P1
JOIN UserPhase UP
ON P1.Id = UP.Id
JOIN Phase PH
ON PH.Id = UP.PhaseId
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
    AND DPU.StartDate BETWEEN 1551744000 AND 1554162000
) DPTemp
ON F.DayDate >= DPTemp.StartDate
AND F.DayDate <= DPTemp.EndDate
AND DD.DietDayTypeId = DPTemp.TypeId
-- Weight
LEFT JOIN Weight W
ON F.Id = W.Id 
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id
WHERE P1.UserId = 12
AND UP.StartDate BETWEEN 1535845000 AND 1535847000
--AND P.CreatedOn AND DPU.StartDate BETWEEN 1515370000 AND 1546645000  -- Whole 2018
ORDER BY P.CreatedOn

;



-- Caloric Intake Vs Planned Calories according to the DietDay on the current Phase + Other FitnessDayEntries 
-- SUBQUERY

SELECT Date(DayDate, 'unixepoch') AS Day, 
4*(DD.CarbGrams + DD.ProteinGrams) + 9 * DD.FatGrams AS CaloricIntake, 
4*(DPD.CarbGrams + DPD.ProteinGrams) + 9 * DPD.FatGrams AS CaloriesPlanned, 
DDT.Name, Kg / 10.0 AS Weight,
A.CaloriesOut, A.Steps, WD.Temperature, PH.Name, P.Caption
FROM Post P1
JOIN UserPhase UP
ON P1.Id = UP.Id
JOIN Phase PH
ON PH.Id = UP.PhaseId
JOIN Post P
ON P.CreatedOn BETWEEN UP.StartDate AND UP.EndDate
AND P.UserId = P1.UserId
JOIN FitnessDayEntry F
ON P.Id = F.Id
JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id


LEFT JOIN Post P2
ON P2.UserId = P1.UserId
JOIN DietPlan DP
ON P2.Id = DP.Id
JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
AND F.DayDate >= DPU.StartDate
AND F.DayDate <= DPU.EndDate
AND DPU.StartDate BETWEEN UP.StartDate AND UP.EndDate
JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
AND DD.DietDayTypeId = DPD.DietDayTypeId



-- Weight
LEFT JOIN Weight W
ON F.Id = W.Id 
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id
WHERE P1.UserId = 12
AND UP.StartDate BETWEEN 1535845000 AND 1535847000
--AND P.CreatedOn AND DPU.StartDate BETWEEN 1515370000 AND 1546645000  -- Whole 2018
ORDER BY P.CreatedOn

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

