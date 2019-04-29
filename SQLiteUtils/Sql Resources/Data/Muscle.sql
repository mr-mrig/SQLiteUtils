--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 15:23:16 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Muscle
CREATE TABLE Muscle (Id INTEGER CONSTRAINT PK_Muscle_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_Muscle_Name UNIQUE CONSTRAINT CK_Muscle_Name_NotNull NOT NULL, Abbreviation TEXT CONSTRAINT UQ_Muscle_Abbreviation UNIQUE CONSTRAINT CK_Muscle_Abbreviation_NotNull NOT NULL);
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

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
