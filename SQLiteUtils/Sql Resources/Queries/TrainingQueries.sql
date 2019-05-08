







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

--PERFORMANCE: 

-- NOTE: To be finished




SELECT TP.Id, TP.Name as PlanName, TP.IsBookmarked, TP.IsTemplate, 
TH.Body As Hashtag, TProf.Name as Proficiency, Pha.Name as Phase,

WT.Id as WeekId, WOT.Id as WorkoutId, WUT.Id as WorkUnitId, ST.TargetRepetitions,

-- Average Workout Days per plan
(
    SELECT AVG(WoCount.Counter)
    FROM
    (
        SELECT TrainingPlanId, count(1) as Counter
        FROM TrainingWeekTemplate
        JOIN WorkoutTemplate
        ON TrainingWeekTemplate.Id = TrainingWeekId
        
        WHERE WT.TrainingPlanId = TP.Id
        GROUP BY TrainingWeekTemplate.Id
    ) WoCount
) AS AvgWorkoutDays,

-- Average weekly working sets
(
    SELECT Avg(WsCount.Counter)
    FROM
    (
    select WT.Id, count(1) as Counter
        FROM TrainingWeekTemplate WT
    JOIN WorkoutTemplate WOT
    ON WT.Id = WOT.TrainingWeekId
    JOIN WorkUnitTemplate WUT
    ON WOT.Id = WUT.WorkoutTemplateId
    JOIN SetTemplate ST
    ON WUT.Id = ST.WorkUnitId
    
        WHERE WT.TrainingPlanId = 325
        GROUP BY WT.Id
    ) WsCount
) As AvgWorkingSets

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
--AND TP.Id = 323

ORDER BY TP.IsBookmarked DESC, TP.IsTemplate DESC







