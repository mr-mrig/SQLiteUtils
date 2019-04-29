--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 16:44:27 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: EffortType
CREATE TABLE EffortType (Id INTEGER CONSTRAINT PK_EffortType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_EffortType_Name UNIQUE CONSTRAINT CK_EffortType_Name_NotNull NOT NULL, Abbreviation TEXT CONSTRAINT UQ_EffortType_Abbreviation UNIQUE CONSTRAINT CK_EffortType_Abbreviation_NotNull NOT NULL, Description TEXT);
INSERT INTO EffortType (Id, Name, Abbreviation, Description) VALUES (1, 'Intensity', '%', 'Percentage of 1RM');
INSERT INTO EffortType (Id, Name, Abbreviation, Description) VALUES (2, 'Repetition Maximum', 'RM', 'The most weight you can lift for a defined number of exercise movements');
INSERT INTO EffortType (Id, Name, Abbreviation, Description) VALUES (3, 'Rate of Perceived Exertion ', 'RPE', 'Self-assessed measure of the difficulty of a training set');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
