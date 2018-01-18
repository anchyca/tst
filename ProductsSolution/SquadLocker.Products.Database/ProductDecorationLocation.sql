CREATE TABLE [dbo].[ProductDecorationLocation]
(
	[ProductId] INT NOT NULL, 
    [DecorationLocation] INT NOT NULL,
	CONSTRAINT [FK_ProductDecorationLocation_Product] FOREIGN KEY([ProductId]) REFERENCES [dbo].[Products]([Id])
)
