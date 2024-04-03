DELETE FROM dbo.DocumentItems;
DELETE FROM dbo.Documents;

DBCC CHECKIDENT ('dbo.DocumentItems', RESEED, 0);
DBCC CHECKIDENT ('dbo.Documents', RESEED, 0);
