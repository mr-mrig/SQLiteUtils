PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: AccountStatusType
INSERT INTO AccountStatusType (Id, Description) VALUES (1, 'Active');
INSERT INTO AccountStatusType (Id, Description) VALUES (2, 'Inactive');
INSERT INTO AccountStatusType (Id, Description) VALUES (3, 'Blocked');
INSERT INTO AccountStatusType (Id, Description) VALUES (4, 'Super');
INSERT INTO AccountStatusType (Id, Description) VALUES (5, 'Reserved');

-- Table: DietDayType
INSERT INTO DietDayType (Id, Name, Description) VALUES (1, 'On', 'Training day');
INSERT INTO DietDayType (Id, Name, Description) VALUES (2, 'Off', 'Rest Day');
INSERT INTO DietDayType (Id, Name, Description) VALUES (3, 'Refeed', 'Caloric refeed');
INSERT INTO DietDayType (Id, Name, Description) VALUES (4, 'Fast', 'Very low calorie day');

-- Table: EffortType
INSERT INTO EffortType (Id, Name, Abbreviation, Description) VALUES (1, 'Intensity', '%', 'Percentage of 1RM');
INSERT INTO EffortType (Id, Name, Abbreviation, Description) VALUES (2, 'Repetition Maximum', 'RM', 'The most weight you can lift for a defined number of exercise movements');
INSERT INTO EffortType (Id, Name, Abbreviation, Description) VALUES (3, 'Rate of Perceived Exertion ', 'RPE', 'Self-assessed measure of the difficulty of a training set');

-- Table: Excercise
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (1, 'Bench Press', NULL, NULL, NULL, NULL, NULL, 1554392334, NULL, 1, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (2, 'Bench Press - Inclined', NULL, NULL, NULL, NULL, NULL, 1554392354, NULL, 1, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (3, 'Bench Press - Dumbell', NULL, NULL, NULL, NULL, NULL, 1554392398, NULL, 1, 3, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (4, 'Cable Flyes', NULL, NULL, NULL, NULL, NULL, 1554392418, NULL, 1, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (5, 'Pec Dec', NULL, NULL, NULL, NULL, NULL, 1554392429, NULL, 1, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (6, 'Dips', NULL, NULL, NULL, NULL, NULL, 1554392445, NULL, 1, 7, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (7, 'Squat', NULL, NULL, NULL, NULL, NULL, 1554392507, NULL, 10, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (8, 'Front Squat', NULL, NULL, NULL, NULL, NULL, 1554392507, NULL, 10, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (9, 'Overhead Squat', NULL, NULL, NULL, NULL, NULL, 1554392507, NULL, 10, 1, 1, 3);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (10, 'Leg Extensions', NULL, NULL, NULL, NULL, NULL, 1554392518, NULL, 10, 1, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (11, 'Sommersault Squat - Smith Machine', NULL, NULL, NULL, NULL, NULL, 1554392590, NULL, 10, 7, 1, 3);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (12, 'Sissy Squat', NULL, NULL, NULL, NULL, NULL, 1554392590, NULL, 10, 8, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (13, 'Military Press', NULL, NULL, NULL, NULL, NULL, 1554392654, NULL, 2, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (14, 'Overhead Press - Dumbell', NULL, NULL, NULL, NULL, NULL, 1554392654, NULL, 2, 3, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (15, 'Lateral Raises', NULL, NULL, NULL, NULL, NULL, 1554392654, NULL, 2, 3, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (16, 'Overhead Press Machine', NULL, NULL, NULL, NULL, NULL, 1554392654, NULL, 2, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (17, 'Shrugs - Barbell', NULL, NULL, NULL, NULL, NULL, 1554392711, NULL, 6, 1, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (18, 'Shrugs - Dumbell', NULL, NULL, NULL, NULL, NULL, 1554392711, NULL, 6, 3, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (19, 'Shrugs on inclined bench', NULL, NULL, NULL, NULL, NULL, 1554392711, NULL, 6, 3, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (20, 'Pullups', NULL, NULL, NULL, NULL, NULL, 1554392816, NULL, 7, 8, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (21, 'Lat Machine', NULL, NULL, NULL, NULL, NULL, 1554392816, NULL, 7, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (22, 'Lat Machine - Reverse grip', NULL, NULL, NULL, NULL, NULL, 1554392816, NULL, 7, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (23, 'Pulldowns', NULL, NULL, NULL, NULL, NULL, 1554392816, NULL, 7, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (24, 'Barbell Row', NULL, NULL, NULL, NULL, NULL, 1554392816, NULL, 7, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (25, 'Romanian Deadlifts', NULL, NULL, NULL, NULL, NULL, 1554392893, NULL, 11, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (26, 'Leg Curl - Lying', NULL, NULL, NULL, NULL, NULL, 1554392893, NULL, 11, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (27, 'Leg Curl - Seated', NULL, NULL, NULL, NULL, NULL, 1554392893, NULL, 11, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (28, 'Deadlifts', NULL, NULL, NULL, NULL, NULL, 1554392893, NULL, 11, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (29, 'Nordic Ham Raises', NULL, NULL, NULL, NULL, NULL, 1554392915, NULL, 11, 8, 1, 3);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (30, 'French Press', NULL, NULL, NULL, NULL, NULL, 1554392999, NULL, 4, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (31, 'Skullcrusher', NULL, NULL, NULL, NULL, NULL, 1554392999, NULL, 4, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (32, 'Triceps Cable Extensions', NULL, NULL, NULL, NULL, NULL, 1554392999, NULL, 4, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (33, 'Triceps Dips', NULL, NULL, NULL, NULL, NULL, 1554392999, NULL, 4, 6, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (34, 'Cable Curl', NULL, NULL, NULL, NULL, NULL, 1554393053, NULL, 3, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (35, 'Dumbell Curl', NULL, NULL, NULL, NULL, NULL, 1554393053, NULL, 3, 3, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (36, 'Lat Machine behind the neck', NULL, NULL, NULL, NULL, NULL, 1554393053, NULL, 3, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (37, 'Seated Calvees - Smith Machine', NULL, NULL, NULL, NULL, NULL, 1554393135, NULL, 13, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (38, 'Standing Calves Machine', NULL, NULL, NULL, NULL, NULL, 1554393135, NULL, 13, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (39, 'Squatting Calves - Smith Machine', NULL, NULL, NULL, NULL, NULL, 1554393135, NULL, 13, 7, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (40, 'Calves on lg press', NULL, NULL, NULL, NULL, NULL, 1554393135, NULL, 13, 7, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (41, 'Seated Calves - Barbell', NULL, NULL, NULL, NULL, NULL, 1554393135, NULL, 13, 1, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (42, 'Hip Thrust', NULL, NULL, NULL, NULL, NULL, 1554393197, NULL, 9, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (43, 'Frog Pump', NULL, NULL, NULL, NULL, NULL, 1554393197, NULL, 9, 8, 1, 1);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (44, 'Sumo Squat', NULL, NULL, NULL, NULL, NULL, 1554393197, NULL, 9, 1, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (45, 'Sumo Squat - Dumbell', NULL, NULL, NULL, NULL, NULL, 1554393197, NULL, 9, 3, 1, 2);
INSERT INTO Excercise (Id, Name, Description, ExecutionGuide, CriticalPointsDescription, ImageUrl, IsApproved, CreatedOn, LastUpdate, MuscleId, TrainingEquipmentId, OwnerId, ExcerciseDifficultyId) VALUES (46, 'Hip Thrust - Leg Extension', NULL, NULL, NULL, NULL, NULL, 1554393219, NULL, 9, 7, 1, 1);


-- Table: ExerciseDifficulty 
INSERT INTO ExerciseDifficulty (Id, Name, Description) VALUES (1, 'Easy', 'Suitable to every user regardless of its skill');
INSERT INTO ExerciseDifficulty (Id, Name, Description) VALUES (2, 'Intermediate', 'Requires some movement skills.');
INSERT INTO ExerciseDifficulty (Id, Name, Description) VALUES (3, 'Advanced', 'Requires solid movement skills');


-- Table: IntensityTechnique
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (1, 'Super Sets', 'SS', NULL, 1, NULL, 1, 1554390140, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (2, 'Jump Sets', 'JS', NULL, 1, NULL, NULL, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (3, 'Giant Sets', 'GS', NULL, 1, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (4, 'Forced Reps', 'Forced', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (5, 'Partial Reps', 'Part', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (6, 'Drop Sets', 'DS', NULL, 1, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (7, 'Cheating', 'Cheat', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (8, 'Rest-Pause', 'RP', NULL, 1, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (9, 'Burns', 'Burn', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (10, 'Forced Negative Reps', 'Neg', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (11, 'EMOM', 'EMOM', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (12, 'J-Reps', 'JR', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (13, 'EDT', 'EDT', NULL, 0, NULL, 1, 1554390149, 1);


-- Table: Muscle
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (1, 'Chest', 'Chest');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (2, 'Shoulders', 'Delt');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (3, 'Biceps', 'Bicep');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (4, 'Triceps', 'Tri');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (5, 'Forearms', 'Forearm');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (6, 'Trapezius', 'Trap');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (7, 'Back', 'Back');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (8, 'Abdomen', 'Abs');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (9, 'Glutes', 'Glute');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (10, 'Quadriceps', 'Quad');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (11, 'Hamstrings', 'Ham');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (12, 'Lower Back', 'Lower Back');
INSERT INTO Muscle (Id, Name, Abbreviation) VALUES (13, 'Calves', 'Calf');


-- Table: Phase 
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (1, 'Bulk', 1553869048, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (2, 'Cut', 1553869077, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (3, 'Recomp', 1553869077, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (4, 'Strength', 1553869077, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (5, 'Contest Prep', 1553869077, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (6, 'Dummy', 1553869127, 0, 3);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (7, 'Peak', 1553869127, 1, 3);


-- Table: TrainingEquipment
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (1, 'Barbell', 'Barbell with plates');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (2, 'Plate', 'Weight plate');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (3, 'Dumbell', 'Dumbells');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (4, 'Kettlebell', 'Kettlebell');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (5, 'Bands', 'Elastic Bands');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (6, 'Bench', 'Bench');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (7, 'Training Machine', 'Specific machine');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (8, 'Free Weight', 'No equipment needed');


-- Table: TrainingPlanRelationType
INSERT INTO TrainingPlanRelationType (Id, Name, Description) VALUES (1, 'Variant', 'Variant of another plan which shares some pecularities with');
INSERT INTO TrainingPlanRelationType (Id, Name, Description) VALUES (2, 'Inherited', 'Received by another user');


-- Table: TrainingProficiency
INSERT INTO TrainingProficiency (Id, Name, Description, CreatedOn, IsApproved, OwnerId) VALUES (1, 'Newcomer', 'Just stepped into the gym and/or very poor athletic capabilities.', 1554386824, 1, 1);
INSERT INTO TrainingProficiency (Id, Name, Description, CreatedOn, IsApproved, OwnerId) VALUES (2, 'Beginner', 'Very low training expirience and basic athletic capabilities.', 1554386824, 1, 1);
INSERT INTO TrainingProficiency (Id, Name, Description, CreatedOn, IsApproved, OwnerId) VALUES (3, 'Intermediate', 'Intermediate training expirience [few months up to many years according to skills]', 1554386936, 1, 1);
INSERT INTO TrainingProficiency (Id, Name, Description, CreatedOn, IsApproved, OwnerId) VALUES (4, 'Advanced', 'High training expirience and solid skills.', 1554386936, 1, 1);


-- Table: TrainingWeekType
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (1, 'Standard', 'Generic week with no specific target');
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (2, 'Deload', 'Active recovery week');
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (3, 'Overreach', 'High stress week');
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (4, 'Peak week', 'Peak performance oriented week');
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (5, 'Tapering', 'Relief phase before a test');


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
