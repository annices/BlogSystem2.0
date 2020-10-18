SET NOCOUNT ON -- Don't display system messages such as "rows effected" etc.

IF DB_ID('BlogSystem') IS NULL
	BEGIN
	  CREATE DATABASE BlogSystem
	  PRINT '>> The database was created successfully!'
	END
ELSE
	BEGIN
		PRINT '>> The database already exists!'
	END
GO

CREATE TABLE BlogSystem.dbo.BS_Users
(
	ID Int IDENTITY(1, 1) PRIMARY KEY,
	Username Varchar(30) UNIQUE NOT NULL,
	Firstname Varchar(30),
	Lastname Varchar(30),
	Email Varchar(50) UNIQUE NOT NULL,
	Password Varchar(500)
)

CREATE TABLE BlogSystem.dbo.BS_Categories
(
	ID Int IDENTITY(1, 1) PRIMARY KEY,
	Category Varchar(50) UNIQUE NOT NULL
)

CREATE TABLE BlogSystem.dbo.BS_Entries
(
	ID Int IDENTITY(1, 1) PRIMARY KEY,
	Title Varchar(50) NOT NULL,
	Date DateTime,
	Entry Varchar(8000) NOT NULL,
	IsPublished Bit NOT NULL DEFAULT 1,
	UserID Int NOT NULL
		REFERENCES BS_Users(ID)
	    ON DELETE CASCADE
)

CREATE TABLE BlogSystem.dbo.BS_EntryCategories
(
	ID Int IDENTITY(1, 1) PRIMARY KEY,
	EntryID Int
		REFERENCES BS_Entries(ID)
		ON DELETE CASCADE,
	CategoryID Int
		REFERENCES BS_Categories(ID)
		ON DELETE CASCADE
)

CREATE UNIQUE INDEX Unique_EntryCategory ON BlogSystem.dbo.BS_EntryCategories(EntryID, CategoryID)

CREATE TABLE BlogSystem.dbo.BS_Comments
(
	ID Int IDENTITY(1, 1) PRIMARY KEY,
	Name Varchar(30) NOT NULL,
	Email Varchar(50),
	Date DateTime,
	Website Varchar(100),
	Comment Varchar(300) NOT NULL,
	Reply Varchar(300),
	EntryID Int NOT NULL
		REFERENCES BS_Entries(ID)
		ON DELETE CASCADE
)

INSERT BlogSystem.dbo.BS_Users VALUES
(
	'YourUserName',
	'YourFirstname', -- Optional.
	'YourLastname', -- Optional.
	'yourmail@mail.se',
	-- Keep the hashed password below until your first login. Default password is set to
	-- "admin", but can be changed under the admin panel once you're logged in:
	'AQAAAAEAACcQAAAAEBehHmgEHZmjXlTBGlKSW9KVuxMIHp1f4r8sC502SFQkGGxiYeef6HFntNMCMdZ76w=='
)