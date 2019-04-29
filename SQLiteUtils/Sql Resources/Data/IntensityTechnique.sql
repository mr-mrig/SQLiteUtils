--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 17:02:52 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: IntensityTechnique
CREATE TABLE IntensityTechnique (Id INTEGER CONSTRAINT PK_IntensityTechnique_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT UQ_IntensityTechnique_Name UNIQUE CONSTRAINT CK_IntensityTechnique_Name_NotNull NOT NULL, Abbreviation TEXT CONSTRAINT UQ_IntensityTechnique_Abbreviation UNIQUE CONSTRAINT CK_IntensityTechnique_Abbreviation_NotNull NOT NULL, Description TEXT, IsLinkingTechnique INTEGER CONSTRAINT CK_IntensityTechnique_IsLinkingTechnique_NotNull NOT NULL CONSTRAINT DF_IntensityTechnique_IsLinkingTechnique DEFAULT (0) CONSTRAINT CK_IntensityTechnique_IsLinkingTechnique_IsBoolean CHECK (IsLinkingTechnique BETWEEN 0 AND 1), RPE INTEGER, IsApproved INTEGER CONSTRAINT CK_IntensityTechnique_IsApproveed_IsBoolean CHECK (IsApproved BETWEEN 0 AND 1), CreatedOn INTEGER CONSTRAINT CK_IntensityTechnique_CreatedOn_NotNull NOT NULL CONSTRAINT DF_IntensityTechnique_CreatedOn DEFAULT (STRFTIME('%s', 'now')), OwnerId INTEGER CONSTRAINT FK_IntensityTechnique_User_OwnerId REFERENCES User (Id) ON UPDATE CASCADE NOT NULL);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (1, 'Super Sets', 'SS', NULL, 1, NULL, 1, 1554390140, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (2, 'Jump Sets', 'JS', NULL, 1, NULL, NULL, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (3, 'Giant Sets', 'GS', NULL, 1, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (4, 'Forced Reps', 'Forced', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (5, 'Partial Reps', 'Part', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (6, 'Drop Sets', 'DS', NULL, 1, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (7, 'Cheating', 'Cheat', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (8, 'Rest-Pause', 'RP', NULL, 1, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (9, 'Burns', 'Burn', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (10, 'Forced Negative Reps', 'Neg', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (11, 'EMOM', 'EMOM', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (12, 'J-Reps', 'JR', NULL, 0, NULL, 1, 1554390149, 1);
INSERT INTO IntensityTechnique (Id, Name, Abbreviation, Description, IsLinkingTechnique, RPE, IsApproved, CreatedOn, OwnerId) VALUES (13, 'EDT', 'EDT', NULL, 0, NULL, 1, 1554390149, 1);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
