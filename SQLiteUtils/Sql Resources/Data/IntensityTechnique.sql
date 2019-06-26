--
-- File generated with SQLiteStudio v3.2.1 on lun giu 24 17:16:50 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: IntensityTechnique
DROP TABLE IF EXISTS IntensityTechnique;
CREATE TABLE IntensityTechnique (Id INTEGER CONSTRAINT PK_IntensityTechnique_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_IntensityTechnique_Name UNIQUE CONSTRAINT CK_IntensityTechnique_Name_NotNull NOT NULL, Abbreviation TEXT CONSTRAINT UQ_IntensityTechnique_Abbreviation UNIQUE CONSTRAINT CK_IntensityTechnique_Abbreviation_NotNull NOT NULL, Description TEXT, IsLinkingTechnique INTEGER CONSTRAINT CK_IntensityTechnique_IsLinkingTechnique_NotNull NOT NULL CONSTRAINT DF_IntensityTechnique_IsLinkingTechnique DEFAULT (0) CONSTRAINT CK_IntensityTechnique_IsLinkingTechnique_IsBoolean CHECK (IsLinkingTechnique BETWEEN 0 AND 1), RPE INTEGER, ModeratorId INTEGER CONSTRAINT FK_IntensityTechnique_User_ModeratorId REFERENCES User (Id) ON UPDATE CASCADE NOT NULL, EntityStatusTypeId INTEGER CONSTRAINT FK_IntensityTechnique_EntityStatusType_Id REFERENCES EntryStatusType (Id) CONSTRAINT CK_IntensityTechnique_EntityStatusTypeId_NotNull NOT NULL);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (1, 'Super Sets', 'SS', NULL, 1, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (2, 'Jump Sets', 'JS', NULL, 1, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (3, 'Giant Sets', 'GS', NULL, 1, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (4, 'Forced Reps', 'Forced', NULL, 0, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (5, 'Partial Reps', 'Part', NULL, 0, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (6, 'Drop Sets', 'DS', NULL, 1, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (7, 'Cheating', 'Cheat', NULL, 0, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (8, 'Rest-Pause', 'RP', NULL, 1, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (9, 'Burns', 'Burn', NULL, 0, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (10, 'Forced Negative Reps', 'Neg', NULL, 0, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (11, 'EMOM', 'EMOM', NULL, 0, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (12, 'J-Reps', 'JR', NULL, 0, NULL, 1, 5);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, ModeratorId, EntityStatusTypeId) VALUES (13, 'EDT', 'EDT', NULL, 0, NULL, 1, 5);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
