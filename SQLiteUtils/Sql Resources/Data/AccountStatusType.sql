--
-- File generated with SQLiteStudio v3.2.1 on gio apr 4 17:04:20 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: AccountStatusType
CREATE TABLE AccountStatusType (Id INTEGER PRIMARY KEY AUTOINCREMENT, Description TEXT (25) NOT NULL UNIQUE);
INSERT INTO AccountStatusType (Id, Description) VALUES (1, 'Active');
INSERT INTO AccountStatusType (Id, Description) VALUES (2, 'Inactive');
INSERT INTO AccountStatusType (Id, Description) VALUES (3, 'Blocked');
INSERT INTO AccountStatusType (Id, Description) VALUES (4, 'Super');
INSERT INTO AccountStatusType (Id, Description) VALUES (5, 'Reserved');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
