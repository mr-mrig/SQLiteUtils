--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 17:53:56 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Excercise
CREATE TABLE Excercise (Id INTEGER CONSTRAINT PK_Exercise_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_Exercise_Name UNIQUE CONSTRAINT CK_Exercise_Name_NotNull NOT NULL, Description TEXT, ExecutionGuide TEXT, CriticalPointsDescription TEXT, ImageUrl TEXT, IsApproved INTEGER CONSTRAINT CK_Exercise_IsApproved_IsBoolean CHECK (IsApproved BETWEEN 0 AND 1), CreatedOn INTEGER CONSTRAINT CK_Exercise_CreatedOn_NotNull NOT NULL CONSTRAINT DF_Exercise_CreatedOn DEFAULT (STRFTIME('%s', 'now')), LastUpdate INTEGER, MuscleId INTEGER CONSTRAINT FK_Excercise_Muscle_MuscleId REFERENCES Muscle (Id) ON UPDATE CASCADE CONSTRAINT CK_Exercise_MuscleId_NotNull NOT NULL, TrainingEquipmentId INTEGER CONSTRAINT FK_Exercise_TrainingEquipment_Id REFERENCES TrainingEquipment (Id) ON UPDATE CASCADE CONSTRAINT CK_Exercise_TrainingEquipmentId_NotNull NOT NULL, OwnerId INTEGER CONSTRAINT FK_Exercise_User_OwnerId REFERENCES User (Id) ON UPDATE CASCADE CONSTRAINT CK_Exercise_OwnerId_NotNull NOT NULL, ExcerciseDifficultyId INTEGER CONSTRAINT FK_Exercise_ExerciseDifficulty_Id REFERENCES ExerciseDifficulty (Id) ON UPDATE CASCADE CONSTRAINT CK_Exercise_ExcerciseDifficulty_NotNull NOT NULL);
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

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
