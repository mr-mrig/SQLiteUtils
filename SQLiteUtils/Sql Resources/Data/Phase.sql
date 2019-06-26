--
-- File generated with SQLiteStudio v3.2.1 on lun giu 24 17:07:37 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Phase
DROP TABLE IF EXISTS Phase;
CREATE TABLE Phase (Id INTEGER CONSTRAINT PK_Phase_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_Phase_Name UNIQUE CONSTRAINT CK_Phase_Name_NotNull NOT NULL, CreatedOn INTEGER CONSTRAINT DF_Phase_CreatedOn DEFAULT (strftime('%s', 'now')) CONSTRAINT CK_Phase_CreatedOn_NotNull NOT NULL, EntryStatusTypeId INTEGER CONSTRAINT FK_Phase_EntryStatusType_Id REFERENCES EntryStatusType (Id) CONSTRAINT CK_Phase_EntryStatusTypeId_NotNull NOT NULL, OwnerId INTEGER CONSTRAINT FK_Phase_User_OwnerId REFERENCES User (Id) ON UPDATE CASCADE CONSTRAINT CK_Phase_OwnerId_NotNull NOT NULL);
INSERT INTO Phase (Id, Name, CreatedOn, EntryStatusTypeId, OwnerId) VALUES (1, 'Bulk', 1553869048, 5, 1);
INSERT INTO Phase (Id, Name, CreatedOn, EntryStatusTypeId, OwnerId) VALUES (2, 'Cut', 1553869077, 5, 1);
INSERT INTO Phase (Id, Name, CreatedOn, EntryStatusTypeId, OwnerId) VALUES (3, 'Recomp', 1553869077, 5, 1);
INSERT INTO Phase (Id, Name, CreatedOn, EntryStatusTypeId, OwnerId) VALUES (4, 'Strength', 1553869077, 5, 1);
INSERT INTO Phase (Id, Name, CreatedOn, EntryStatusTypeId, OwnerId) VALUES (5, 'Contest Prep', 1553869077, 5, 1);
INSERT INTO Phase (Id, Name, CreatedOn, EntryStatusTypeId, OwnerId) VALUES (6, 'Dummy', 1553869127, 4, 3);
INSERT INTO Phase (Id, Name, CreatedOn, EntryStatusTypeId, OwnerId) VALUES (7, 'Peak', 1553869127, 3, 3);
INSERT INTO Phase (Id, Name, CreatedOn, EntryStatusTypeId, OwnerId) VALUES (8, 'PrivatePhase', 1561388818, 1, 12);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
