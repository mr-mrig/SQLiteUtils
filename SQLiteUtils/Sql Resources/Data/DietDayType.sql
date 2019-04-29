--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 17:16:12 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: DietDayType
CREATE TABLE DietDayType (Id INTEGER CONSTRAINT PK_DietDayType_Id PRIMARY KEY AUTOINCREMENT CONSTRAINT UQ_DietDayType_Id UNIQUE CONSTRAINT CK_DietDayType_Id_NotNull NOT NULL, Name TEXT CONSTRAINT CK_DietDayType_Name_NotNull NOT NULL, Description TEXT);
INSERT INTO DietDayType (Id, Name, Description) VALUES (1, 'On', 'Training day');
INSERT INTO DietDayType (Id, Name, Description) VALUES (2, 'Off', 'Rest Day');
INSERT INTO DietDayType (Id, Name, Description) VALUES (3, 'Refeed', 'Caloric refeed');
INSERT INTO DietDayType (Id, Name, Description) VALUES (4, 'Fast', 'Very low calorie day');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
