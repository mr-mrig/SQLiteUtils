--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 16:17:56 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: TrainingWeekType
CREATE TABLE TrainingWeekType (Id INTEGER CONSTRAINT PK_TrainingWeekType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_TrainingWeekType_Name UNIQUE CONSTRAINT CK_TrainingWeekType_Name_NotNull NOT NULL, Description TEXT);
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (1, 'Standard', 'Generic week with no specific target');
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (2, 'Deload', 'Active recovery week');
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (3, 'Overreach', 'High stress week');
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (4, 'Peak week', 'Peak performance oriented week');
INSERT INTO TrainingWeekType (Id, Name, Description) VALUES (5, 'Tapering', 'Relief phase before a test');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
