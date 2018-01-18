CREATE TABLE [dbo].[Products]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[DateCreatedUtc]  DATETIME2 (3)   CONSTRAINT [DF_Products_DateCreatedUtc] DEFAULT (sysutcdatetime()) NOT NULL,
    [DateEditedUtc]   DATETIME2 (3)   CONSTRAINT [DF_Products_DateEditedUtc] DEFAULT (sysutcdatetime()) NOT NULL,
    [SKU] NVARCHAR(50) NOT NULL, 
    [BrandName] NVARCHAR(50) NOT NULL, 
    [ImageUrlBack] NVARCHAR(256) NULL, 
	[ImageUrlFront] NVARCHAR(256) NULL, 
	[ImageUrlName] NVARCHAR(256) NULL, 
	[ImageUrlSide] NVARCHAR(256) NULL, 
    [Size] NVARCHAR(16) NULL, 
    [DecorationMethod] INT NOT NULL, 
    [FloodColor] NVARCHAR(20) NULL,
	[ColorGroup] NVARCHAR(50) NULL, 
    [Price] DECIMAL(10, 2) NULL, 
    [Gender] NVARCHAR(50) NULL, 
	[PersonalizationVariant] INT NOT NULL DEFAULT 0, -- values: 0-Undefined; 1 - L , 2 - D
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UC_Products_SKU] UNIQUE([SKU])
)
