--
-- File generated with SQLiteStudio v3.2.1 on lun giu 24 17:08:45 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: EntryStatusType
DROP TABLE IF EXISTS EntryStatusType;
CREATE TABLE EntryStatusType (Id INTEGER CONSTRAINT PK_EntryStatusType_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_EntryStatusType_Name UNIQUE CONSTRAINT CK_EntryStatusType_Name_NotNull NOT NULL, Description TEXT CONSTRAINT CK_EntryStatusType_Description_NotNull NOT NULL);
INSERT INTO EntryStatusType (Id, Name, Description) VALUES (1, 'Private', 'Private entry shared with nobody');
INSERT INTO EntryStatusType (Id, Name, Description) VALUES (2, 'Pending', 'Public entry waiting for approval');
INSERT INTO EntryStatusType (Id, Name, Description) VALUES (3, 'Approved', 'Approved public entry');
INSERT INTO EntryStatusType (Id, Name, Description) VALUES (4, 'Banned', 'Banned entry');
INSERT INTO EntryStatusType (Id, Name, Description) VALUES (5, 'Native', 'Entry included in the original DB');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
