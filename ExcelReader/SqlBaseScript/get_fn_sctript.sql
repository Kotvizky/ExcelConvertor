use ImportProcessing
go

declare @procs table (id int identity(0,1), name varChar(250))

insert into @procs(name)
select SPECIFIC_NAME 
from ImportProcessing.information_schema.routines 
where 
	routine_type in ('function') and SPECIFIC_SCHEMA = 'dbo'
order by SPECIFIC_NAME

declare @procName varChar(250), @i int = 0, @i_max int = (select max(id) + 1 from @procs)


while @i < @i_max
begin
	
	select @procName = 'ImportProcessing.dbo.'+ name  from @procs where id = @i
	set @i = @i + 1;
	exec sp_helpText  @procName
end