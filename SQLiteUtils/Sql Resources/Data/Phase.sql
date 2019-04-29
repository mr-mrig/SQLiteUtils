--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 17:05:59 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Phase
CREATE TABLE Phase (Id INTEGER CONSTRAINT PK_Phase_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_Phase_Name UNIQUE CONSTRAINT CK_Phase_Name_NotNull NOT NULL, CreatedOn INTEGER CONSTRAINT DF_Phase_CreatedOn DEFAULT (strftime('%s', 'now')) CONSTRAINT CK_Phase_CreatedOn_NotNull NOT NULL, IsApproved INTEGER CONSTRAINT CK_Phase_IsApproved_IsBoolean CHECK (IsApproved BETWEEN 0 AND 1), OwnerId INTEGER CONSTRAINT FK_Phase_User_OwnerId REFERENCES User (Id) ON UPDATE CASCADE CONSTRAINT CK_Phase_OwnerId_NotNull NOT NULL);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (1, 'Bulk', 1553869048, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (2, 'Cut', 1553869077, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (3, 'Recomp', 1553869077, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (4, 'Strength', 1553869077, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (5, 'Contest Prep', 1553869077, NULL, 1);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (6, 'Dummy', 1553869127, 0, 3);
INSERT INTO Phase (Id, Name, CreatedOn, IsApproved, OwnerId) VALUES (7, 'Peak', 1553869127, 1, 3);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
