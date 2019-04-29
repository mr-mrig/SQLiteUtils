--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 16:09:18 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: TrainingProficiency
CREATE TABLE TrainingProficiency (Id INTEGER CONSTRAINT PK_TrainingProficiency_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_TrainingProficiency_Id UNIQUE CONSTRAINT CK_TrainingProficiency_NotNull NOT NULL, Description TEXT, CreatedOn INTEGER CONSTRAINT CK_TrainingProficiency_NotNull NOT NULL CONSTRAINT DF_TrainingProficiency_CreatedOn DEFAULT (strftime('%s', 'now')), IsApproved INTEGER CONSTRAINT DF_TrainingProficiency_IsApproved DEFAULT (0) CONSTRAINT CK_TrainingProficiency_IsApproved_IsBoolean CHECK (IsApproved BETWEEN 0 AND 1), OwnerId INTEGER CONSTRAINT FK_TrainingProficiency_User_OwnerId REFERENCES User (Id) ON DELETE SET NULL ON UPDATE CASCADE CONSTRAINT CK_TrainingProficiency_OwnerId_NotNull NOT NULL);
INSERT INTO TrainingProficiency (Id, Name, Description, CreatedOn, IsApproved, OwnerId) VALUES (1, 'Newcomer', 'Just stepped into the gym and/or very poor athletic capabilities.', 1554386824, 1, 1);
INSERT INTO TrainingProficiency (Id, Name, Description, CreatedOn, IsApproved, OwnerId) VALUES (2, 'Beginner', 'Very low training expirience and basic athletic capabilities.', 1554386824, 1, 1);
INSERT INTO TrainingProficiency (Id, Name, Description, CreatedOn, IsApproved, OwnerId) VALUES (3, 'Intermediate', 'Intermediate training expirience [few months up to many years according to skills]', 1554386936, 1, 1);
INSERT INTO TrainingProficiency (Id, Name, Description, CreatedOn, IsApproved, OwnerId) VALUES (4, 'Advanced', 'High training expirience and solid skills.', 1554386936, 1, 1);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
