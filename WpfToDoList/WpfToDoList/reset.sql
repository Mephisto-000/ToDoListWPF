select * from TbToDoList

delete from TbToDoList
DBCC CHECKIDENT ('TbToDoList', RESEED, 0);  -- 把自增 Id 重設，下次從 1 開始

select * from TbToDoList