--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 17:38:47 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: ExerciseDifficulty
CREATE TABLE ExerciseDifficulty (Id INTEGER CONSTRAINT PK_ExerciseDifficulty_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_ExerciseDifficulty_Name UNIQUE CONSTRAINT CK_ExerciseDifficulty_Name_NotNull NOT NULL, Description TEXT);
INSERT INTO ExerciseDifficulty (Id, Name, Description) VALUES (1, 'Easy', 'Suitable to every user regardless of its skill');
INSERT INTO ExerciseDifficulty (Id, Name, Description) VALUES (2, 'Intermediate', 'Requires some movement skills.');
INSERT INTO ExerciseDifficulty (Id, Name, Description) VALUES (3, 'Advanced', 'Requires solid movement skills');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
