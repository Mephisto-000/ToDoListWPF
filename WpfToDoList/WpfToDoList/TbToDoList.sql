CREATE TABLE [dbo].[TbToDoList] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [Content]  NVARCHAR (MAX) NOT NULL,
    [Priority] VARCHAR (20)  NOT NULL,
    [Date]     DATETIME      DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);