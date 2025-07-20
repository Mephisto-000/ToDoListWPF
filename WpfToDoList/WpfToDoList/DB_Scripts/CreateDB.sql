
if not exists (select name from sys.databases where name = N'ToDoListDB')
begin
	create DATABASE ToDoListDB
	collate Chinese_Taiwan_Stroke_CS_AS
end

GO
