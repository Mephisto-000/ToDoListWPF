-- 參數，1=強制刪除重建，0=只要表已存在就不動
declare @ForceDropAndRecreate bit = 0;

if object_id(N'dbo.TbToDoList', N'U') is null
begin
    -- 資料表不存在，直接建立
    create table [dbo].[TbToDoList] (
        [Id]							int           IDENTITY (1, 1) NOT NULL,
        [Content]						nvarchar (MAX)				  NOT NULL,
        [Priority]						varchar (20)				  NOT NULL,
        [Date]							datetime					  default (getdate()) NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    );
end
else if @ForceDropAndRecreate = 1
begin
    -- 資料表已存在且選擇強制重建
    drop table [dbo].[TbToDoList];

    create table [dbo].[TbToDoList] (
        [Id]							int           IDENTITY (1, 1) NOT NULL,
        [Content]						nvarchar (MAX)				  NOT NULL,
        [Priority]						varchar (20)				  NOT NULL,
        [Date]							datetime					  default (getdate()) NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    );
end
