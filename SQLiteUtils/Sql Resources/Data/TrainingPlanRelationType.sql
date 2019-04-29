--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 17:09:42 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: TrainingPlanRelationType
CREATE TABLE TrainingPlanRelationType (Id INTEGER CONSTRAINT PK_TrainingPlanRelationType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_TrainingPlanRelationType_Name UNIQUE CONSTRAINT CK_TrainingPlanRelationType_Name_NotNull NOT NULL, Description TEXT CONSTRAINT CK_TrainingPlanRelationType_Description_NotNull NOT NULL);
INSERT INTO TrainingPlanRelationType (Id, Name, Description) VALUES (1, 'Variant', 'Variant of another plan which shares some pecularities with');
INSERT INTO TrainingPlanRelationType (Id, Name, Description) VALUES (2, 'Inherited', 'Received by another user');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
