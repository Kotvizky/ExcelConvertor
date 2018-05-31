Changed database context to 'ImportProcessing'.

(обработано строк: 57)
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_actUkrSib] (@Ip AS binary(15), @RNumber varChar(100),
                                                                                                                                                                              
		@StartDate dateTime,@FinishDate dateTime,
                                                                                                                                                                                                                  
		@bonusGroup nVarChar(50), @params varChar(250))
                                                                                                                                                                                                            
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Fio nVarChar(500),
                                                                                                                                                                                                                                          
	Pays money,
                                                                                                                                                                                                                                                 
	DPD int,
                                                                                                                                                                                                                                                    
	Perc dec(6,3),
                                                                                                                                                                                                                                              
	ProdName nVarChar(50),
                                                                                                                                                                                                                                      
	daysGroup nVarChar(50),
                                                                                                                                                                                                                                     
	bankSumm money,
                                                                                                                                                                                                                                             
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	declare @maxRow int = (select max(t0.Row_Id) from inVal t0 where t0.ip = @ip);
                                                                                                                                                                              

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,Fio, Comment)
                                                                                                                                                                                             
	select ROW_NUMBER () OVER (ORDER BY t00.ContractId)+@maxRow,
                                                                                                                                                                                                
		t00.ContractId,t00.ContractNum,t00.Fio,t00.Comment
                                                                                                                                                                                                         
	from
                                                                                                                                                                                                                                                        
		(	select distinct ContractId, ContractNum,Fio, Comment 
                                                                                                                                                                                                    
			from [dbo].[fn_getBaseId] (@Ip, @RNumber,null,@params) t0	
                                                                                                                                                                                                
		) t00
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	-- Calculate product name 
                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set ProdName = left(t2.Name,50)
                                                                                                                                                                                                                             
	from @tRes t0 
                                                                                                                                                                                                                                              
		join Contract t1 on t0.ContractId = t1.id
                                                                                                                                                                                                                  
		join DictionaryProd t2 on t1.ProductTypeId = t2.id
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	-- Calculate Payment 
                                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
	declare @tPays table(Row_Id int, ContractId bigint, Pays money, ContractNum nVarChar(90))
                                                                                                                                                                   

                                                                                                                                                                                                                                                             
	insert into @tPays (Row_Id , ContractId , Pays )
                                                                                                                                                                                                            
	select t0.Row_Id,t0.ContractId,sum(t1.CurrencyPay)
                                                                                                                                                                                                          
	from @tRes t0
                                                                                                                                                                                                                                               
		join SchedulePay t1 on t0.ContractId = t1.ContractId
                                                                                                                                                                                                       
	where t1.DatePay between @StartDate and @FinishDate
                                                                                                                                                                                                         
	group by t0.Row_Id,t0.ContractId
                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
	update @tRes
                                                                                                                                                                                                                                                
	set Pays = t1.Pays
                                                                                                                                                                                                                                          
	from @tRes t0
                                                                                                                                                                                                                                               
		join @tPays t1 
                                                                                                                                                                                                                                            
			on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	-- Calculate DPD
                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
	declare @tDpd table(Row_Id int, ContractId bigint, Dpd int)
                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	insert into @tDpd (Row_Id , ContractId , Dpd )
                                                                                                                                                                                                              
	select t0.Row_Id,t0.ContractId,t1.Dpd
                                                                                                                                                                                                                       
	from @tRes t0
                                                                                                                                                                                                                                               
		join ContractStatic t1 on t0.ContractId = t1.ContractId
                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
	update @tRes
                                                                                                                                                                                                                                                
	set Dpd = t1.DPD
                                                                                                                                                                                                                                            
	from @tRes t0
                                                                                                                                                                                                                                               
		join @tDpd t1 
                                                                                                                                                                                                                                             
			on t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	--	Calculate percentages
                                                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
	declare @tFine table(Row_Id int, ContractId bigint, perc dec(6,3),daysGroup nVarChar(50))
                                                                                                                                                                   

                                                                                                                                                                                                                                                             
	insert into @tFine (Row_Id, ContractId, perc, daysGroup)
                                                                                                                                                                                                    
	select  Row_Id int, ContractId,t1.koef,t1.daysGroup
                                                                                                                                                                                                         
	from 
                                                                                                                                                                                                                                                       
		@tRes t0
                                                                                                                                                                                                                                                   
		join fineKoef t1 on t0.dpd between t1.startDay 
                                                                                                                                                                                                            
			and t1.endDay and t1.bonusGroup = @bonusGroup and  t1.project = 'UkrSib' 
                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	update @tRes
                                                                                                                                                                                                                                                
	set perc = t1.perc, daysGroup = t1.daysGroup
                                                                                                                                                                                                                
	from @tRes t0
                                                                                                                                                                                                                                               
		join @tFine t1 
                                                                                                                                                                                                                                            
			on t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	delete @tPays
                                                                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
	insert into @tPays(ContractNum, Pays)
                                                                                                                                                                                                                       
	select t0.ContractNum,sum(t1.Suma)
                                                                                                                                                                                                                          
	from @tRes t0 join inVal t1 
                                                                                                                                                                                                                                
		on t0.ContractNum = t1.ContractNum and t1.ip = @Ip
                                                                                                                                                                                                         
	group by t0.ContractNum
                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	update t0
                                                                                                                                                                                                                                                   
	set bankSumm = t1.Pays
                                                                                                                                                                                                                                      
	from @tRes t0 
                                                                                                                                                                                                                                              
		join @tPays t1 on t0.ContractNum = t1.ContractNum
                                                                                                                                                                                                          
		
                                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_addrId] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                                 
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	row_id_inn int,
                                                                                                                                                                                                                                             
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	declare @allRes table (numInn int,
                                                                                                                                                                                                                          
			Row_Id int,
                                                                                                                                                                                                                                               
			ContractId bigint,
                                                                                                                                                                                                                                        
			ContractNum nVarChar(90),
                                                                                                                                                                                                                                 
			Inn varChar(50)
                                                                                                                                                                                                                                           
		)
                                                                                                                                                                                                                                                          
	
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
	insert into @allRes(Row_Id, ContractId, ContractNum,Inn, numInn)
                                                                                                                                                                                            
	select t3.Row_Id, t0.id, t0.ContractNum, t3.Inn, ROW_NUMBER() OVER(PARTITION BY t3.inn ORDER BY t0.id ASC)
                                                                                                                                                  
	from 
                                                                                                                                                                                                                                                       
		CONTRACT t0
                                                                                                                                                                                                                                                
		join reestr t1 on t0.ReestrId = t1.id and t1.RNumber = @RNumber
                                                                                                                                                                                            
		join Client t2 on t0.ClientId = t2.id
                                                                                                                                                                                                                      
		join inVal t3 on t2.INN = t3.INN 
                                                                                                                                                                                                                          
	where t3.ip = @Ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,row_id_inn)
                                                                                                                                                                                               
	select Row_Id, ContractId, ContractNum,row_id + 1
                                                                                                                                                                                                           
	from @allRes t0
                                                                                                                                                                                                                                             
	where t0.numInn = 1 
                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	declare @rowIdMax int = (select max(t0.Row_Id) from inval t0 where ip = @Ip)
                                                                                                                                                                                

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,row_id_inn)
                                                                                                                                                                                               
	select ROW_NUMBER() OVER(ORDER BY t0.Row_Id ASC) + @rowIdMax,
                                                                                                                                                                                               
		ContractId, ContractNum,row_id + 1
                                                                                                                                                                                                                         
	from @allRes t0
                                                                                                                                                                                                                                             
	where t0.numInn > 1 
                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, Comment)
                                                                                                                                                                                                                          
	select t0.Row_Id,'NF' 
                                                                                                                                                                                                                                      
	from 
                                                                                                                                                                                                                                                       
		inval t0
                                                                                                                                                                                                                                                   
		left join @tRes t1 on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                                
	where t0.ip = @Ip and t1.ContractId is null
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
create function [dbo].[fn_answer] (@Ip AS binary(15))
                                                                                                                                                                                                        
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	answer nVarChar(50)
                                                                                                                                                                                                                                         
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     
	insert into @tRes(row_id,answer)
                                                                                                                                                                                                                            
	select t0.Row_Id,'ok'
                                                                                                                                                                                                                                       
	from inval t0 where t0.Ip = @ip
                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
	CREATE FUNCTION dbo.fn_diagramobjects() 
                                                                                                                                                                                                                    
	RETURNS int
                                                                                                                                                                                                                                                 
	WITH EXECUTE AS N'dbo'
                                                                                                                                                                                                                                      
	AS
                                                                                                                                                                                                                                                          
	BEGIN
                                                                                                                                                                                                                                                       
		declare @id_upgraddiagrams		int
                                                                                                                                                                                                                            
		declare @id_sysdiagrams			int
                                                                                                                                                                                                                              
		declare @id_helpdiagrams		int
                                                                                                                                                                                                                              
		declare @id_helpdiagramdefinition	int
                                                                                                                                                                                                                      
		declare @id_creatediagram	int
                                                                                                                                                                                                                              
		declare @id_renamediagram	int
                                                                                                                                                                                                                              
		declare @id_alterdiagram 	int 
                                                                                                                                                                                                                             
		declare @id_dropdiagram		int
                                                                                                                                                                                                                               
		declare @InstalledObjects	int
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
		select @InstalledObjects = 0
                                                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
		select 	@id_upgraddiagrams = object_id(N'dbo.sp_upgraddiagrams'),
                                                                                                                                                                                          
			@id_sysdiagrams = object_id(N'dbo.sysdiagrams'),
                                                                                                                                                                                                          
			@id_helpdiagrams = object_id(N'dbo.sp_helpdiagrams'),
                                                                                                                                                                                                     
			@id_helpdiagramdefinition = object_id(N'dbo.sp_helpdiagramdefinition'),
                                                                                                                                                                                   
			@id_creatediagram = object_id(N'dbo.sp_creatediagram'),
                                                                                                                                                                                                   
			@id_renamediagram = object_id(N'dbo.sp_renamediagram'),
                                                                                                                                                                                                   
			@id_alterdiagram = object_id(N'dbo.sp_alterdiagram'), 
                                                                                                                                                                                                    
			@id_dropdiagram = object_id(N'dbo.sp_dropdiagram')
                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
		if @id_upgraddiagrams is not null
                                                                                                                                                                                                                          
			select @InstalledObjects = @InstalledObjects + 1
                                                                                                                                                                                                          
		if @id_sysdiagrams is not null
                                                                                                                                                                                                                             
			select @InstalledObjects = @InstalledObjects + 2
                                                                                                                                                                                                          
		if @id_helpdiagrams is not null
                                                                                                                                                                                                                            
			select @InstalledObjects = @InstalledObjects + 4
                                                                                                                                                                                                          
		if @id_helpdiagramdefinition is not null
                                                                                                                                                                                                                   
			select @InstalledObjects = @InstalledObjects + 8
                                                                                                                                                                                                          
		if @id_creatediagram is not null
                                                                                                                                                                                                                           
			select @InstalledObjects = @InstalledObjects + 16
                                                                                                                                                                                                         
		if @id_renamediagram is not null
                                                                                                                                                                                                                           
			select @InstalledObjects = @InstalledObjects + 32
                                                                                                                                                                                                         
		if @id_alterdiagram  is not null
                                                                                                                                                                                                                           
			select @InstalledObjects = @InstalledObjects + 64
                                                                                                                                                                                                         
		if @id_dropdiagram is not null
                                                                                                                                                                                                                             
			select @InstalledObjects = @InstalledObjects + 128
                                                                                                                                                                                                        
		
                                                                                                                                                                                                                                                           
		return @InstalledObjects 
                                                                                                                                                                                                                                  
	END
                                                                                                                                                                                                                                                         
	                                                                                                                                                                                                                                                              
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
create function [dbo].[fn_getAlfaBusId](@ip varChar(15), @contragentId int) 
                                                                                                                                                                                 
returns
                                                                                                                                                                                                                                                      
@tRes table (
                                                                                                                                                                                                                                                
	Row_Id int primary key, 
                                                                                                                                                                                                                                    
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90), 
                                                                                                                                                                                                                                  
	Comment nVarChar(500)  default ''
                                                                                                                                                                                                                           
) 
                                                                                                                                                                                                                                                           
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	declare @tBankId table (id int, ContractId bigint) 
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	insert into @tBankId(id,ContractId)
                                                                                                                                                                                                                         
	select t0.Row_Id,ContractId from fn_IdbyBankId(@ip,@contragentId) t0
                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id,ContractId)
                                                                                                                                                                                                                        
	select id, max(ContractId) 
                                                                                                                                                                                                                                 
	from 
                                                                                                                                                                                                                                                       
	(
                                                                                                                                                                                                                                                           
		select id, ContractId from @tBankId
                                                                                                                                                                                                                        
	) t00
                                                                                                                                                                                                                                                       
	group by id
                                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	update @tRes set ContractNum = t1.ContractNum from @tRes t0 join Contract t1 on t0.ContractId = t1.id
                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	declare @tOthers table (id int, ContractId bigint, ContractNum nVarChar(90))
                                                                                                                                                                                
	declare @tIndex table (ind int identity(1,1), id int )
                                                                                                                                                                                                      
	declare @suff varChar(3), @iTable int = 0, @iTable_max int = 1
                                                                                                                                                                                              
	while @iTable<@iTable_max
                                                                                                                                                                                                                                   
	begin
                                                                                                                                                                                                                                                       
		set @iTable = @iTable +1 
                                                                                                                                                                                                                                  
	
                                                                                                                                                                                                                                                            
		delete @tOthers
                                                                                                                                                                                                                                            
		delete @tIndex
                                                                                                                                                                                                                                             
	
                                                                                                                                                                                                                                                            
		if @iTable = 1
                                                                                                                                                                                                                                             
		begin
                                                                                                                                                                                                                                                      
		
                                                                                                                                                                                                                                                           
			insert into @tOthers (id, ContractId, ContractNum)
                                                                                                                                                                                                        
			select t0.id, t0.ContractId, t2.ContractNum
                                                                                                                                                                                                               
			from @tBankId t0
                                                                                                                                                                                                                                          
				left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                  
					and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
				join Contract t2 on t0.ContractId = t2.id
                                                                                                                                                                                                                
			where t1.Row_Id is null
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
			insert @tIndex (id)
                                                                                                                                                                                                                                       
			select distinct id from @tOthers 
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
			set @suff = 'BId'
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
		declare @i int, @i_max int, @result nVarChar(500) , @id int
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
		select @i=0, @i_max = (select max(ind) from @tIndex)
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
		while @i<@i_max
                                                                                                                                                                                                                                            
		begin
                                                                                                                                                                                                                                                      
			set @i = @i+1
                                                                                                                                                                                                                                             
			select @id = (select max(id) from @tIndex t0 where t0.ind = @i)
                                                                                                                                                                                           
			set @result = '';
                                                                                                                                                                                                                                         
			select @result = @result + ',' + t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                        

                                                                                                                                                                                                                                                             
			--select t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
			update @tRes set Comment = Comment + @suff + ' (' + right(@result, len(@result) - 1) + ');' where Row_Id = @id
                                                                                                                                            
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	end
                                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getAlfaByBusIdStr](@ip varChar(15), @contragentStr varChar(500)) 
                                                                                                                                                                  
returns
                                                                                                                                                                                                                                                      
@tRes table (
                                                                                                                                                                                                                                                
	Row_Id int primary key, 
                                                                                                                                                                                                                                    
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90), 
                                                                                                                                                                                                                                  
	Comment nVarChar(500)  default ''
                                                                                                                                                                                                                           
) 
                                                                                                                                                                                                                                                           
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	declare @tBankId table (id int, ContractId bigint) 
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	insert into @tBankId(id,ContractId)
                                                                                                                                                                                                                         
	select t0.Row_Id,ContractId from fn_IdbyBankIdStr(@ip,@contragentStr,',') t0
                                                                                                                                                                                

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id,ContractId)
                                                                                                                                                                                                                        
	select id, max(ContractId) 
                                                                                                                                                                                                                                 
	from 
                                                                                                                                                                                                                                                       
	(
                                                                                                                                                                                                                                                           
		select id, ContractId from @tBankId
                                                                                                                                                                                                                        
	) t00
                                                                                                                                                                                                                                                       
	group by id
                                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	update @tRes set ContractNum = t1.ContractNum from @tRes t0 join Contract t1 on t0.ContractId = t1.id
                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	declare @tOthers table (id int, ContractId bigint, ContractNum nVarChar(90))
                                                                                                                                                                                
	declare @tIndex table (ind int identity(1,1), id int )
                                                                                                                                                                                                      
	declare @suff varChar(3), @iTable int = 0, @iTable_max int = 1
                                                                                                                                                                                              
	while @iTable<@iTable_max
                                                                                                                                                                                                                                   
	begin
                                                                                                                                                                                                                                                       
		set @iTable = @iTable +1 
                                                                                                                                                                                                                                  
	
                                                                                                                                                                                                                                                            
		delete @tOthers
                                                                                                                                                                                                                                            
		delete @tIndex
                                                                                                                                                                                                                                             
	
                                                                                                                                                                                                                                                            
		if @iTable = 1
                                                                                                                                                                                                                                             
		begin
                                                                                                                                                                                                                                                      
		
                                                                                                                                                                                                                                                           
			insert into @tOthers (id, ContractId, ContractNum)
                                                                                                                                                                                                        
			select t0.id, t0.ContractId, t2.ContractNum
                                                                                                                                                                                                               
			from @tBankId t0
                                                                                                                                                                                                                                          
				left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                  
					and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
				join Contract t2 on t0.ContractId = t2.id
                                                                                                                                                                                                                
			where t1.Row_Id is null
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
			insert @tIndex (id)
                                                                                                                                                                                                                                       
			select distinct id from @tOthers 
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
			set @suff = 'BId'
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
		declare @i int, @i_max int, @result nVarChar(500) , @id int
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
		select @i=0, @i_max = (select max(ind) from @tIndex)
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
		while @i<@i_max
                                                                                                                                                                                                                                            
		begin
                                                                                                                                                                                                                                                      
			set @i = @i+1
                                                                                                                                                                                                                                             
			select @id = (select max(id) from @tIndex t0 where t0.ind = @i)
                                                                                                                                                                                           
			set @result = '';
                                                                                                                                                                                                                                         
			select @result = @result + ',' + t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                        

                                                                                                                                                                                                                                                             
			--select t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
			update @tRes set Comment = Comment + @suff + ' (' + right(@result, len(@result) - 1) + ');' where Row_Id = @id
                                                                                                                                            
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	end
                                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getBaseId] (@Ip AS binary(15), @RNumber varChar(100),@fileDate dateTime,@params varChar(250))
                                                                                                                                      
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	PayDate dateTime,
                                                                                                                                                                                                                                           
	StList varChar(150) default '',
                                                                                                                                                                                                                             
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Fio varChar(500),
                                                                                                                                                                                                                                           
	dbRecord varChar(10) default '',
                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
--declare @ip binary(4) = 0x0A011E38, @ContragentId int = 423 
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
declare @tParam table(par varChar(25))
                                                                                                                                                                                                                       
insert into @tParam select element from fn_strSplit(@params,',')
                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),
                                                                                                                                                                                
	Fio nVarChar(500),Inn varChar(50),DebDate varChar(10),PayDate dateTime,CurrCode nvarChar(50))
                                                                                                                                                               

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
if (select count(par) from @tParam where par = 'NumById') > 0
                                                                                                                                                                                                
begin
                                                                                                                                                                                                                                                        
	insert into @t_All(Row_Id, ContractId,ContractNum,Fio,Inn,DebDate,PayDate) 
                                                                                                                                                                                 
	SELECT t0.Row_Id,c.id,t0.ContractNum,
                                                                                                                                                                                                                       
		case
                                                                                                                                                                                                                                                       
			when (t0.FIO like t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName or  t0.FIO like t4.Surname + char(160)  + t4.Name +  char(160) + t4.MiddleName)
                                                                                                       
			then '' 
                                                                                                                                                                                                                                                  
			else  t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName 
                                                                                                                                                                                                  
		end,
                                                                                                                                                                                                                                                       
		case when t4.INN <> t0.INN then t4.INN else '' end,
                                                                                                                                                                                                        
		convert(varChar,@fileDate - t0.Days, 112),
                                                                                                                                                                                                                 
		DATEADD(dd, DATEDIFF(dd, 0, t0.Date), 0) 
                                                                                                                                                                                                                  
	FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                              
		join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                            
		join inval t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                            
		left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                  
	where  t0.ip = @ip 
                                                                                                                                                                                                                                         
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
if (select count(par) from @tParam where par = 'NumAsNumber') > 0
                                                                                                                                                                                            
begin
                                                                                                                                                                                                                                                        
	insert into @t_All(Row_Id, ContractId,ContractNum,Fio,Inn,DebDate,PayDate) 
                                                                                                                                                                                 
	SELECT t0.Row_Id,c.id,c.ContractNum,
                                                                                                                                                                                                                        
		case
                                                                                                                                                                                                                                                       
			when (t0.FIO like t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName or  t0.FIO like t4.Surname + char(160)  + t4.Name +  char(160) + t4.MiddleName)
                                                                                                       
			then '' 
                                                                                                                                                                                                                                                  
			else  t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName 
                                                                                                                                                                                                  
		end,
                                                                                                                                                                                                                                                       
		case when t4.INN <> t0.INN then t4.INN else '' end,
                                                                                                                                                                                                        
		convert(varChar,@fileDate - t0.Days, 112),
                                                                                                                                                                                                                 
		DATEADD(dd, DATEDIFF(dd, 0, t0.Date), 0) 
                                                                                                                                                                                                                  
	FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                              
		join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                            
		join inval t0 on c.ContractNum = 
                                                                                                                                                                                                                          
			case when ISNUMERIC(t0.ContractNum) = 1 then ltrim(str(t0.ContractNum,150)) end
                                                                                                                                                                           
			
                                                                                                                                                                                                                                                          
		left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                  
		left join @t_All t5 on t0.Row_Id = t5.Row_Id
                                                                                                                                                                                                               
	where  t0.ip = @ip and t5.Row_Id is null
                                                                                                                                                                                                                    
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
if (select count(par) from @tParam where par = 'NumByBusId') > 0
                                                                                                                                                                                             
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @t_All(Row_Id, ContractId,ContractNum,Fio,Inn,DebDate,PayDate) 
                                                                                                                                                                                 
	SELECT t2.Row_Id,c.id,c.ContractNum,
                                                                                                                                                                                                                        
		case
                                                                                                                                                                                                                                                       
			when (t2.FIO like t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName or  t2.FIO like t4.Surname + char(160)  + t4.Name +  char(160) + t4.MiddleName)
                                                                                                       
			then '' 
                                                                                                                                                                                                                                                  
			else  t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName 
                                                                                                                                                                                                  
		end,
                                                                                                                                                                                                                                                       
		case when t4.INN <> t2.INN then t4.INN else '' end,
                                                                                                                                                                                                        
		convert(varChar,@fileDate - t2.Days, 112),
                                                                                                                                                                                                                 
		DATEADD(dd, DATEDIFF(dd, 0, t2.Date), 0) 
                                                                                                                                                                                                                  
	from
                                                                                                                                                                                                                                                        
		Contract c
                                                                                                                                                                                                                                                 
		join SDSD.ComercialProject.RussiaAgreemID t0 on c.Id = t0.Agreem
                                                                                                                                                                                           
		join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                            
		join inval t2 on t0.id = t2.BusId collate Cyrillic_General_CI_AS
                                                                                                                                                                                           
		left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                  
	where  t2.ip = @ip
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
if (select count(par) from @tParam where par = 'NumById--CTX-UAH') > 0
                                                                                                                                                                                       
begin
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
	
                                                                                                                                                                                                                                                            
	insert into @t_All(Row_Id, ContractId,ContractNum,Fio,Inn,DebDate,PayDate) 
                                                                                                                                                                                 
	SELECT t0.Row_Id,isnull(c0.id,c1.id),t0.ContractNum,
                                                                                                                                                                                                        
		case
                                                                                                                                                                                                                                                       
			when (t0.FIO like t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName or  t0.FIO like t4.Surname + char(160)  + t4.Name +  char(160) + t4.MiddleName)
                                                                                                       
			then '' 
                                                                                                                                                                                                                                                  
			else  t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName 
                                                                                                                                                                                                  
		end,
                                                                                                                                                                                                                                                       
		case when t4.INN <> t0.INN then t4.INN else '' end,
                                                                                                                                                                                                        
		convert(varChar,@fileDate - t0.Days, 112),
                                                                                                                                                                                                                 
		DATEADD(dd, DATEDIFF(dd, 0, t0.Date), 0) 
                                                                                                                                                                                                                  
	FROM 
                                                                                                                                                                                                                                                       
		inval t0 
                                                                                                                                                                                                                                                  
		left join Contract AS c0 on c0.ContractNum = 'CTX' + t0.ContractNum + 'UAH' 
                                                                                                                                                                               
			and c0.ReestrId in (select id from dbo.fn_reestrByRNumbers(@RNumber))
                                                                                                                                                                                     
		left join Contract AS c1 on c1.ContractNum = 'CTX' + t0.ContractNum + 'UAH_'
                                                                                                                                                                               
			and c1.ReestrId in (select id from dbo.fn_reestrByRNumbers(@RNumber))
                                                                                                                                                                                     
		left join Client t4 on isnull(c0.ClientId,c1.ClientId) = t4.id
                                                                                                                                                                                             
	where  t0.ip = @ip  and (c0.id is not null or c1.id is not null )
                                                                                                                                                                                           
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,Fio,Inn,Comment,DebDate,PayDate) 
                                                                                                                                                                           
select t0.Row_Id, t0.ContractId,t0.ContractNum,Fio,Inn,
                                                                                                                                                                                                      
	case when colRec > 1 then 'col:' + cast(colRec as varChar(5)) + ';' else '' end,
                                                                                                                                                                            
	t0.DebDate,
                                                                                                                                                                                                                                                 
	PayDate
                                                                                                                                                                                                                                                     
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId, count(*) colRec
                                                                                                                                                                                                  
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
insert into @tRes (Row_Id,Comment)
                                                                                                                                                                                                                           
select t0.row_id,'NF;' 
                                                                                                                                                                                                                                      
from inval t0
                                                                                                                                                                                                                                                
	left join @tRes t1 
                                                                                                                                                                                                                                         
		on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                                                   
where t1.Row_Id is null and t0.Ip = @ip
                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
if (select count(par) from @tParam where par = 'stopList') > 0
                                                                                                                                                                                               
begin
                                                                                                                                                                                                                                                        
	update @tRes
                                                                                                                                                                                                                                                
	set StList = t2.Name
                                                                                                                                                                                                                                        
	from @tRes t0 
                                                                                                                                                                                                                                              
		join StopList t1 
                                                                                                                                                                                                                                          
			on t0.ContractId = t1.ItemId and t1.StopDate is null
                                                                                                                                                                                                      
		 join Dictionary t2 on t1.ReasonId = t2.id
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
-- drop table #t1
                                                                                                                                                                                                                                            
-- select * into #t1 from @tRes
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getBaseIdPay] (@Ip AS binary(15), @RNumber varChar(100),@params varChar(250))
                                                                                                                                                      
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Fio varChar(500),
                                                                                                                                                                                                                                           
	dbRecord varChar(10) default '',
                                                                                                                                                                                                                            
	PayDate dateTime,
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id,ContractId,ContractNum,Inn,Fio,Comment,suma,PayDate)
                                                                                                                                                                                
select Row_Id,max(t0.ContractId),max(t0.ContractNum),max(t0.Inn),max(t0.Fio),max(t0.Comment) ,sum(t2.SummPay),max(PayDate)
                                                                                                                                   
from fn_getBaseId(@ip,@RNumber,null,@params) t0
                                                                                                                                                                                                              
		left join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                          
			on t0.ContractId = t2.ContractId and t0.PayDate = t2.DatePay
                                                                                                                                                                                              
group by t0.Row_Id
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @nDate dateTime, @now dateTime = getDate()
                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
set @nDate = DATEFROMPARTS (year(@now),1,1 )
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
update @tRes set Comment = isnull(Comment,'') + 'DT;' where  PayDate < @nDate or PayDate > @now 
                                                                                                                                                             

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
 CREATE function [dbo].[fn_getBaseStopList] (@Ip AS binary(15), @RNumber varChar(100),@param varChar(150) ='NumById' )
                                                                                                                                       
returns 
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
-- select distinct ip from inVal
                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
--declare @Ip AS binary(15) = 0x0A011E23, @RNumber varChar(100) = '2184, 2185'
                                                                                                                                                                               

                                                                                                                                                                                                                                                             
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),Comment VarChar(250) )
                                                                                                                                                          

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,Comment) 
                                                                                                                                                                                                  
select Row_Id, ContractId,ContractNum,Comment
                                                                                                                                                                                                                
from [dbo].[fn_getBaseId](@ip,@RNumber,null,@param)
                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id,Comment) 
                                                                                                                                                                                                                           
select t0.Row_Id,t0.Comment
                                                                                                                                                                                                                                  
from @t_All t0
                                                                                                                                                                                                                                               
where t0.ContractNum is null
                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
declare @maxNom int
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
select @maxNom = max(Row_id)  from inval where ip = @Ip
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id,ContractId, ContractNum) 
                                                                                                                                                                                                           
SELECT  ROW_NUMBER()  OVER(ORDER BY c.ContractNum ASC) + @maxNom,c.id,c.ContractNum
                                                                                                                                                                          
FROM dbo.Contract AS c
                                                                                                                                                                                                                                       
	join dbo.fn_reestrByRNumbers(@RNumber) t5 on c.ReestrId = t5.id 
                                                                                                                                                                                            
--	join inval t0 on c.ContractNum = t0.ContractNum and t0.Ip = @Ip
                                                                                                                                                                                           
	left join @t_All t2 on c.ContractNum = t2.ContractNum -- and t2.ContractNum not in ('380500161489')
                                                                                                                                                         
	left join StopList t3 on c.id = t3.ItemId
                                                                                                                                                                                                                   
where t2.Row_Id is null and t3.ItemId is null
                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getCahsUpUpd] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                           
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	StList varChar(5) default '',
                                                                                                                                                                                                                               
	Fio varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,StList,Fio,Comment)
                                                                                                                                                                                       
	select Row_Id, ContractId, ContractNum,StList,Fio,Comment
                                                                                                                                                                                                   
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,null,'NumById,stopList')
                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
create function [dbo].[fn_getCashUpPay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                           
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Fio varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, suma,Fio,Comment)
                                                                                                                                                                                        
	select Row_Id, ContractId, ContractNum, suma,Fio,Comment
                                                                                                                                                                                                    
	from [dbo].[fn_getBaseIdPay] (@Ip, @RNumber,'NumById')
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getCashUpStopList] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                      
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,Comment)
                                                                                                                                                                                                  
	select t0.Row_Id, t0.ContractId, t0.ContractNum, t0.Comment
                                                                                                                                                                                                 
	from [dbo].[fn_getBaseStopList] (@Ip, @RNumber,'NumById') t0
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getCredoPay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                            
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, suma,Inn,Comment)
                                                                                                                                                                                        
	select Row_Id, ContractId, ContractNum, suma,Inn,Comment
                                                                                                                                                                                                    
	from [dbo].[fn_getBaseIdPay] (@Ip, @RNumber,'NumById')
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getCredoUpd] (@Ip AS binary(15), @RNumber varChar(100),@FileDate dateTime)
                                                                                                                                                         
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	StList varChar(150) default '',
                                                                                                                                                                                                                             
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,DebDate,StList,Comment)
                                                                                                                                                                                   
	select Row_Id, ContractId, ContractNum,DebDate,StList,Comment
                                                                                                                                                                                               
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,@FileDate,'NumById,stopList')
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
create function [dbo].fn_getFidoPay (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                               
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Inn varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, suma,Inn,Comment)
                                                                                                                                                                                        
	select Row_Id, ContractId, ContractNum, suma,Inn,Comment
                                                                                                                                                                                                    
	from [dbo].[fn_getBaseIdPay] (@Ip, @RNumber,'NumById')
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getFidoStopList] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                        
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,Comment)
                                                                                                                                                                                                  
	select t0.Row_Id, t0.ContractId, t0.ContractNum, t0.Comment
                                                                                                                                                                                                 
	from [dbo].[fn_getBaseStopList] (@Ip, @RNumber,'NumById') t0
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getFidoUpd] (@Ip AS binary(15), @RNumber varChar(100),@FileDate dateTime)
                                                                                                                                                          
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	StList varChar(150) default '',
                                                                                                                                                                                                                             
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,DebDate,StList,Inn,Comment)
                                                                                                                                                                               
	select Row_Id, ContractId, ContractNum,DebDate,StList,Inn,Comment
                                                                                                                                                                                           
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,@FileDate,'NumById,stopList')
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getImexUpd] (@Ip AS binary(15), @RNumber varChar(100),@FileDate dateTime)
                                                                                                                                                          
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	StList varChar(150) default '',
                                                                                                                                                                                                                             
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,DebDate,StList,Inn,Comment)
                                                                                                                                                                               
	select Row_Id, ContractId, ContractNum,DebDate,StList,Inn,Comment
                                                                                                                                                                                           
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,@FileDate,'NumById,stopList')
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
create function [dbo].fn_getImpexUpPay (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                            
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Inn varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, suma,Inn,Comment)
                                                                                                                                                                                        
	select Row_Id, ContractId, ContractNum, suma,Inn,Comment
                                                                                                                                                                                                    
	from [dbo].[fn_getBaseIdPay] (@Ip, @RNumber,'NumById')
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getNikoId] (@Ip AS binary(15), @RNumber varChar(100),@fileDate dateTime)
                                                                                                                                                           
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	StList varChar(5) default '',
                                                                                                                                                                                                                               
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
--declare @ip binary(4) = 0x0A011E38, @ContragentId int = 423 
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),CurrCode nvarChar(50),Inn varChar(50),DebDate varChar(10))
                                                                                                                      

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,Inn,DebDate) 
                                                                                                                                                                                              
SELECT t0.Row_Id,c.id,t0.ContractNum,
                                                                                                                                                                                                                        
	case when t4.INN <> t0.INN then t4.INN else '' end,
                                                                                                                                                                                                         
	convert(varChar,@fileDate - t0.Days, 112)
                                                                                                                                                                                                                   
FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                               
	join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                             
	join inval t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                             
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,inn,Comment,DebDate) 
                                                                                                                                                                                       
select t0.Row_Id, t0.ContractId,t0.ContractNum,inn,
                                                                                                                                                                                                          
	case when colRec > 1 then 'col:' + cast(colRec as varChar(5)) + ';' else '' end,
                                                                                                                                                                            
	t0.DebDate
                                                                                                                                                                                                                                                  
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId, count(*) colRec
                                                                                                                                                                                                  
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
update @tRes
                                                                                                                                                                                                                                                 
set StList = 'Yes'
                                                                                                                                                                                                                                           
from @tRes t0 
                                                                                                                                                                                                                                               
	join StopList t1 
                                                                                                                                                                                                                                           
		on t0.ContractId = t1.ItemId and t1.StopDate is null
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
-- drop table #t1
                                                                                                                                                                                                                                            
-- select * into #t1 from @tRes
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getNikoPay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                             
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	FIO nvarChar(500),
                                                                                                                                                                                                                                          
	DateSQL datetime,
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
--declare @ip binary(4) = 0x0A011E38, @ContragentId int = 423 
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),Fio nvarChar(500),DateSQL DateTime)
                                                                                                                                             

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,Fio,DateSQL) 
                                                                                                                                                                                              
SELECT t0.Row_Id,c.id,c.ContractNum,
                                                                                                                                                                                                                         
	case
                                                                                                                                                                                                                                                        
		when (t0.FIO like t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName or  t0.FIO like t4.Surname + char(160)  + t4.Name +  char(160) + t4.MiddleName)
                                                                                                        
		then '' 
                                                                                                                                                                                                                                                   
		else  t4.Surname + ' '  + t4.Name + ' ' + t4.MiddleName 
                                                                                                                                                                                                   
	end,
                                                                                                                                                                                                                                                        
	DATEADD(dd, DATEDIFF(dd, 0, t0.Date), 0) 
                                                                                                                                                                                                                   
FROM Contract AS c
                                                                                                                                                                                                                                           
	join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                             
	join multipleNiko t5 on c.ContractNum = t5.CaseNum
                                                                                                                                                                                                          
	join inval t0 on t0.ContractNum = t5.CaseId
                                                                                                                                                                                                                 
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @nDate dateTime, @now dateTime = getDate()
                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
set @nDate = DATEFROMPARTS (year(@now),1,1 )
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,Fio,DateSQL,Comment) 
                                                                                                                                                                                       
select t0.Row_Id, t0.ContractId,t0.ContractNum,t0.Fio,DateSQL,
                                                                                                                                                                                               
	case when colRec > 1 then 'col:' + cast(colRec as varChar(5)) + ';' else '' end
                                                                                                                                                                             
	+ case when  DateSQL < @nDate or DateSQL > @now  then 'DT;'  else '' end
                                                                                                                                                                                    
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId, count(*) colRec
                                                                                                                                                                                                  
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @pay table(row_id int, suma money)
                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
insert into @pay(row_id, suma)
                                                                                                                                                                                                                               
select t0.Row_Id,sum(t2.SummPay)
                                                                                                                                                                                                                             
from @tRes t0
                                                                                                                                                                                                                                                
	left join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                           
		on t0.ContractId = t2.ContractId and t0.DateSQL = t2.DatePay
                                                                                                                                                                                               
group by t0.Row_Id
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
update @tRes 
                                                                                                                                                                                                                                                
set suma = t2.suma
                                                                                                                                                                                                                                           
from @tRes t1 
                                                                                                                                                                                                                                               
	join @pay t2 on t1.Row_Id = t2.row_id
                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getNikoStopList] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                        
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
--declare @ip binary(4) = 0x0A011E38, @ContragentId int = 423 
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90))
                                                                                                                                                                                

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum) 
                                                                                                                                                                                                          
SELECT t0.Row_Id,c.id,t0.ContractNum
                                                                                                                                                                                                                         
FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                               
	join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                             
	join inval t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                             
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractNum,Comment) 
                                                                                                                                                                                                              
select t0.Row_Id,t0.ContractNum,'NF;'
                                                                                                                                                                                                                        
from inVal t0
                                                                                                                                                                                                                                                
  left join @t_All t1 on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                               
where t1.Row_Id is null 
                                                                                                                                                                                                                                     
	and t0.ip = @ip 
                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
declare @maxNom int
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
select @maxNom = max(Row_id)  from inval where ip = @Ip
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id,ContractId, ContractNum) 
                                                                                                                                                                                                           
SELECT  ROW_NUMBER()  OVER(ORDER BY c.ContractNum ASC) + @maxNom,c.id,c.ContractNum
                                                                                                                                                                          
FROM dbo.Contract AS c
                                                                                                                                                                                                                                       
	join dbo.fn_reestrByRNumbers(@RNumber) t5 on c.ReestrId = t5.id 
                                                                                                                                                                                            
--	join inval t0 on c.ContractNum = t0.ContractNum and t0.Ip = @Ip
                                                                                                                                                                                           
	left join @t_All t2 on c.ContractNum = t2.ContractNum -- and t2.ContractNum not in ('380500161489')
                                                                                                                                                         
	left join StopList t3 on c.id = t3.ItemId and t3.StopDate is null
                                                                                                                                                                                           
where t2.Row_Id is null and t3.ItemId is null
                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getNikoUpd] (@Ip AS binary(15), @RNumber varChar(100),@fileDate dateTime)
                                                                                                                                                          
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	StList varChar(150) default '',
                                                                                                                                                                                                                             
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,DebDate,StList,Inn,Comment)
                                                                                                                                                                               
	select Row_Id, ContractId, ContractNum,DebDate,StList,Inn,Comment
                                                                                                                                                                                           
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,@FileDate,'NumById,stopList')
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getPlatinumPay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                         
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
--declare @ip binary(4) = 0x0A011E38, @ContragentId int = 423 
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),Inn varChar(50))
                                                                                                                                                                

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,Inn) 
                                                                                                                                                                                                      
SELECT t0.Row_Id,c.id,t0.ContractNum,
                                                                                                                                                                                                                        
	case when t4.INN <> t0.INN then t4.INN else '' end
                                                                                                                                                                                                          
FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                               
	join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                             
	join inval t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                             
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,inn,Comment) 
                                                                                                                                                                                               
select t0.Row_Id, t0.ContractId,t0.ContractNum,inn,
                                                                                                                                                                                                          
	case when colRec > 1 then 'col:' + cast(colRec as varChar(5)) + ';' else '' end
                                                                                                                                                                             
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId, count(*) colRec
                                                                                                                                                                                                  
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
declare @pay table(row_id int, suma money)
                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
insert into @pay(row_id, suma)
                                                                                                                                                                                                                               
select t0.Row_Id,sum(t2.SummPay)
                                                                                                                                                                                                                             
from @tRes t0
                                                                                                                                                                                                                                                
	join inval t1 on t0.Row_Id = t1.Row_Id and t1.Ip = @ip
                                                                                                                                                                                                      
	left join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                           
		on t0.ContractId = t2.ContractId and t1.Date = t2.DatePay
                                                                                                                                                                                                  
group by t0.Row_Id
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
update @tRes 
                                                                                                                                                                                                                                                
set suma = t2.suma
                                                                                                                                                                                                                                           
from @tRes t1 
                                                                                                                                                                                                                                               
	join @pay t2 on t1.Row_Id = t2.row_id
                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
create function [dbo].[fn_getPlatinumUpd] (@Ip AS binary(15), @RNumber varChar(100),@fileDate dateTime)
                                                                                                                                                      
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	StList varChar(5) default '',
                                                                                                                                                                                                                               
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
--declare @ip binary(4) = 0x0A011E38, @ContragentId int = 423 
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),CurrCode nvarChar(50),Inn varChar(50),DebDate varChar(10))
                                                                                                                      

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,Inn,DebDate) 
                                                                                                                                                                                              
SELECT t0.Row_Id,c.id,t0.ContractNum,
                                                                                                                                                                                                                        
	case when t4.INN <> t0.INN then t4.INN else '' end,
                                                                                                                                                                                                         
	convert(varChar,@fileDate - t0.Days, 112)
                                                                                                                                                                                                                   
FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                               
	join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                             
	join inval t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                             
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
	left join Currency t2 on t0.CurrCode = t2.[CurrCode]
                                                                                                                                                                                                        
	left join Currency t3 on c.CurrencyId= t3.id
                                                                                                                                                                                                                
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,inn,Comment,DebDate) 
                                                                                                                                                                                       
select t0.Row_Id, t0.ContractId,t0.ContractNum,inn,
                                                                                                                                                                                                          
	case when colRec > 1 then 'col:' + cast(colRec as varChar(5)) + ';' else '' end,
                                                                                                                                                                            
	t0.DebDate
                                                                                                                                                                                                                                                  
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId, count(*) colRec
                                                                                                                                                                                                  
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
update @tRes
                                                                                                                                                                                                                                                 
set StList = 'Yes'
                                                                                                                                                                                                                                           
from @tRes t0 
                                                                                                                                                                                                                                               
	join StopList t1 
                                                                                                                                                                                                                                           
		on t0.ContractId = t1.ItemId and t1.StopDate is null
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
-- drop table #t1
                                                                                                                                                                                                                                            
-- select * into #t1 from @tRes
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getPravexId] (@Ip AS binary(15), @RNumber Int,@fileDate datetime)
                                                                                                                                                                  
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Inn varChar(50),
                                                                                                                                                                                                                                            
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	dbRecord varChar(10) default '',
                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
declare @maxDate dateTime = '29990101'
                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
declare @t_active table (Row_Id int,ContractNum nVarChar(90),DocDate datetime,dbRecord varChar(10) default '', active bit)
                                                                                                                                   

                                                                                                                                                                                                                                                             
--update inval set Date = '29990101' where ip = @ip and Date < '18500101'
                                                                                                                                                                                    

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
insert into @t_active(DocDate,ContractNum,Row_Id,active)
                                                                                                                                                                                                     
select t00.Date, t00.ContractNum,max(t00.Row_Id) Row_Id, 1
                                                                                                                                                                                                   
from inval t00
                                                                                                                                                                                                                                               
join
                                                                                                                                                                                                                                                         
(
                                                                                                                                                                                                                                                            
	select ContractNum, max(isnull(t0.Date,@maxDate)) Date 
                                                                                                                                                                                                     
	from inval t0 where t0.Ip = @ip 
                                                                                                                                                                                                                            
	group by ContractNum
                                                                                                                                                                                                                                        
) t11 on t00.ContractNum = t11.ContractNum 
                                                                                                                                                                                                                  
	and isnull(t00.Date,@maxDate) = t11.Date and t00.ip = @ip
                                                                                                                                                                                                   
group by t00.Date,t00.ContractNum
                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
insert into @t_active(row_id,DocDate,ContractNum,dbRecord,active)
                                                                                                                                                                                            
select t1.Row_Id,t1.Date, t1.ContractNum,t0.Row_Id+1, 0 
                                                                                                                                                                                                     
from @t_active t0 
                                                                                                                                                                                                                                           
	join inval t1 on t0.ContractNum = t1.ContractNum
                                                                                                                                                                                                            
		and t0.Row_Id <> t1.Row_Id and t1.ip = @ip
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
--select * from @t_active order by row_id
                                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),DocDate datetime,
                                                                                                                                                               
	Inn varChar(50),DebDate varChar(10),dbRecord varChar(10),Comment VarChar(250) default '')
                                                                                                                                                                   

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,Inn,DebDate,DocDate,dbRecord) 
                                                                                                                                                                             
SELECT t0.Row_Id,c.id,t0.ContractNum,
                                                                                                                                                                                                                        
	case when t4.INN <> t0.INN then t4.INN else '' end,
                                                                                                                                                                                                         
	convert(varChar,@fileDate - t0.Days, 112),
                                                                                                                                                                                                                  
	t6.DocDate,
                                                                                                                                                                                                                                                 
	dbRecord
                                                                                                                                                                                                                                                    
FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                               
	join reestr t5 on c.ReestrId = t5.id and t5.RNumber = @RNumber
                                                                                                                                                                                              
	join inval t0 on c.ContractNum = t0.ContractNum 
                                                                                                                                                                                                            
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
	join @t_active t6 on t0.Row_Id = t6.Row_Id
                                                                                                                                                                                                                  
where  t0.ip = @ip and t6.active = 1
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,Inn,DebDate,DocDate,dbRecord,Comment) 
                                                                                                                                                                     
SELECT t0.Row_Id,c.id,c.ContractNum,
                                                                                                                                                                                                                         
	case when t4.INN <> t0.INN then t4.INN else '' end,
                                                                                                                                                                                                         
	convert(varChar,@fileDate - t0.Days, 112),
                                                                                                                                                                                                                  
	t6.DocDate,
                                                                                                                                                                                                                                                 
	dbRecord,
                                                                                                                                                                                                                                                   
	'MC;'
                                                                                                                                                                                                                                                       
FROM multipleContr AS c
                                                                                                                                                                                                                                      
	join reestr t5 on c.ReestrId = t5.id and t5.RNumber = @RNumber
                                                                                                                                                                                              
	join inval t0 on c.DocNum = t0.ContractNum 
                                                                                                                                                                                                                 
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
	join @t_active t6 on t0.Row_Id = t6.Row_Id
                                                                                                                                                                                                                  
where  t0.ip = @ip and t6.active = 1
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,DebDate,inn,Comment) 
                                                                                                                                                                                       
select t0.Row_Id, t0.ContractId,t0.ContractNum,DebDate,inn,Comment
                                                                                                                                                                                           
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId
                                                                                                                                                                                                                   
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @tOthers table (id int, ContractId bigint, ContractNum nVarChar(90))
                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
insert @tOthers(id,ContractId,ContractNum )
                                                                                                                                                                                                                  
select t0.Row_id,t0.ContractId,t0.ContractNum
                                                                                                                                                                                                                
from @t_All t0 
                                                                                                                                                                                                                                              
	left join @tRes t1 on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                                 
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          
where t1.ContractId is null
                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
declare @tIndex table (ind int identity(1,1), id int )
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
insert @tIndex(id)
                                                                                                                                                                                                                                           
select distinct id from @tOthers
                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @i int, @i_max int, @comm nVarChar(500) , @id int
                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
select @i=0, @i_max = (select max(ind) from @tIndex)
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
while @i<@i_max
                                                                                                                                                                                                                                              
begin
                                                                                                                                                                                                                                                        
	set @i = @i+1
                                                                                                                                                                                                                                               
	select @id = (select max(id) from @tIndex t0 where t0.ind = @i)
                                                                                                                                                                                             
	set @comm = '';
                                                                                                                                                                                                                                             
	select @comm = @comm + ',' + t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                              

                                                                                                                                                                                                                                                             
	--select t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	update @tRes set Comment = Comment + right(@comm, len(@comm) - 1) + ';' where Row_Id = @id
                                                                                                                                                                  
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
insert into @tRes (Row_Id,ContractNum,dbRecord)
                                                                                                                                                                                                              
select row_id,ContractNum,dbRecord 
                                                                                                                                                                                                                          
from @t_active t0
                                                                                                                                                                                                                                            
where t0.active  = 0
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
-- drop table #t1
                                                                                                                                                                                                                                            
-- select * into #t1 from @tRes
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
 
                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getPumbAct](@ip BINARY(4), @RNumber varChar(100),
                                                                                                                                                                                  
	@dateBeg dateTime,@dateEnd dateTime, @bonusGroup varChar(50)) 
                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
returns
                                                                                                                                                                                                                                                      
@tRes table (
                                                                                                                                                                                                                                                
	Row_Id int primary key, 
                                                                                                                                                                                                                                    
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90), 
                                                                                                                                                                                                                                  
	suma money default 0,
                                                                                                                                                                                                                                       
	Inn varChar(50),
                                                                                                                                                                                                                                            
	Fio nVarChar(500),
                                                                                                                                                                                                                                          
	KOef numeric(8,3),
                                                                                                                                                                                                                                          
	Comment VarChar(500)  default '',
                                                                                                                                                                                                                           
	Comment2 VarChar(50)  default ''
                                                                                                                                                                                                                            
) 
                                                                                                                                                                                                                                                           
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
	declare @Reestr table (id int)
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
	declare @inVal table(row_id int, ContractNum varChar(150))
                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	insert into @inVal (row_id,ContractNum)
                                                                                                                                                                                                                     
	select row_id, max(ISNULL(t1.caseID,t0.ContractNum)) 
                                                                                                                                                                                                       
	from inval t0
                                                                                                                                                                                                                                               
		left join multiplePUMB t1 on t0.ContractNum = t1.caseNum
                                                                                                                                                                                                   
	where t0.ip = @ip
                                                                                                                                                                                                                                           
	group by row_id
                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	insert into @Reestr(id)
                                                                                                                                                                                                                                     
	select id from reestr t0 
                                                                                                                                                                                                                                   
	where t0.RNumber in (select element from fn_strSplit(@RNumber,','))
                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	declare @tNumber table (id int, ContractId bigint) 
                                                                                                                                                                                                         
	declare @tBankId table (id int, ContractId bigint) 
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	insert into @tNumber(id,ContractId)
                                                                                                                                                                                                                         
	SELECT t0.Row_Id,c.id
                                                                                                                                                                                                                                       
	FROM Contract AS c
                                                                                                                                                                                                                                          
		join @Reestr t1 on c.ReestrId = t1.Id
                                                                                                                                                                                                                      
		join @inVal t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                           
	--where  t0.ip = @ip
                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
--	select t0.Row_Id,ContractId from fn_IdbyNumber(@ip,@contragentId) t0
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	insert into @tBankId(id,ContractId)
                                                                                                                                                                                                                         
	select  t2.Row_Id,max(c.id) id
                                                                                                                                                                                                                              
	from
                                                                                                                                                                                                                                                        
		Contract c
                                                                                                                                                                                                                                                 
		join SDSD.ComercialProject.RussiaAgreemID t0 on c.Id = t0.Agreem
                                                                                                                                                                                           
		join @Reestr t1 on c.ReestrId = t1.Id
                                                                                                                                                                                                                      
		join @inVal t2 on t0.id = t2.ContractNum collate Cyrillic_General_CI_AS
                                                                                                                                                                                    
	--where  t2.ip = @ip
                                                                                                                                                                                                                                        
	group by t2.Row_Id
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
--	select t0.Row_Id,ContractId from fn_IdbyBankId(@ip,@contragentId) t0
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id,ContractId)
                                                                                                                                                                                                                        
	select id, max(ContractId) 
                                                                                                                                                                                                                                 
	from 
                                                                                                                                                                                                                                                       
	(
                                                                                                                                                                                                                                                           
		select id, ContractId from @tNumber
                                                                                                                                                                                                                        
		union all
                                                                                                                                                                                                                                                  
		select id, ContractId from @tBankId
                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	) t00
                                                                                                                                                                                                                                                       
	group by id
                                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	update @tRes set ContractNum = t1.ContractNum from @tRes t0 join Contract t1 on t0.ContractId = t1.id
                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	declare @tOthers table (id int, ContractId bigint, ContractNum nVarChar(90))
                                                                                                                                                                                
	declare @tIndex table (ind int identity(1,1), id int )
                                                                                                                                                                                                      
	declare @suff varChar(3), @iTable int = 0, @iTable_max int = 3
                                                                                                                                                                                              
	while @iTable<@iTable_max
                                                                                                                                                                                                                                   
	begin
                                                                                                                                                                                                                                                       
		set @iTable = @iTable +1 
                                                                                                                                                                                                                                  
	
                                                                                                                                                                                                                                                            
		delete @tOthers
                                                                                                                                                                                                                                            
		delete @tIndex
                                                                                                                                                                                                                                             
	
                                                                                                                                                                                                                                                            
		if @iTable = 1 
                                                                                                                                                                                                                                            
		begin 
                                                                                                                                                                                                                                                     
		
                                                                                                                                                                                                                                                           
			insert into @tOthers (id, ContractId, ContractNum)
                                                                                                                                                                                                        
			select t0.id, t0.ContractId, t2.ContractNum
                                                                                                                                                                                                               
			from @tNumber t0
                                                                                                                                                                                                                                          
				left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                  
					and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
				join Contract t2 on t0.ContractId = t2.id
                                                                                                                                                                                                                
			where t1.Row_Id is null
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
			insert @tIndex (id)
                                                                                                                                                                                                                                       
			select distinct id from @tOthers 
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
			set @suff = 'Num'
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		end
                                                                                                                                                                                                                                                        
		else if @iTable = 2
                                                                                                                                                                                                                                        
		begin
                                                                                                                                                                                                                                                      
		
                                                                                                                                                                                                                                                           
			insert into @tOthers (id, ContractId, ContractNum)
                                                                                                                                                                                                        
			select t0.id, t0.ContractId, t2.ContractNum
                                                                                                                                                                                                               
			from @tBankId t0
                                                                                                                                                                                                                                          
				left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                  
					and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
				join Contract t2 on t0.ContractId = t2.id
                                                                                                                                                                                                                
			where t1.Row_Id is null
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
			insert @tIndex (id)
                                                                                                                                                                                                                                       
			select distinct id from @tOthers 
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
			set @suff = 'BId'
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
		declare @i int, @i_max int, @result nVarChar(500) , @id int
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
		select @i=0, @i_max = (select max(ind) from @tIndex)
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
		while @i<@i_max
                                                                                                                                                                                                                                            
		begin
                                                                                                                                                                                                                                                      
			set @i = @i+1
                                                                                                                                                                                                                                             
			select @id = (select max(id) from @tIndex t0 where t0.ind = @i)
                                                                                                                                                                                           
			set @result = '';
                                                                                                                                                                                                                                         
			select @result = @result + ',' + t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                        

                                                                                                                                                                                                                                                             
			--select t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
			update @tRes set Comment = Comment + @suff + ' (' + right(@result, len(@result) - 1) + ');' where Row_Id = @id
                                                                                                                                            
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	end
                                                                                                                                                                                                                                                         
------------------------
                                                                                                                                                                                                                                     
-- calculate comment2
                                                                                                                                                                                                                                        
------------------------
                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	declare @tCom2 table (id int, Comment2 VarChar(50))
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	insert @tCom2(id,Comment2)
                                                                                                                                                                                                                                  
	select row_id,
                                                                                                                                                                                                                                              
		case when t2.id is null then 'Num;' else '' end
                                                                                                                                                                                                            
		+ case when t3.id is null then 'BId;' else '' end
                                                                                                                                                                                                          
	
                                                                                                                                                                                                                                                            
	from inval t0
                                                                                                                                                                                                                                               
		left join @tNumber t2 on t0.Row_Id = t2.id
                                                                                                                                                                                                                 
		left join @tBankId t3 on t0.Row_Id = t3.id
                                                                                                                                                                                                                 
	where ip = @ip
                                                                                                                                                                                                                                              
	 and (t2.id is null or t3.id is null)
                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set Comment2 = t1.Comment2
                                                                                                                                                                                                                                  
	from @tRes t0 join @tCom2 t1 on t0.Row_Id = t1.id
                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
	insert into @tRes (Row_Id,Comment2)
                                                                                                                                                                                                                         
	select t0.id,t0.Comment2 
                                                                                                                                                                                                                                   
	from @tCom2 t0 
                                                                                                                                                                                                                                             
		left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                    
	where t1.Row_Id is null
                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	declare @pay table(row_id int, suma money)
                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	insert into @pay(row_id, suma)
                                                                                                                                                                                                                              
	select t0.Row_Id,sum(t2.SummPay)
                                                                                                                                                                                                                            
	from @tRes t0
                                                                                                                                                                                                                                               
		join inval t1 on t0.Row_Id = t1.Row_Id and t1.Ip = @ip
                                                                                                                                                                                                     
		left join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                          
			on t0.ContractId = t2.ContractId and t2.DatePay between @dateBeg and @dateEnd
                                                                                                                                                                             
	group by t0.Row_Id
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set Inn = t1.INN, Fio = t2.Surname + ' '  + t2.Name + ' ' + t2.MiddleName, Koef = t3.koef
                                                                                                                                                                   
	from @tRes t0
                                                                                                                                                                                                                                               
		join inVal t1 on t0.Row_Id = t1.Row_Id and t1.ip = @ip
                                                                                                                                                                                                     
		join Client t2 on t1.INN = t2.INN
                                                                                                                                                                                                                          
		left join fineKoef t3 on t1.fio = t3.daysGroup and t3.bonusGroup = @bonusGroup and t3.project = 'PUMB'
                                                                                                                                                     

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set suma = t2.suma
                                                                                                                                                                                                                                          
	from @tRes t1 
                                                                                                                                                                                                                                              
		join @pay t2 on t1.Row_Id = t2.row_id
                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getPUMBId] (@Ip AS binary(15), @RNumber varChar(100),@fileDate dateTime)
                                                                                                                                                           
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	StList varChar(5) default '',
                                                                                                                                                                                                                               
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
declare @tReestr table (RNumber int)
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
insert into @tReestr(RNumber)
                                                                                                                                                                                                                                
select cast (element as  int) from  fn_strSplit(@RNumber,',')
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),DDays float)
                                                                                                                                                                    

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,DDays) 
                                                                                                                                                                                                    
SELECT t0.Row_Id,c.id,t0.ContractNum,t0.Days
                                                                                                                                                                                                                 
FROM dbo.Contract AS c
                                                                                                                                                                                                                                       
	join reestr t5 on c.ReestrId = t5.id and t5.RNumber in (select RNumber from @tReestr)
                                                                                                                                                                       
	join inval t0 on c.ContractNum = t0.ContractNum and t0.Ip = @Ip
                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,DebDate) 
                                                                                                                                                                                                   
select t0.Row_Id, t0.ContractId,t0.ContractNum, convert(varChar,@fileDate - t0.DDays, 112)
                                                                                                                                                                   
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId
                                                                                                                                                                                                                   
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @tOthers table (id int, ContractId bigint, ContractNum nVarChar(90))
                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
insert @tOthers(id,ContractId,ContractNum )
                                                                                                                                                                                                                  
select t0.Row_id,t0.ContractId,t0.ContractNum
                                                                                                                                                                                                                
from @t_All t0 
                                                                                                                                                                                                                                              
	left join @tRes t1 on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                                 
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          
where t1.ContractId is null
                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
declare @tIndex table (ind int identity(1,1), id int )
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
insert @tIndex(id)
                                                                                                                                                                                                                                           
select distinct id from @tOthers
                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @i int, @i_max int, @comm nVarChar(500) , @id int
                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
select @i=0, @i_max = (select max(ind) from @tIndex)
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
while @i<@i_max
                                                                                                                                                                                                                                              
begin
                                                                                                                                                                                                                                                        
	set @i = @i+1
                                                                                                                                                                                                                                               
	select @id = (select max(id) from @tIndex t0 where t0.ind = @i)
                                                                                                                                                                                             
	set @comm = '';
                                                                                                                                                                                                                                             
	select @comm = @comm + ',' + t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                              

                                                                                                                                                                                                                                                             
	--select t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	update @tRes set Comment = Comment + right(@comm, len(@comm) - 1) + ';' where Row_Id = @id
                                                                                                                                                                  
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
update @tRes
                                                                                                                                                                                                                                                 
set StList = 'Yes'
                                                                                                                                                                                                                                           
from @tRes t0 
                                                                                                                                                                                                                                               
	join StopList t1 
                                                                                                                                                                                                                                           
		on t0.ContractId = t1.ItemId and t1.StopDate is null
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
-- drop table #t1
                                                                                                                                                                                                                                            
-- select * into #t1 from @tRes
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
 
                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getPUMBPay](@ip BINARY(4), @RNumber varChar(100)) 
                                                                                                                                                                                 
returns
                                                                                                                                                                                                                                                      
@tRes table (
                                                                                                                                                                                                                                                
	Row_Id int primary key, 
                                                                                                                                                                                                                                    
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90), 
                                                                                                                                                                                                                                  
	suma money default 0,
                                                                                                                                                                                                                                       
	Comment VarChar(500)  default '',
                                                                                                                                                                                                                           
	Comment2 VarChar(50)  default ''
                                                                                                                                                                                                                            
) 
                                                                                                                                                                                                                                                           
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
	declare @Reestr table (id int)
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
	declare @inVal table(row_id int, ContractNum varChar(150))
                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	insert into @inVal (row_id,ContractNum)
                                                                                                                                                                                                                     
	select row_id, max(ISNULL(t1.caseID,t0.ContractNum)) 
                                                                                                                                                                                                       
	from inval t0
                                                                                                                                                                                                                                               
		left join multiplePUMB t1 on t0.ContractNum = t1.caseNum
                                                                                                                                                                                                   
	where t0.ip = @ip
                                                                                                                                                                                                                                           
	group by row_id
                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	insert into @Reestr(id)
                                                                                                                                                                                                                                     
	select id from reestr t0 
                                                                                                                                                                                                                                   
	where t0.RNumber in (select element from fn_strSplit(@RNumber,','))
                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	declare @tNumber table (id int, ContractId bigint) 
                                                                                                                                                                                                         
	declare @tBankId table (id int, ContractId bigint) 
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	insert into @tNumber(id,ContractId)
                                                                                                                                                                                                                         
	SELECT t0.Row_Id,c.id
                                                                                                                                                                                                                                       
	FROM Contract AS c
                                                                                                                                                                                                                                          
		join @Reestr t1 on c.ReestrId = t1.Id
                                                                                                                                                                                                                      
		join @inVal t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                           
	--where  t0.ip = @ip
                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
--	select t0.Row_Id,ContractId from fn_IdbyNumber(@ip,@contragentId) t0
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	insert into @tBankId(id,ContractId)
                                                                                                                                                                                                                         
	select  t2.Row_Id,max(c.id) id
                                                                                                                                                                                                                              
	from
                                                                                                                                                                                                                                                        
		Contract c
                                                                                                                                                                                                                                                 
		join SDSD.ComercialProject.RussiaAgreemID t0 on c.Id = t0.Agreem
                                                                                                                                                                                           
		join @Reestr t1 on c.ReestrId = t1.Id
                                                                                                                                                                                                                      
		join @inVal t2 on t0.id = t2.ContractNum collate Cyrillic_General_CI_AS
                                                                                                                                                                                    
	--where  t2.ip = @ip
                                                                                                                                                                                                                                        
	group by t2.Row_Id
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
--	select t0.Row_Id,ContractId from fn_IdbyBankId(@ip,@contragentId) t0
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id,ContractId)
                                                                                                                                                                                                                        
	select id, max(ContractId) 
                                                                                                                                                                                                                                 
	from 
                                                                                                                                                                                                                                                       
	(
                                                                                                                                                                                                                                                           
		select id, ContractId from @tNumber
                                                                                                                                                                                                                        
		union all
                                                                                                                                                                                                                                                  
		select id, ContractId from @tBankId
                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	) t00
                                                                                                                                                                                                                                                       
	group by id
                                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	update @tRes set ContractNum = t1.ContractNum from @tRes t0 join Contract t1 on t0.ContractId = t1.id
                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	declare @tOthers table (id int, ContractId bigint, ContractNum nVarChar(90))
                                                                                                                                                                                
	declare @tIndex table (ind int identity(1,1), id int )
                                                                                                                                                                                                      
	declare @suff varChar(3), @iTable int = 0, @iTable_max int = 3
                                                                                                                                                                                              
	while @iTable<@iTable_max
                                                                                                                                                                                                                                   
	begin
                                                                                                                                                                                                                                                       
		set @iTable = @iTable +1 
                                                                                                                                                                                                                                  
	
                                                                                                                                                                                                                                                            
		delete @tOthers
                                                                                                                                                                                                                                            
		delete @tIndex
                                                                                                                                                                                                                                             
	
                                                                                                                                                                                                                                                            
		if @iTable = 1 
                                                                                                                                                                                                                                            
		begin 
                                                                                                                                                                                                                                                     
		
                                                                                                                                                                                                                                                           
			insert into @tOthers (id, ContractId, ContractNum)
                                                                                                                                                                                                        
			select t0.id, t0.ContractId, t2.ContractNum
                                                                                                                                                                                                               
			from @tNumber t0
                                                                                                                                                                                                                                          
				left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                  
					and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
				join Contract t2 on t0.ContractId = t2.id
                                                                                                                                                                                                                
			where t1.Row_Id is null
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
			insert @tIndex (id)
                                                                                                                                                                                                                                       
			select distinct id from @tOthers 
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
			set @suff = 'Num'
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		end
                                                                                                                                                                                                                                                        
		else if @iTable = 2
                                                                                                                                                                                                                                        
		begin
                                                                                                                                                                                                                                                      
		
                                                                                                                                                                                                                                                           
			insert into @tOthers (id, ContractId, ContractNum)
                                                                                                                                                                                                        
			select t0.id, t0.ContractId, t2.ContractNum
                                                                                                                                                                                                               
			from @tBankId t0
                                                                                                                                                                                                                                          
				left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                  
					and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
				join Contract t2 on t0.ContractId = t2.id
                                                                                                                                                                                                                
			where t1.Row_Id is null
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
			insert @tIndex (id)
                                                                                                                                                                                                                                       
			select distinct id from @tOthers 
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
			set @suff = 'BId'
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
		declare @i int, @i_max int, @result nVarChar(500) , @id int
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
		select @i=0, @i_max = (select max(ind) from @tIndex)
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
		while @i<@i_max
                                                                                                                                                                                                                                            
		begin
                                                                                                                                                                                                                                                      
			set @i = @i+1
                                                                                                                                                                                                                                             
			select @id = (select max(id) from @tIndex t0 where t0.ind = @i)
                                                                                                                                                                                           
			set @result = '';
                                                                                                                                                                                                                                         
			select @result = @result + ',' + t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                        

                                                                                                                                                                                                                                                             
			--select t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
			update @tRes set Comment = Comment + @suff + ' (' + right(@result, len(@result) - 1) + ');' where Row_Id = @id
                                                                                                                                            
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	end
                                                                                                                                                                                                                                                         
------------------------
                                                                                                                                                                                                                                     
-- calculate comment2
                                                                                                                                                                                                                                        
------------------------
                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	declare @tCom2 table (id int, Comment2 VarChar(50))
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	insert @tCom2(id,Comment2)
                                                                                                                                                                                                                                  
	select row_id,
                                                                                                                                                                                                                                              
		case when t2.id is null then 'Num;' else '' end
                                                                                                                                                                                                            
		+ case when t3.id is null then 'BId;' else '' end
                                                                                                                                                                                                          
	
                                                                                                                                                                                                                                                            
	from inval t0
                                                                                                                                                                                                                                               
		left join @tNumber t2 on t0.Row_Id = t2.id
                                                                                                                                                                                                                 
		left join @tBankId t3 on t0.Row_Id = t3.id
                                                                                                                                                                                                                 
	where ip = @ip
                                                                                                                                                                                                                                              
	 and (t2.id is null or t3.id is null)
                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set Comment2 = t1.Comment2
                                                                                                                                                                                                                                  
	from @tRes t0 join @tCom2 t1 on t0.Row_Id = t1.id
                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
	insert into @tRes (Row_Id,Comment2)
                                                                                                                                                                                                                         
	select t0.id,t0.Comment2 
                                                                                                                                                                                                                                   
	from @tCom2 t0 
                                                                                                                                                                                                                                             
		left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                    
	where t1.Row_Id is null
                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	declare @pay table(row_id int, suma money)
                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	insert into @pay(row_id, suma)
                                                                                                                                                                                                                              
	select t0.Row_Id,sum(t2.SummPay)
                                                                                                                                                                                                                            
	from @tRes t0
                                                                                                                                                                                                                                               
		join inval t1 on t0.Row_Id = t1.Row_Id and t1.Ip = @ip
                                                                                                                                                                                                     
		left join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                          
			on t0.ContractId = t2.ContractId and t1.Date = t2.DatePay
                                                                                                                                                                                                 
	group by t0.Row_Id
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set suma = t2.suma
                                                                                                                                                                                                                                          
	from @tRes t1 
                                                                                                                                                                                                                                              
		join @pay t2 on t1.Row_Id = t2.row_id
                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getPUMBStopList] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                        
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
declare @tReestr table (RNumber int)
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
insert into @tReestr(RNumber)
                                                                                                                                                                                                                                
select cast (element as  int) from  fn_strSplit(@RNumber,',')
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int primary key, ContractId bigint,ContractNum nVarChar(90))
                                                                                                                                                                    

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum) 
                                                                                                                                                                                                          
SELECT t0.Row_Id,c.id,t0.ContractNum
                                                                                                                                                                                                                         
FROM dbo.Contract AS c
                                                                                                                                                                                                                                       
	join reestr t5 on c.ReestrId = t5.id and t5.RNumber in (select RNumber from @tReestr)
                                                                                                                                                                       
	join inval t0 on c.ContractNum = t0.ContractNum and t0.Ip = @Ip
                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,Comment) 
                                                                                                                                                                                                   
select t0.Row_Id, t0.ContractId,t0.ContractNum,
                                                                                                                                                                                                              
	case when colId > 1 then 'col:' + cast(colId as varchar(10)) else '' end
                                                                                                                                                                                    
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId,count(*) colId
                                                                                                                                                                                                    
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          
where colId > 1
                                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractNum,Comment) 
                                                                                                                                                                                                              
select t0.Row_Id,t0.ContractNum,'Id not found'
                                                                                                                                                                                                               
from inVal t0
                                                                                                                                                                                                                                                
  left join @t_All t1 on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                               
where t1.Row_Id is null 
                                                                                                                                                                                                                                     
	and t0.ip = @ip 
                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
declare @maxNom int
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
select @maxNom = max(Row_id)  from inval where ip = @Ip
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id,ContractId, ContractNum) 
                                                                                                                                                                                                           
SELECT  ROW_NUMBER()  OVER(ORDER BY c.ContractNum ASC) + @maxNom,c.id,c.ContractNum
                                                                                                                                                                          
FROM dbo.Contract AS c
                                                                                                                                                                                                                                       
	join reestr t5 on c.ReestrId = t5.id and t5.RNumber in (select RNumber from @tReestr)
                                                                                                                                                                       
--	join inval t0 on c.ContractNum = t0.ContractNum and t0.Ip = @Ip
                                                                                                                                                                                           
	left join @t_All t2 on c.ContractNum = t2.ContractNum
                                                                                                                                                                                                       
	left join StopList t3 on c.id = t3.ItemId
                                                                                                                                                                                                                   
where t2.Row_Id is null and t3.ItemId is null
                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
 
                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getSberId] (@Ip AS binary(15), @RNumber varChar(100),@fileDate dateTime)
                                                                                                                                                           
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	StList varChar(5) default '',
                                                                                                                                                                                                                               
	CurrCode nvarChar(50),
                                                                                                                                                                                                                                      
	Inn varChar(50),
                                                                                                                                                                                                                                            
	dbRecord varChar(10) default '',
                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
--declare @ip binary(4) = 0x0A011E38, @ContragentId int = 423 
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),CurrCode nvarChar(50),Inn varChar(50),DebDate varChar(10))
                                                                                                                      

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,CurrCode,Inn,DebDate) 
                                                                                                                                                                                     
SELECT t0.Row_Id,c.id,t0.ContractNum,
                                                                                                                                                                                                                        
	case when t2.[CurrCode] is null then t3.CurrCode else '' end,
                                                                                                                                                                                               
	case when t4.INN <> t0.INN then t4.INN else '' end,
                                                                                                                                                                                                         
	convert(varChar,@fileDate - t0.Days, 112)
                                                                                                                                                                                                                   
FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                               
	join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                             
	join inval t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                             
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
	left join Currency t2 on t0.CurrCode = t2.[CurrCode]
                                                                                                                                                                                                        
	left join Currency t3 on c.CurrencyId= t3.id
                                                                                                                                                                                                                
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,CurrCode,Inn,DebDate) 
                                                                                                                                                                                     
SELECT t0.Row_Id,c.id,c.ContractNum,
                                                                                                                                                                                                                         
	'',
                                                                                                                                                                                                                                                         
	case when t4.INN <> t0.INN then t4.INN else '' end,
                                                                                                                                                                                                         
	convert(varChar,@fileDate - t0.Days, 112)
                                                                                                                                                                                                                   
FROM multipleContr AS c
                                                                                                                                                                                                                                      
	join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                             
	left join Currency t3 on c.CurrencyId= t3.id
                                                                                                                                                                                                                
	join inval t0 on c.[DocNum] = t0.ContractNum and t0.CurrCode = t3.[CurrCode]--t2.CurrCode
                                                                                                                                                                   
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,CurrCode,inn,Comment,DebDate) 
                                                                                                                                                                              
select t0.Row_Id, t0.ContractId,t0.ContractNum,t0.CurrCode,inn,
                                                                                                                                                                                              
	case when colRec > 1 then 'col:' + cast(colRec as varChar(5)) + ';' else '' end,
                                                                                                                                                                            
	t0.DebDate
                                                                                                                                                                                                                                                  
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId, count(*) colRec
                                                                                                                                                                                                  
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
/*
                                                                                                                                                                                                                                                           
insert into @tRes(Row_Id, ContractId,ContractNum,CurrCode,inn) 
                                                                                                                                                                                              
select t0.Row_Id, t0.ContractId,t0.ContractNum,t0.CurrCode,inn
                                                                                                                                                                                               
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId
                                                                                                                                                                                                                   
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @tOthers table (id int, ContractId bigint, ContractNum nVarChar(90))
                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
insert @tOthers(id,ContractId,ContractNum )
                                                                                                                                                                                                                  
select t0.Row_id,t0.ContractId,t0.ContractNum
                                                                                                                                                                                                                
from @t_All t0 
                                                                                                                                                                                                                                              
	left join @tRes t1 on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                                 
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          
where t1.ContractId is null
                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
declare @tIndex table (ind int identity(1,1), id int )
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
insert @tIndex(id)
                                                                                                                                                                                                                                           
select distinct id from @tOthers
                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @i int, @i_max int, @comm nVarChar(500) , @id int
                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
select @i=0, @i_max = (select max(ind) from @tIndex)
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
while @i<@i_max
                                                                                                                                                                                                                                              
begin
                                                                                                                                                                                                                                                        
	set @i = @i+1
                                                                                                                                                                                                                                               
	select @id = (select max(id) from @tIndex t0 where t0.ind = @i)
                                                                                                                                                                                             
	set @comm = '';
                                                                                                                                                                                                                                             
	select @comm = @comm + ',' + t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                              

                                                                                                                                                                                                                                                             
	--select t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	update @tRes set Comment = Comment + right(@comm, len(@comm) - 1) + ';' where Row_Id = @id
                                                                                                                                                                  
end
                                                                                                                                                                                                                                                          
*/
                                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
update @tRes
                                                                                                                                                                                                                                                 
set dbRecord = CAST(t1.Row_Id as varchar(10))
                                                                                                                                                                                                                
from @tRes t0
                                                                                                                                                                                                                                                
	join 
                                                                                                                                                                                                                                                       
	(
                                                                                                                                                                                                                                                           
	select ContractId,min(Row_Id) Row_Id 
                                                                                                                                                                                                                       
	from @tRes
                                                                                                                                                                                                                                                  
	group by ContractId
                                                                                                                                                                                                                                         
	having count(*)>0
                                                                                                                                                                                                                                           
	) t1 on t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
		and t0.Row_Id <> t1.Row_Id
                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
update @tRes 
                                                                                                                                                                                                                                                
set Comment = Comment+'MC;'
                                                                                                                                                                                                                                  
from @tRes t0 join multipleContr t1 on t0.ContractId =t1.id
                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
update @tRes
                                                                                                                                                                                                                                                 
set StList = 'Yes'
                                                                                                                                                                                                                                           
from @tRes t0 
                                                                                                                                                                                                                                               
	join StopList t1 
                                                                                                                                                                                                                                           
		on t0.ContractId = t1.ItemId and t1.StopDate is null
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
-- drop table #t1
                                                                                                                                                                                                                                            
-- select * into #t1 from @tRes
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getSberIdPay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                           
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	CurrCode nvarChar(50),
                                                                                                                                                                                                                                      
	Inn varChar(50),
                                                                                                                                                                                                                                            
	dbRecord varChar(10) default '',
                                                                                                                                                                                                                            
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
--declare @ip binary(4) = 0x0A011E38, @ContragentId int = 423 
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90),CurrCode nvarChar(50),Inn varChar(50))
                                                                                                                                          

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,CurrCode,Inn) 
                                                                                                                                                                                             
SELECT t0.Row_Id,c.id,t0.ContractNum,
                                                                                                                                                                                                                        
	case when t2.[CurrCode] is null then t3.CurrCode else '' end,
                                                                                                                                                                                               
	case when t4.INN <> t0.INN then t4.INN else '' end
                                                                                                                                                                                                          
FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                               
	join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                             
	join inval t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                             
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
	left join Currency t2 on t0.CurrCode = t2.[CurrCode]
                                                                                                                                                                                                        
	left join Currency t3 on c.CurrencyId= t3.id
                                                                                                                                                                                                                
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum,CurrCode,Inn) 
                                                                                                                                                                                             
SELECT t0.Row_Id,c.id,c.ContractNum,
                                                                                                                                                                                                                         
	'',
                                                                                                                                                                                                                                                         
	case when t4.INN <> t0.INN then t4.INN else '' end
                                                                                                                                                                                                          
FROM multipleContr AS c
                                                                                                                                                                                                                                      
	join dbo.fn_reestrByRNumbers(@RNumber) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                             
	left join Currency t3 on c.CurrencyId= t3.id
                                                                                                                                                                                                                
	join inval t0 on c.[DocNum] = t0.ContractNum and t0.CurrCode = t3.[CurrCode]--t2.CurrCode
                                                                                                                                                                   
	left join Client t4 on c.ClientId = t4.id
                                                                                                                                                                                                                   
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,CurrCode,inn,Comment) 
                                                                                                                                                                                      
select t0.Row_Id, t0.ContractId,t0.ContractNum,t0.CurrCode,t0.inn,
                                                                                                                                                                                           
	case when colRec > 1 then 'col:' + cast(colRec as varChar(5)) + ';' else '' end
                                                                                                                                                                             
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId, count(*) colRec
                                                                                                                                                                                                  
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          
	join inval t2 on t0.Row_Id = t2.Row_Id 
                                                                                                                                                                                                                     
		and t2.ip = @ip and t2.Suma <> 0  and t2.Suma is not null
                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
update @tRes 
                                                                                                                                                                                                                                                
set Comment = Comment+'MC;'
                                                                                                                                                                                                                                  
from @tRes t0 join multipleContr t1 on t0.ContractId =t1.id
                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
declare @pay table(row_id int, suma money)
                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
insert into @pay(row_id, suma)
                                                                                                                                                                                                                               
select t0.Row_Id,sum(t2.SummPay)
                                                                                                                                                                                                                             
from @tRes t0
                                                                                                                                                                                                                                                
	join inval t1 on t0.Row_Id = t1.Row_Id and t1.Ip = @ip
                                                                                                                                                                                                      
	left join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                           
		on t0.ContractId = t2.ContractId and t1.Date = t2.DatePay
                                                                                                                                                                                                  
group by t0.Row_Id
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
update @tRes 
                                                                                                                                                                                                                                                
set suma = t2.suma
                                                                                                                                                                                                                                           
from @tRes t1 
                                                                                                                                                                                                                                               
	join @pay t2 on t1.Row_Id = t2.row_id
                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getSberStopList] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                        
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
declare @tReestr table (id int,RNumber int)
                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
insert into @tReestr(id, RNumber)
                                                                                                                                                                                                                            
select t0.id,t0.RNumber from reestr t0
                                                                                                                                                                                                                       
	join fn_strSplit(@RNumber,',') t1 on t0.RNumber = cast (t1.element as  int)
                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
declare @t_All table (Row_Id int, ContractId bigint,ContractNum nVarChar(90))
                                                                                                                                                                                

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum) 
                                                                                                                                                                                                          
SELECT t0.Row_Id,c.id,t0.ContractNum
                                                                                                                                                                                                                         
FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                               
	join @tReestr t1 on c.ReestrId = t1.Id
                                                                                                                                                                                                                      
	join inval t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                             
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert into @t_All(Row_Id, ContractId,ContractNum) 
                                                                                                                                                                                                          
SELECT t0.Row_Id,c.id,c.ContractNum
                                                                                                                                                                                                                          
FROM multipleContr AS c
                                                                                                                                                                                                                                      
	join @tReestr t1 on c.ReestrId = t1.Id
                                                                                                                                                                                                                      
	left join Currency t3 on c.CurrencyId= t3.id
                                                                                                                                                                                                                
	join inval t0 on c.[DocNum] = t0.ContractNum and t0.CurrCode = t3.[CurrCode]--t2.CurrCode
                                                                                                                                                                   
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractId,ContractNum,Comment) 
                                                                                                                                                                                                   
select t0.Row_Id, t0.ContractId,t0.ContractNum,
                                                                                                                                                                                                              
	case when colId > 1 then 'col:' + cast(colId as varchar(10)) else '' end
                                                                                                                                                                                    
from @t_All t0 join (
                                                                                                                                                                                                                                        
	select row_id, max(ContractId) ContractId,count(*) colId
                                                                                                                                                                                                    
	from @t_All
                                                                                                                                                                                                                                                 
	group by Row_Id
                                                                                                                                                                                                                                             
	) t1 on t0.Row_Id = t1.Row_Id 
                                                                                                                                                                                                                              
		and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                          
where colId > 1
                                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
-- delete @t_All where row_id in (1,2,5)
                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id, ContractNum,Comment) 
                                                                                                                                                                                                              
select t0.Row_Id,t0.ContractNum,'Id not found'
                                                                                                                                                                                                               
from inVal t0
                                                                                                                                                                                                                                                
  left join @t_All t1 on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                               
where t1.Row_Id is null 
                                                                                                                                                                                                                                     
	and t0.ip = @ip 
                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
declare @maxNom int
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
select @maxNom = max(Row_id)  from inval where ip = @Ip
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id,ContractId, ContractNum) 
                                                                                                                                                                                                           
SELECT  ROW_NUMBER()  OVER(ORDER BY c.ContractNum ASC) + @maxNom,c.id,c.ContractNum
                                                                                                                                                                          
FROM dbo.Contract AS c
                                                                                                                                                                                                                                       
	join reestr t5 on c.ReestrId = t5.id and t5.RNumber in (select RNumber from @tReestr)
                                                                                                                                                                       
--	join inval t0 on c.ContractNum = t0.ContractNum and t0.Ip = @Ip
                                                                                                                                                                                           
	left join @t_All t2 on c.ContractNum = t2.ContractNum 
                                                                                                                                                                                                      
	left join StopList t3 on c.id = t3.ItemId
                                                                                                                                                                                                                   
where t2.Row_Id is null and t3.ItemId is null
                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
 
                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function fn_getTrastCheck (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                                  
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	SumTotal money,
                                                                                                                                                                                                                                             
	SumNKS money,
                                                                                                                                                                                                                                               
	SumDiff money,
                                                                                                                                                                                                                                              
	PayDate DateTime,
                                                                                                                                                                                                                                           
	Inn varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,SumTotal,PayDate,Inn,Comment)
                                                                                                                                                                             
	select t0.Row_Id, t0.ContractId, t0.ContractNum, Suma, t1.Date, t0.Inn,t0.Comment
                                                                                                                                                                           
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,null,'NumById') t0
                                                                                                                                                                                                 
		join inVal t1 on t0.Row_Id = t1.Row_Id and t1.ip = @ip
                                                                                                                                                                                                     
	where t1.Suma >0
                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
	declare @startDate datetime, @finishDate datetime,
                                                                                                                                                                                                          
		@minDate datetime = (select min(PayDate) from @tRes),
                                                                                                                                                                                                      
		@maxDate datetime = (select max(PayDate) from @tRes) 
                                                                                                                                                                                                      
	
                                                                                                                                                                                                                                                            
	set @startDate = DATEADD(month, DATEDIFF(month, 0, @minDate), 0)
                                                                                                                                                                                            
	set @finishDate = EOMONTH(@maxDate) 
                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	declare @nksPay table (ContractId bigint,SumNKS money);
                                                                                                                                                                                                     
	
                                                                                                                                                                                                                                                            
	insert into @nksPay (ContractId,SumNKS)
                                                                                                                                                                                                                     
	select t0.ContractId,sum(t2.SummPay) 
                                                                                                                                                                                                                       
	from @tRes t0
                                                                                                                                                                                                                                               
		join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                               
			on t0.ContractId = t2.ContractId and t0.PayDate >= t2.DatePay
                                                                                                                                                                                             
				and DATEADD(month, DATEDIFF(month, 0, t0.PayDate), 0) <= t2.DatePay
                                                                                                                                                                                      
				and t2.DatePay >= @startDate
                                                                                                                                                                                                                             
	group by t0.ContractId
                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	update @tRes
                                                                                                                                                                                                                                                
	set SumNKS = t1.SumNKS, SumDiff = t0.SumTotal - t1.SumNKS
                                                                                                                                                                                                   
	from @tRes t0 
                                                                                                                                                                                                                                              
		join @nksPay t1 on t0.ContractId = t1.ContractId
                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
	--select * from @tRes
                                                                                                                                                                                                                                       
	declare @maxNom int
                                                                                                                                                                                                                                         
	select @maxNom = max(Row_id)  from inval where ip = @Ip
                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(row_id,ContractId,ContractNum,PayDate,SumNKS,SumDiff)
                                                                                                                                                                                     
	select ROW_NUMBER()  OVER(ORDER BY t0.ContractNum ASC) + @maxNom,t0.id,t0.ContractNum, @finishDate,sum(t2.SummPay),-sum(t2.SummPay)
                                                                                                                         
	FROM Contract AS t0
                                                                                                                                                                                                                                         
		join dbo.fn_reestrByRNumbers(@RNumber) t1 on t0.ReestrId = t1.Id
                                                                                                                                                                                           
		join [dbo].[SchedulePay] t2 on t0.id = t2.ContractId
                                                                                                                                                                                                       
			and t2.DatePay between @startDate and @finishDate 
                                                                                                                                                                                                        
		left join @tRes t3 on t0.id = t3.ContractId
                                                                                                                                                                                                                
	where t3.ContractId is null
                                                                                                                                                                                                                                 
	group by t0.id,t0.ContractNum
                                                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getTrastPay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                            
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Suma money,
                                                                                                                                                                                                                                                 
	SumTotal money,
                                                                                                                                                                                                                                             
	SumNKS money,
                                                                                                                                                                                                                                               
	PayDate DateTime,
                                                                                                                                                                                                                                           
	Inn varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,SumTotal,PayDate,Inn,Comment)
                                                                                                                                                                             
	select t0.Row_Id, t0.ContractId, t0.ContractNum, Suma, t1.Date, t0.Inn,t0.Comment
                                                                                                                                                                           
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,null,'NumById') t0
                                                                                                                                                                                                 
		join inVal t1 on t0.Row_Id = t1.Row_Id and t1.ip = @ip
                                                                                                                                                                                                     
	where t1.Suma >0
                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
	declare @nksPay table (ContractId bigint,SumaTotal money, SumaDay money);
                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
	declare @startDate datetime, @minDate datetime = (select min(PayDate) from @tRes) 
                                                                                                                                                                          
	
                                                                                                                                                                                                                                                            
	set @startDate = DATEADD(month, DATEDIFF(month, 0, @minDate), 0)
                                                                                                                                                                                            
	
                                                                                                                                                                                                                                                            
	insert into @nksPay (ContractId,SumaTotal,SumaDay)
                                                                                                                                                                                                          
	select t0.ContractId,
                                                                                                                                                                                                                                       
		sum( case 
                                                                                                                                                                                                                                                 
			when DATEADD(month, DATEDIFF(month, 0, PayDate), 0) < = t2.DatePay 
                                                                                                                                                                                       
				and t0.PayDate > t2.DatePay 
                                                                                                                                                                                                                             
				then t2.SummPay
                                                                                                                                                                                                                                          
				else 0 
                                                                                                                                                                                                                                                  
			end) PeriodPay,
                                                                                                                                                                                                                                           
		sum( case 
                                                                                                                                                                                                                                                 
			when t2.DatePay = t0.PayDate
                                                                                                                                                                                                                              
				then t2.SummPay
                                                                                                                                                                                                                                          
				else 0 
                                                                                                                                                                                                                                                  
			end) DayPay
                                                                                                                                                                                                                                               
	from @tRes t0
                                                                                                                                                                                                                                               
		join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                               
			on t0.ContractId = t2.ContractId and t0.PayDate <= t2.DatePay
                                                                                                                                                                                             
				and t2.DatePay >= @startDate
                                                                                                                                                                                                                             
	group by t0.ContractId
                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	update @tRes
                                                                                                                                                                                                                                                
	set Suma = t0.SumTotal - t1.SumaTotal, SumTotal = t1.SumaTotal, SumNKS = t1.SumaDay
                                                                                                                                                                         
	from @tRes t0 
                                                                                                                                                                                                                                              
		join @nksPay t1 on t0.ContractId = t1.ContractId
                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getTriMobPay] (@Ip AS binary(15), @ContrAgentId int)
                                                                                                                                                                               
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Fio varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	declare @RNumber varChar(100) = ''
                                                                                                                                                                                                                          
	select @RNumber = @RNumber + ',' + ltrim(str(RNumber)) from [dbo].[fn_reestrByContragentId](399)
                                                                                                                                                            

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, suma,Fio,Comment)
                                                                                                                                                                                        
	select Row_Id, ContractId, ContractNum, suma,Fio,Comment
                                                                                                                                                                                                    
	from [dbo].[fn_getBaseIdPay] (@Ip, @RNumber,'NumById')
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getUkrSibRCCPay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                        
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Fio varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, suma,Fio,Comment)
                                                                                                                                                                                        
	select t0.Row_Id, t0.ContractId, t0.ContractNum, t0.suma,t0.Fio,t0.Comment
                                                                                                                                                                                  
	from [dbo].[fn_getBaseIdPay] (@Ip, @RNumber,'NumById') t0
                                                                                                                                                                                                   
	 join inval t1 on t0.row_id = t1.row_id and t1.Ip = @ip
                                                                                                                                                                                                     
	where t1.Suma > 0 
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getUkrSibRCCUpd] (@Ip AS binary(15), @RNumber varChar(100),@fileDate dateTime)
                                                                                                                                                     
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	StList varChar(150) default '',
                                                                                                                                                                                                                             
	Fio varChar(500),
                                                                                                                                                                                                                                           
	Project varChar(15),
                                                                                                                                                                                                                                        
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, DebDate,	StList,Fio,Comment)
                                                                                                                                                                             
	select Row_Id, ContractId, ContractNum, DebDate,	StList,Fio,Comment
                                                                                                                                                                                         
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,@fileDate,'NumById,stopList')
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	declare @tab table(id int, prj varChar(10))
                                                                                                                                                                                                                 
	insert into @tab(id, prj) values(2244,'RCC_1-60')
                                                                                                                                                                                                           
	insert into @tab(id, prj) values(2245,'RCC_1-60')
                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
	declare @tPrj table (id bigInt, Project varChar(15))
                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @tPrj(id,Project)
                                                                                                                                                                                                                               
	select t1.ContractId, t3.prj 
                                                                                                                                                                                                                               
	from 
                                                                                                                                                                                                                                                       
		CONTRACT t0 
                                                                                                                                                                                                                                               
		join @tRes t1 on t0.ContractNum =t1.ContractNum + '_' 
                                                                                                                                                                                                     
		join dbo.fn_reestrByRNumbers(@RNumber) t2 on t0.ReestrId = t2.Id
                                                                                                                                                                                           
		join @tab t3 on t0.ReestrId = t3.id
                                                                                                                                                                                                                        
		join inval t4 on t1.Row_Id = t4.Row_Id and ip = @Ip
                                                                                                                                                                                                        
	where t3.prj != t4.BusId
                                                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set Project = t1.Project
                                                                                                                                                                                                                                    
	from @tRes t0 join @tPrj t1
                                                                                                                                                                                                                                 
		on t0.ContractId = t1.id
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
/*
                                                                                                                                                                                                                                                           
2244	2308
                                                                                                                                                                                                                                                    
2245	2309
                                                                                                                                                                                                                                                    
*/	
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getUkrSibSAPPay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                        
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, suma,Comment)
                                                                                                                                                                                            
	select t0.Row_Id, t0.ContractId, t0.ContractNum, t0.Suma,t0.Comment
                                                                                                                                                                                         
	from [dbo].[fn_getBaseIdPay] (@Ip, @RNumber,'NumById') t0
                                                                                                                                                                                                   
		join inVal t1 on t0.Row_Id = t1.Row_Id and t1.ip = @ip 
                                                                                                                                                                                                    
			and t1.Suma <> 0  and t1.Suma is not null
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getUkrSibStopList]  (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                     
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	Inn varChar(50),
                                                                                                                                                                                                                                            
	StopListId bigint,
                                                                                                                                                                                                                                          
	StopListName varChar(150),
                                                                                                                                                                                                                                  
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum,Inn,Comment,
                                                                                                                                                                                              
		StopListName,StopListId)
                                                                                                                                                                                                                                   
	select t0.Row_Id, t0.ContractId, t0.ContractNum,t0.Inn, t0.Comment,
                                                                                                                                                                                         
		case 
                                                                                                                                                                                                                                                      
			when t1.BusId like '%Полное погашение%' or t1.BusId like '%По ДПД%' 
                                                                                                                                                                                      
				then 'По просьбе банка' -- 266
                                                                                                                                                                                                                           
			when t1.BusId like '%Должник умер%' then 'Умер'-- 239
                                                                                                                                                                                                     
		end , 
                                                                                                                                                                                                                                                     
		case 
                                                                                                                                                                                                                                                      
			when t1.BusId like '%Полное погашение%' or t1.BusId like '%По ДПД%' 
                                                                                                                                                                                      
				then 266
                                                                                                                                                                                                                                                 
			when t1.BusId like '%Должник умер%' then 239
                                                                                                                                                                                                              
		end
                                                                                                                                                                                                                                                        
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,null,'NumById,NumAsNumber') t0
                                                                                                                                                                                     
		join inVal t1 on t0.Row_Id = t1.Row_Id and t1.ip = @ip
                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getUkrSibUl60Pay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                       
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	Fio varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, suma,Fio,Comment)
                                                                                                                                                                                        
	select t0.Row_Id, t0.ContractId, t0.ContractNum, t0.suma,t0.Fio,t0.Comment
                                                                                                                                                                                  
	from [dbo].[fn_getBaseIdPay] (@Ip, @RNumber,'NumById,NumAsNumber') t0
                                                                                                                                                                                       
		join inval t1 on t0.Row_Id = t1.Row_Id and t1.ip = @Ip 
                                                                                                                                                                                                    
			and t1.Suma <> 0 and  t1.Suma is not null
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getUkrSibUl60Upd] (@Ip AS binary(15), @RNumber varChar(100),@fileDate dateTime)
                                                                                                                                                    
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	StList varChar(150) default '',
                                                                                                                                                                                                                             
	Fio varChar(500),
                                                                                                                                                                                                                                           
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id, ContractId, ContractNum, DebDate,	StList,Fio,Comment)
                                                                                                                                                                             
	select Row_Id, ContractId, ContractNum, DebDate,	StList,Fio,Comment
                                                                                                                                                                                         
	from [dbo].[fn_getBaseId] (@Ip, @RNumber,@fileDate,'NumById,NumAsNumber,stopList')
                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
--ALTER function [dbo].[fn_getUkrSotz](@ip varChar(15), @contragentIdChar nvarChar(6)) 
                                                                                                                                                                      
CREATE function [dbo].[fn_getUkrSotz](@ip BINARY(4), @contragentId int, @FileDate datetime) 
                                                                                                                                                                 
returns
                                                                                                                                                                                                                                                      
@tRes table (
                                                                                                                                                                                                                                                
	Row_Id int primary key, 
                                                                                                                                                                                                                                    
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90), 
                                                                                                                                                                                                                                  
	DebDate varChar(10),
                                                                                                                                                                                                                                        
	RNumber int,
                                                                                                                                                                                                                                                
	Comment VarChar(500)  default '',
                                                                                                                                                                                                                           
	Comment2 VarChar(50)  default ''
                                                                                                                                                                                                                            
) 
                                                                                                                                                                                                                                                           
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
	declare @tAccount table (id int, ContractId bigint) 
                                                                                                                                                                                                        
	declare @tNumber table (id int, ContractId bigint) 
                                                                                                                                                                                                         
	declare @tBankId table (id int, ContractId bigint) 
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	insert into @tAccount(id,ContractId)
                                                                                                                                                                                                                        
	select t0.Row_Id,ContractId from fn_IdbyAccount(@ip,@contragentId) t0
                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
	insert into @tNumber(id,ContractId)
                                                                                                                                                                                                                         
	select t0.Row_Id,ContractId from fn_IdbyNumber(@ip,@contragentId) t0
                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @tBankId(id,ContractId)
                                                                                                                                                                                                                         
	select t0.Row_Id,ContractId from fn_IdbyBankId(@ip,@contragentId) t0
                                                                                                                                                                                        

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	insert into @tRes(Row_Id,ContractId)
                                                                                                                                                                                                                        
	select id, max(ContractId) 
                                                                                                                                                                                                                                 
	from 
                                                                                                                                                                                                                                                       
	(
                                                                                                                                                                                                                                                           
		select id, ContractId from @tAccount 
                                                                                                                                                                                                                      
		union all
                                                                                                                                                                                                                                                  
		select id, ContractId from @tNumber
                                                                                                                                                                                                                        
		union all
                                                                                                                                                                                                                                                  
		select id, ContractId from @tBankId
                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	) t00
                                                                                                                                                                                                                                                       
	group by id
                                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	update @tRes set ContractNum = t1.ContractNum from @tRes t0 join Contract t1 on t0.ContractId = t1.id
                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
	declare @tOthers table (id int, ContractId bigint, ContractNum nVarChar(90))
                                                                                                                                                                                
	declare @tIndex table (ind int identity(1,1), id int )
                                                                                                                                                                                                      
	declare @suff varChar(3), @iTable int = 0, @iTable_max int = 3
                                                                                                                                                                                              
	while @iTable<@iTable_max
                                                                                                                                                                                                                                   
	begin
                                                                                                                                                                                                                                                       
		set @iTable = @iTable +1 
                                                                                                                                                                                                                                  
	
                                                                                                                                                                                                                                                            
		delete @tOthers
                                                                                                                                                                                                                                            
		delete @tIndex
                                                                                                                                                                                                                                             
	
                                                                                                                                                                                                                                                            
		if @iTable = 1 
                                                                                                                                                                                                                                            
		begin 
                                                                                                                                                                                                                                                     
		
                                                                                                                                                                                                                                                           
			insert into @tOthers (id, ContractId, ContractNum)
                                                                                                                                                                                                        
			select t0.id, t0.ContractId, t2.ContractNum
                                                                                                                                                                                                               
			from @tNumber t0
                                                                                                                                                                                                                                          
				left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                  
					and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
				join Contract t2 on t0.ContractId = t2.id
                                                                                                                                                                                                                
			where t1.Row_Id is null
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
			insert @tIndex (id)
                                                                                                                                                                                                                                       
			select distinct id from @tOthers 
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
			set @suff = 'Num'
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		end
                                                                                                                                                                                                                                                        
		else if @iTable = 2
                                                                                                                                                                                                                                        
		begin
                                                                                                                                                                                                                                                      
		
                                                                                                                                                                                                                                                           
			insert into @tOthers (id, ContractId, ContractNum)
                                                                                                                                                                                                        
			select t0.id, t0.ContractId, t2.ContractNum
                                                                                                                                                                                                               
			from @tAccount t0
                                                                                                                                                                                                                                         
				left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                  
					and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
				join Contract t2 on t0.ContractId = t2.id
                                                                                                                                                                                                                
			where t1.Row_Id is null
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
			insert @tIndex (id)
                                                                                                                                                                                                                                       
			select distinct id from @tOthers 
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
			set @suff = 'Acc'
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
		else if @iTable = 3
                                                                                                                                                                                                                                        
		begin
                                                                                                                                                                                                                                                      
		
                                                                                                                                                                                                                                                           
			insert into @tOthers (id, ContractId, ContractNum)
                                                                                                                                                                                                        
			select t0.id, t0.ContractId, t2.ContractNum
                                                                                                                                                                                                               
			from @tBankId t0
                                                                                                                                                                                                                                          
				left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                  
					and t0.ContractId = t1.ContractId
                                                                                                                                                                                                                       
				join Contract t2 on t0.ContractId = t2.id
                                                                                                                                                                                                                
			where t1.Row_Id is null
                                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
			insert @tIndex (id)
                                                                                                                                                                                                                                       
			select distinct id from @tOthers 
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
			set @suff = 'BId'
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
		declare @i int, @i_max int, @result nVarChar(500) , @id int
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
		select @i=0, @i_max = (select max(ind) from @tIndex)
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
		while @i<@i_max
                                                                                                                                                                                                                                            
		begin
                                                                                                                                                                                                                                                      
			set @i = @i+1
                                                                                                                                                                                                                                             
			select @id = (select max(id) from @tIndex t0 where t0.ind = @i)
                                                                                                                                                                                           
			set @result = '';
                                                                                                                                                                                                                                         
			select @result = @result + ',' + t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                        

                                                                                                                                                                                                                                                             
			--select t0.ContractNum from @tOthers t0 where t0.id = @id
                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
			update @tRes set Comment = Comment + @suff + ' (' + right(@result, len(@result) - 1) + ');' where Row_Id = @id
                                                                                                                                            
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	end
                                                                                                                                                                                                                                                         
------------------------
                                                                                                                                                                                                                                     
-- calculate comment2
                                                                                                                                                                                                                                        
------------------------
                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	declare @tCom2 table (id int, Comment2 VarChar(50))
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	insert @tCom2(id,Comment2)
                                                                                                                                                                                                                                  
	select row_id,
                                                                                                                                                                                                                                              
		case when t1.id is null then 'Acc;' else '' end
                                                                                                                                                                                                            
		+ case when t2.id is null then 'Num;' else '' end
                                                                                                                                                                                                          
		+ case when t3.id is null then 'BId;' else '' end
                                                                                                                                                                                                          
	
                                                                                                                                                                                                                                                            
	from inval t0
                                                                                                                                                                                                                                               
		left join @tAccount t1 on t0.Row_Id = t1.id
                                                                                                                                                                                                                
		left join @tNumber t2 on t0.Row_Id = t2.id
                                                                                                                                                                                                                 
		left join @tBankId t3 on t0.Row_Id = t3.id
                                                                                                                                                                                                                 
	where ip = @ip
                                                                                                                                                                                                                                              
	 and (t1.id is null or t2.id is null or t3.id is null)
                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set Comment2 = t1.Comment2
	from @tRes t0 join @tCom2 t1 on t0.Row_Id = t1.id
                                                                                                                                                                              

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set DebDate =convert(varChar,@fileDate - t0.Days, 112)
                                                                                                                                                                                                      
	from @tRes t1 join inval t0 on t1.Row_Id = t0.Row_Id and t0.Ip = @ip
                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @tRes (Row_Id,Comment2)
                                                                                                                                                                                                                         
	select t0.id,t0.Comment2 
                                                                                                                                                                                                                                   
	from @tCom2 t0 
                                                                                                                                                                                                                                             
		left join @tRes t1 on t0.id = t1.Row_Id
                                                                                                                                                                                                                    
	where t1.Row_Id is null
                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	update @tRes 
                                                                                                                                                                                                                                               
	set RNumber = t2.RNumber
                                                                                                                                                                                                                                    
	from @tRes t0
                                                                                                                                                                                                                                               
		join CONTRACT t1 on t0.ContractId = t1.id
                                                                                                                                                                                                                  
		join reestr t2 on t1.ReestrId = t2.id
                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
--ALTER function [dbo].[fn_getUkrSotz](@ip varChar(15), @contragentIdChar nvarChar(6)) 
                                                                                                                                                                      
CREATE function [dbo].[fn_getUkrSotzPay](@ip BINARY(4), @contragentId int) 
                                                                                                                                                                                  
returns
                                                                                                                                                                                                                                                      
@tRes table (
                                                                                                                                                                                                                                                
	Row_Id int primary key, 
                                                                                                                                                                                                                                    
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90), 
                                                                                                                                                                                                                                  
	suma money,
                                                                                                                                                                                                                                                 
	RNumber int,
                                                                                                                                                                                                                                                
	Comment VarChar(500)  default '',
                                                                                                                                                                                                                           
	Comment2 VarChar(50)  default ''
                                                                                                                                                                                                                            
) 
                                                                                                                                                                                                                                                           
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
	insert into @tRes (Row_Id,ContractId,ContractNum,RNumber,Comment,Comment2,suma)
                                                                                                                                                                             
	select t0.Row_Id, max(t0.ContractId),max(t0.ContractNum),max(RNumber),
                                                                                                                                                                                      
		max(t0.Comment),max(Comment2),sum(t2.SummPay)
                                                                                                                                                                                                              
	from  fn_getUkrSotz(@ip, @contragentId,null ) t0
                                                                                                                                                                                                            
		left join inval t1 on t0.row_id = t1.row_id and t1.Ip = @ip
                                                                                                                                                                                                
		left join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                          
			on t0.ContractId = t2.ContractId and t1.Date = t2.DatePay
                                                                                                                                                                                                 
	group by t0.Row_Id
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--declare @ip binary(4) = 0x0A011E86 '2224, 2225'
                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_getVabPay] (@Ip AS binary(15), @RNumber varChar(100))
                                                                                                                                                                              
returns 
                                                                                                                                                                                                                                                     
@tRes table
                                                                                                                                                                                                                                                  
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	BusId bigint,
                                                                                                                                                                                                                                               
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	suma money,
                                                                                                                                                                                                                                                 
	sumaVab money,
                                                                                                                                                                                                                                              
	reason money,
                                                                                                                                                                                                                                               
	sumaTotal money,
                                                                                                                                                                                                                                            
	PayDate dateTime,
                                                                                                                                                                                                                                           
	Correct bit default(0),
                                                                                                                                                                                                                                     
	Comment VarChar(250)  default ''
                                                                                                                                                                                                                            
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
insert into @tRes(Row_Id,ContractId,ContractNum,PayDate,Comment)
                                                                                                                                                                                             
select t0.Row_Id,t0.ContractId,t0.ContractNum,t0.PayDate,t0.Comment
                                                                                                                                                                                          
from fn_getBaseId(@ip,@RNumber,null,'NumByBusId') t0
                                                                                                                                                                                                         
	join inVal t1 on t0.Row_Id = t1.Row_Id and t1.ip = @ip
                                                                                                                                                                                                      
where PayDate is not null  --and row_id in (0,1)
                                                                                                                                                                                                             
	and t1.Suma > 0
                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
update @tRes 
                                                                                                                                                                                                                                                
set BusId = t1.BusId,sumaVab = t1.Suma,sumaTotal =t1.Suma2
                                                                                                                                                                                                   
from @tRes t0
                                                                                                                                                                                                                                                
	join inVal t1 on t0.Row_Id = t1.Row_Id and ip = @ip
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
update @tRes 
                                                                                                                                                                                                                                                
set reason = t1.Suma1
                                                                                                                                                                                                                                        
from @tRes t0
                                                                                                                                                                                                                                                
	join inVal t1 on t0.BusId = t1.BusId and t1.ip = @ip
                                                                                                                                                                                                        
		and t0.PayDate = t1.Date1 
                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
declare @tCheckSum table (ContractId bigInt)
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
insert into @tCheckSum (ContractId)
                                                                                                                                                                                                                          
select  t0.ContractId
                                                                                                                                                                                                                                        
from @tRes t0 
                                                                                                                                                                                                                                               
group by t0.ContractId,t0.sumaTotal 
                                                                                                                                                                                                                         
having sumaTotal = sum(sumaVab)
                                                                                                                                                                                                                              
order by t0.ContractId
                                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
update @tRes
                                                                                                                                                                                                                                                 
set Correct = 1
                                                                                                                                                                                                                                              
from @tRes t0 join @tCheckSum t1 
                                                                                                                                                                                                                            
	on t0.ContractId = t1.ContractId
                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @tPay table(ContractId bigint,suma money)
                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
insert into @tPay(ContractId,suma)
                                                                                                                                                                                                                           
select t0.ContractId,sum(t2.SummPay)
                                                                                                                                                                                                                         
from @tRes t0
                                                                                                                                                                                                                                                
		left join [dbo].[SchedulePay] t2 
                                                                                                                                                                                                                          
			on t0.ContractId = t2.ContractId and t0.PayDate = t2.DatePay
                                                                                                                                                                                              
group by t0.ContractId
                                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
update @tRes
                                                                                                                                                                                                                                                 
set suma = t1.suma
                                                                                                                                                                                                                                           
from @tRes t0
                                                                                                                                                                                                                                                
	join @tPay t1 
                                                                                                                                                                                                                                              
	on t0.ContractId = t1.ContractId
                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
insert into @tRes(row_id,Comment)
                                                                                                                                                                                                                            
select min(t0.Row_Id), min(t0.Comment)
                                                                                                                                                                                                                       
from fn_getBaseId(@ip,'2224, 2225',null,'NumByBusId') t0
                                                                                                                                                                                                     
	join inVal t1 on t0.Row_Id = t1.Row_Id
                                                                                                                                                                                                                      
where  t0.ContractId is null
                                                                                                                                                                                                                                 
group by t1.BusId
                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_getViber] (@RNumber varChar(150) = '')
                                                                                                                                                                                             
returns 
                                                                                                                                                                                                                                                     
 @tRes table
                                                                                                                                                                                                                                                 
(
                                                                                                                                                                                                                                                            
	ContractId bigint,
                                                                                                                                                                                                                                          
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	RNumber int,
                                                                                                                                                                                                                                                
	Inn nVarChar(50),
                                                                                                                                                                                                                                           
	RName nVarChar(90),
                                                                                                                                                                                                                                         
	Number nVarChar(50),
                                                                                                                                                                                                                                        
	SummToClose money,
                                                                                                                                                                                                                                          
	CurrCode nVarChar(50)
                                                                                                                                                                                                                                       
)
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
as begin
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
--select * from reestr t0 where t0.IsActual = 1 order 
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
if isnull(@RNumber,'') = '' 
                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
insert into @tRes(ContractId,ContractNum,RNumber,Inn,RName,Number,SummToClose,CurrCode)
                                                                                                                                                                      
select t0.id,t0.ContractNum,t3.RNumber,t1.INN,t3.Name, t2.Value, t0.SummToClose,t4.CurrCode
                                                                                                                                                                  
from CONTRACT t0
                                                                                                                                                                                                                                             
	join Client t1 on t0.ClientId = t1.id
                                                                                                                                                                                                                       
	join viberContact t2 on t1.id = t2.ClientId
                                                                                                                                                                                                                 
	join reestr t3 on t0.ReestrId = t3.id and t3.IsActual = 1
                                                                                                                                                                                                   
	join Currency t4 on t0.CurrencyId = t4.id
                                                                                                                                                                                                                   
else
                                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
insert into @tRes(ContractId,ContractNum,RNumber,Inn,RName,Number,SummToClose,CurrCode)
                                                                                                                                                                      
select t0.id,t0.ContractNum,t3.RNumber,t1.INN,t3.Name, t2.Value, t0.SummToClose,t5.CurrCode
                                                                                                                                                                  
from CONTRACT t0
                                                                                                                                                                                                                                             
	join dbo.fn_reestrByRNumbers(@RNumber) t4 on t0.ReestrId = t4.Id
                                                                                                                                                                                            
	join Client t1 on t0.ClientId = t1.id
                                                                                                                                                                                                                       
	join viberContact t2 on t1.id = t2.ClientId
                                                                                                                                                                                                                 
	join reestr t3 on t0.ReestrId = t3.id
                                                                                                                                                                                                                       
	join Currency t5 on t0.CurrencyId = t5.id
                                                                                                                                                                                                                   
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
--select * from Currency                                                                                                                                                                                                                                       
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_IdbyAccount] (@ip AS BINARY(4), @ContragentId bigInt)
                                                                                                                                                                              
returns 
                                                                                                                                                                                                                                                     
@result table
                                                                                                                                                                                                                                                
(
                                                                                                                                                                                                                                                            
	Row_Id bigint,
                                                                                                                                                                                                                                              
	Acoount varChar(150),
                                                                                                                                                                                                                                       
	ContractNum nVarChar(90),
                                                                                                                                                                                                                                   
	ContractId bigint
                                                                                                                                                                                                                                           
)
                                                                                                                                                                                                                                                            
as
                                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
begin
                                                                                                                                                                                                                                                        
	insert into @result(Row_Id,Acoount,ContractNum,ContractId)
                                                                                                                                                                                                  
	SELECT t0.Row_Id,c.Account, c.ContractNum, c.id
                                                                                                                                                                                                             
	FROM collect.dbo.CONTRACT c
                                                                                                                                                                                                                                 
		join inval t0 on c.Account = t0.Account
                                                                                                                                                                                                                    
		join fn_reestrByContragentId(@ContragentId) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                       
	where  t0.ip = @ip
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	return 
                                                                                                                                                                                                                                                     
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
--
                                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
-- new function
                                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_IdbyBankId] (@ip AS binary(4), @ContragentId bigInt)
                                                                                                                                                                               
returns 
                                                                                                                                                                                                                                                     
@result table
                                                                                                                                                                                                                                                
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint
                                                                                                                                                                                                                                           
)
                                                                                                                                                                                                                                                            
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @result
                                                                                                                                                                                                                                         
	select  t2.Row_Id,max(c.id) id
                                                                                                                                                                                                                              
	from
                                                                                                                                                                                                                                                        
		Contract c
                                                                                                                                                                                                                                                 
		join SDSD.ComercialProject.RussiaAgreemID t0 on c.Id = t0.Agreem
                                                                                                                                                                                           
		join dbo.fn_reestrByContragentId(@contragentId) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                   
		join inval t2 on t0.id = t2.BusId collate Cyrillic_General_CI_AS
                                                                                                                                                                                           
	where  t2.ip = @ip
                                                                                                                                                                                                                                          
	group by t2.Row_Id
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
-- new function
                                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_IdbyBankIdStr] (
                                                                                                                                                                                                                   
	@ip AS VARCHAR(15), 
                                                                                                                                                                                                                                        
	@ContragentStr varChar(500), 
                                                                                                                                                                                                                               
	@separator varChar(5))
                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
returns 
                                                                                                                                                                                                                                                     
@result table
                                                                                                                                                                                                                                                
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint
                                                                                                                                                                                                                                           
)
                                                                                                                                                                                                                                                            
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	declare @ContragentId table(rowId int identity(0,1), ContragentId  bigInt)
                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
	insert into @ContragentId(ContragentId)
                                                                                                                                                                                                                     
	select cast(element as bigint) from fn_strSplit(@ContragentStr,@separator)
                                                                                                                                                                                  
	where isnumeric(element) =1
                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
	declare @reestrId table (Id bigint)
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
	declare @i int  = 0, @max int, @tempId bigInt
                                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
	select @max = max(rowId)+1 from @ContragentId
                                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
	while @i < @max 
                                                                                                                                                                                                                                            
	begin
                                                                                                                                                                                                                                                       
		select @tempId = max(t0.ContragentId) from @ContragentId t0 where t0.rowId = @i
                                                                                                                                                                            
		insert into @reestrId(id) select id from dbo.fn_reestrByContragentId(@tempId)
                                                                                                                                                                              
		set @i = @i +1
                                                                                                                                                                                                                                             
	end 
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	-- dbo.fn_reestrByContragentId(@contragentId)
                                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
	insert into @result
                                                                                                                                                                                                                                         
	select  t2.Row_Id,max(c.id) id
                                                                                                                                                                                                                              
	from
                                                                                                                                                                                                                                                        
		Contract c
                                                                                                                                                                                                                                                 
		join SDSD.ComercialProject.RussiaAgreemID t0 on c.Id = t0.Agreem
                                                                                                                                                                                           
		join (select distinct id from @reestrId) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                          
		join inval t2 on t0.id = t2.BusId collate Cyrillic_General_CI_AS
                                                                                                                                                                                           
	where  t2.ip = dbo.fnBinaryIPv4(@ip)
                                                                                                                                                                                                                        
	group by t2.Row_Id
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
	return
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_IdbyNumber] (@ip AS binary(15), @ContragentId bigInt)
                                                                                                                                                                              
returns 
                                                                                                                                                                                                                                                     
@result table
                                                                                                                                                                                                                                                
(
                                                                                                                                                                                                                                                            
	Row_Id int,
                                                                                                                                                                                                                                                 
	ContractId bigint
                                                                                                                                                                                                                                           
)
                                                                                                                                                                                                                                                            
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
insert into @result(Row_Id,ContractId)
                                                                                                                                                                                                                       
SELECT t0.Row_Id,c.id 
                                                                                                                                                                                                                                       
FROM Collect.dbo.Contract AS c
                                                                                                                                                                                                                               
	join dbo.fn_reestrByContragentId(@ContragentId) t1 on c.ReestrId = t1.Id
                                                                                                                                                                                    
	join inval t0 on c.ContractNum = t0.ContractNum
                                                                                                                                                                                                             
where  t0.ip = @ip
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
return 
                                                                                                                                                                                                                                                      
end
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--declare @ip BINARY(4) = 0x0A011E68
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
create function [dbo].[fn_mNiko](@ip BINARY(4))
                                                                                                                                                                                                              
returns
                                                                                                                                                                                                                                                      
@tRes table (
                                                                                                                                                                                                                                                
	Row_Id int primary key, 
                                                                                                                                                                                                                                    
	Comment int
                                                                                                                                                                                                                                                 
) 
                                                                                                                                                                                                                                                           
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @tRes(row_id,comment)
                                                                                                                                                                                                                           
	select t0.row_id, case when t1.[caseNum] is null then '0' else 1 end
                                                                                                                                                                                        
	from
                                                                                                                                                                                                                                                        
		(select row_id, t00.ContractNum , t00.BusId from inVal t00 where t00.Ip = @ip) t0
                                                                                                                                                                          
		left join multipleNiko t1 on t0.ContractNum  = t1.[caseNum] and t0.BusId = t1.[caseID]
                                                                                                                                                                     

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--declare @ip BINARY(4) = 0x0A011E68
                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
create function fn_mPUMB(@ip BINARY(4))
                                                                                                                                                                                                                      
returns
                                                                                                                                                                                                                                                      
@tRes table (
                                                                                                                                                                                                                                                
	Row_Id int primary key, 
                                                                                                                                                                                                                                    
	Comment int
                                                                                                                                                                                                                                                 
) 
                                                                                                                                                                                                                                                           
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @tRes(row_id,comment)
                                                                                                                                                                                                                           
	select t0.row_id, case when t1.[caseNum] is null then '0' else 1 end
                                                                                                                                                                                        
	from
                                                                                                                                                                                                                                                        
		(select row_id, t00.ContractNum , t00.BusId from inVal t00 where t00.Ip = @ip) t0
                                                                                                                                                                          
		left join multiplePUMB t1 on t0.ContractNum  = t1.[caseNum] and t0.BusId = t1.[caseID]
                                                                                                                                                                     

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE function [dbo].[fn_reestrByContragentId] (@ContragentId bigInt)
                                                                                                                                                                                       
returns 
                                                                                                                                                                                                                                                     
@result table  (id bigint,RNumber int)
                                                                                                                                                                                                                       
as
                                                                                                                                                                                                                                                           
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	insert into @result
                                                                                                                                                                                                                                         
	select  t0.ID,t0.RNumber 
                                                                                                                                                                                                                                   
	from Reestr t0
                                                                                                                                                                                                                                              
	where t0.ContragentId = @ContragentId --and t0.IsActual = 1
                                                                                                                                                                                                 
		and FinishDate >='20180101'
                                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end
                                                                                                                                                                                                                                                          
--
                                                                                                                                                                                                                                                           
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
create function fn_reestrByRNumbers (@RNumber varChar(100))
                                                                                                                                                                                                  
returns @tReestr table (id int, RNumber int)
                                                                                                                                                                                                                 
as 
                                                                                                                                                                                                                                                          
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
insert into @tReestr(id,RNumber)
                                                                                                                                                                                                                             
select t0.id, t0.RNumber 
                                                                                                                                                                                                                                    
from reestr t0
                                                                                                                                                                                                                                               
	join  fn_strSplit(@RNumber,',') t1 on t0.RNumber = cast(t1.element as  int)
                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
return
                                                                                                                                                                                                                                                       
end                                                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
CREATE function [dbo].[fn_strSplit](@string varChar(1000), @separator varChar(5))
                                                                                                                                                                            
returns 
                                                                                                                                                                                                                                                     
	@individuals table (element varChar(150))
                                                                                                                                                                                                                   
begin
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
	declare @individual varchar(150) = null
                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
	while len(@string) > 0
                                                                                                                                                                                                                                      
	begin
                                                                                                                                                                                                                                                       
		if patindex('%'+@separator+'%', @string) > 0
                                                                                                                                                                                                               
		begin
                                                                                                                                                                                                                                                      
			set @individual = substring(@string,
                                                                                                                                                                                                                      
										0,
                                                                                                                                                                                                                                                 
										patindex('%'+@separator+'%', @string))
                                                                                                                                                                                                             
			insert into @individuals(element) values(@individual) 
                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
			set @string = substring(@string,
                                                                                                                                                                                                                          
										len(@individual + @separator) + 1,
                                                                                                                                                                                                                 
										len(@string))
                                                                                                                                                                                                                                      
		end
                                                                                                                                                                                                                                                        
		else
                                                                                                                                                                                                                                                       
		begin
                                                                                                                                                                                                                                                      
			set @individual = @string
                                                                                                                                                                                                                                 
			set @string = null
                                                                                                                                                                                                                                        
			insert into @individuals(element) values(@individual) 
                                                                                                                                                                                                    
		end
                                                                                                                                                                                                                                                        
	end
                                                                                                                                                                                                                                                         
	return
                                                                                                                                                                                                                                                      
end
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
-- ======================================= IP function
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
CREATE FUNCTION fnBinaryIPv4(@ip AS VARCHAR(15)) RETURNS BINARY(4)
                                                                                                                                                                                           
AS
                                                                                                                                                                                                                                                           
BEGIN
                                                                                                                                                                                                                                                        
    DECLARE @bin AS BINARY(4)
                                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
    SELECT @bin = CAST( CAST( PARSENAME( @ip, 4 ) AS INTEGER) AS BINARY(1))
                                                                                                                                                                                  
                + CAST( CAST( PARSENAME( @ip, 3 ) AS INTEGER) AS BINARY(1))
                                                                                                                                                                                  
                + CAST( CAST( PARSENAME( @ip, 2 ) AS INTEGER) AS BINARY(1))
                                                                                                                                                                                  
                + CAST( CAST( PARSENAME( @ip, 1 ) AS INTEGER) AS BINARY(1))
                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
    RETURN @bin
                                                                                                                                                                                                                                              
END
                                                                                                                                                                                                                                                          
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
CREATE FUNCTION fnDisplayIPv4(@ip AS BINARY(4)) RETURNS VARCHAR(15)
                                                                                                                                                                                          
AS
                                                                                                                                                                                                                                                           
BEGIN
                                                                                                                                                                                                                                                        
    DECLARE @str AS VARCHAR(15) 
                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
    SELECT @str = CAST( CAST( SUBSTRING( @ip, 1, 1) AS INTEGER) AS VARCHAR(3) ) + '.'
                                                                                                                                                                        
                + CAST( CAST( SUBSTRING( @ip, 2, 1) AS INTEGER) AS VARCHAR(3) ) + '.'
                                                                                                                                                                        
                + CAST( CAST( SUBSTRING( @ip, 3, 1) AS INTEGER) AS VARCHAR(3) ) + '.'
                                                                                                                                                                        
                + CAST( CAST( SUBSTRING( @ip, 4, 1) AS INTEGER) AS VARCHAR(3) );
                                                                                                                                                                             

                                                                                                                                                                                                                                                             
    RETURN @str
                                                                                                                                                                                                                                              
END;
                                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
-- ======================================= Main function
                                                                                                                                                                                                     
