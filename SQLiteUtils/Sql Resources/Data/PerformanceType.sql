--
-- File generated with SQLiteStudio v3.2.1 on mar giu 25 09:50:16 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: PerformanceType
DROP TABLE IF EXISTS PerformanceType;
CREATE TABLE PerformanceType (Id INTEGER CONSTRAINT PK_PerformanceType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT CK_PerformanceType_Name_NotNull NOT NULL CONSTRAINT UQ_PerformanceType_Name UNIQUE, Description TEXT);
INSERT INTO PerformanceType (Id, Name, Description) VALUES (1, 'RM', 'Weights and Reps');
INSERT INTO PerformanceType (Id, Name, Description) VALUES (2, 'Maximum Reps', 'Reps Only');
INSERT INTO PerformanceType (Id, Name, Description) VALUES (3, 'Duration', 'Time Duration');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
