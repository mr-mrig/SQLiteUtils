--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 17:36:22 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: TrainingEquipment
CREATE TABLE TrainingEquipment (Id INTEGER CONSTRAINT PK_TrainingEquipment_Id PRIMARY KEY AUTOINCREMENT, Name TEXT CONSTRAINT CK_TrainingEquipment_Name_NotNull NOT NULL CONSTRAINT UQ_TrainingEquipment_Name UNIQUE, Description TEXT);
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (1, 'Barbell', 'Barbell with plates');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (2, 'Plate', 'Weight plate');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (3, 'Dumbell', 'Dumbells');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (4, 'Kettlebell', 'Kettlebell');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (5, 'Bands', 'Elastic Bands');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (6, 'Bench', 'Bench');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (7, 'Training Machine', 'Specific machine');
INSERT INTO TrainingEquipment (Id, Name, Description) VALUES (8, 'Free Weight', 'No equipment needed');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
