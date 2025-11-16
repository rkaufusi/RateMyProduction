-- ========================================
-- RateMyProduction - Initial Schema
-- Generated: November 16, 2025
-- Author: @RhettKaufusi
-- ========================================


USE RateMyProduction;
GO

-------------------------------------------------------------------------------
-- 1. Users Table
-------------------------------------------------------------------------------
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    DisplayName NVARCHAR(100) NULL,
    PrimaryRole NVARCHAR(100) NULL,
    IsEmailVerified BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    DateJoined DATETIME2 DEFAULT GETDATE(),
    LastLogin DATETIME2 NULL
);
GO

CREATE UNIQUE INDEX IX_Users_Email ON Users (Email);
GO

-------------------------------------------------------------------------------
-- 2. Refresh Tokens Table
-------------------------------------------------------------------------------
CREATE TABLE RefreshTokens (
    TokenID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
    
    TokenHash NVARCHAR(255) NOT NULL,
    JwtId NVARCHAR(100) NOT NULL,
    DeviceInfo NVARCHAR(255) NULL,
    IpAddress NVARCHAR(45) NULL,
    
    ExpiresAt DATETIME2 NOT NULL,
    RevokedAt DATETIME2 NULL,
    ReplacedByTokenID INT NULL,

    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

CREATE INDEX IX_RefreshTokens_UserID ON RefreshTokens(UserID);
CREATE INDEX IX_RefreshTokens_TokenHash ON RefreshTokens(TokenHash);
CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);
GO

-------------------------------------------------------------------------------
-- 3. Productions Table
-------------------------------------------------------------------------------
CREATE TABLE Productions (
    ProductionID INT PRIMARY KEY IDENTITY(1,1),

    Title NVARCHAR(200) NOT NULL,
    ProductionType NVARCHAR(50) NOT NULL,
    
    Studio NVARCHAR(100) NULL,
    Director NVARCHAR(100) NULL,
    YearReleased INT NULL,
	
    AverageRating DECIMAL(3, 2) NULL,
    ReviewCount INT NOT NULL DEFAULT 0,
    
    Synopsis NVARCHAR(MAX) NULL,

    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT CK_Productions_Year CHECK (YearReleased > 1890 AND YearReleased <= YEAR(GETDATE()) + 1)
);
GO

CREATE INDEX IX_Productions_Title ON Productions (Title);
CREATE INDEX IX_Productions_Studio ON Productions (Studio);
GO


-------------------------------------------------------------------------------
-- 4. Reviews Table
-------------------------------------------------------------------------------
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),

    ProductionID INT NOT NULL,
    UserID INT NOT NULL,

    RatingOverall TINYINT NOT NULL,

    RoleWorked NVARCHAR(100) NOT NULL,
    ReviewText NVARCHAR(MAX) NOT NULL,
    
    DatePosted DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsAnonymous BIT NOT NULL DEFAULT 0, -- 1 = Yes, 0 = No

    CONSTRAINT CK_Reviews_RatingOverall CHECK (RatingOverall BETWEEN 1 AND 5),

    -- Foreign Key Definitions
    CONSTRAINT FK_Reviews_ProductionID 
        FOREIGN KEY (ProductionID) 
        REFERENCES Productions(ProductionID)
        ON DELETE CASCADE, 

    CONSTRAINT FK_Reviews_UserID 
        FOREIGN KEY (UserID) 
        REFERENCES Users(UserID)
        ON DELETE NO ACTION, 

    CONSTRAINT UQ_User_Production UNIQUE (UserID, ProductionID)
);
GO

CREATE INDEX IX_Reviews_ProductionID ON Reviews (ProductionID, DatePosted DESC);
GO

-------------------------------------------------------------------------------
-- 5. Tags Table
-------------------------------------------------------------------------------
CREATE TABLE Tags (
    TagID INT PRIMARY KEY IDENTITY(1,1),
    TagName NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE UNIQUE INDEX IX_Tags_TagName ON Tags (TagName);
GO


-------------------------------------------------------------------------------
-- 6. ReviewTags Table
-------------------------------------------------------------------------------
CREATE TABLE ReviewTags (
    ReviewID INT NOT NULL,
    TagID INT NOT NULL,
    
    PRIMARY KEY (ReviewID, TagID), 

    CONSTRAINT FK_ReviewTags_ReviewID
        FOREIGN KEY (ReviewID)
        REFERENCES Reviews(ReviewID)
        ON DELETE CASCADE, 

    CONSTRAINT FK_ReviewTags_TagID
        FOREIGN KEY (TagID)
        REFERENCES Tags(TagID)
        ON DELETE NO ACTION 
);
GO

-------------------------------------------------------------------------------
-- 1. Aggregate Review Trigger
-------------------------------------------------------------------------------
CREATE TRIGGER TR_Review_Update_ProductionSummary
ON Reviews
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @AffectedProductionIDs TABLE (ProductionID INT PRIMARY KEY);

    INSERT INTO @AffectedProductionIDs (ProductionID)
    SELECT ProductionID FROM inserted
    UNION 
    SELECT ProductionID FROM deleted;
    
    UPDATE P
    SET 
        P.AverageRating = T.AvgRating,
        P.ReviewCount = T.ReviewCount
    FROM 
        Productions P
    INNER JOIN 
    (
        SELECT
            R.ProductionID,
            CAST(AVG(CAST(R.RatingOverall AS DECIMAL(3, 2))) AS DECIMAL(3, 2)) AS AvgRating,
            COUNT(R.ReviewID) AS ReviewCount
        FROM
            Reviews R
        INNER JOIN 
            @AffectedProductionIDs A ON R.ProductionID = A.ProductionID
        GROUP BY 
            R.ProductionID
    ) AS T ON P.ProductionID = T.ProductionID;

    UPDATE P
    SET
        P.AverageRating = NULL,
        P.ReviewCount = 0
    FROM 
        Productions P
    INNER JOIN 
        @AffectedProductionIDs A ON P.ProductionID = A.ProductionID
    WHERE
        NOT EXISTS (SELECT 1 FROM Reviews R WHERE R.ProductionID = P.ProductionID);

END
GO
