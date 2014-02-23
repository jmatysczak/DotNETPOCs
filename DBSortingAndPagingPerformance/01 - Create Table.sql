CREATE TABLE [Item] (
   [Id] [int] IDENTITY(1,1) NOT NULL,
   [Title] [nvarchar](250) NOT NULL,
   CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
