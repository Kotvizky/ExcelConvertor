Changed database context to 'ImportProcessing'.

(обработано строк: 14)
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
create procedure CopyTemlpById @id int,  @newId int OUT  
                                                                                                                                                                                                    
as
                                                                                                                                                                                                                                                           
BEGIN TRANSACTION t_Transaction
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
BEGIN TRY
                                                                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
insert into i_tmpl_head WITH (HOLDLOCK) (name,comm) 
                                                                                                                                                                                                         
select left(name + '(copy)',250),comm
                                                                                                                                                                                                                        
from i_tmpl_head 
                                                                                                                                                                                                                                            
where idHead = @id 
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
select @newId = max(idHead) from i_tmpl_head
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
insert i_tmpl_str (idHead,npp,resName,xlsName,isPrint,attr,strFormat,isActive,comm)
                                                                                                                                                                          
select  @newId,npp,resName,xlsName,isPrint,attr,strFormat,isActive,comm
                                                                                                                                                                                      
from i_tmpl_str
                                                                                                                                                                                                                                              
where idHead = @id
                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
COMMIT TRANSACTION t_Transaction
                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
END TRY 
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
BEGIN CATCH
                                                                                                                                                                                                                                                  
  ROLLBACK TRANSACTION t_Transaction
                                                                                                                                                                                                                         
END CATCH
                                                                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
/*
                                                                                                                                                                                                                                                           
delete i_tmpl_str where idHead != 1
                                                                                                                                                                                                                          
delete i_tmpl_head where idHead != 1
                                                                                                                                                                                                                         
*/
                                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
-- select * from i_tmpl_head
                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
create procedure sp_addTmplToFunct @fnName varChar(250), @tmpl varChar(10)
                                                                                                                                                                                   
as
                                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
update impFunc set tmpl = isnull(tmpl,';')+@tmpl+';' where fnName = @fnName
                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
select * from impFunc where fnName = @fnName
                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
--declare @tmpl varChar(10) = 45,  @fnName varChar(250)  = 'fn_getUkrSotz'
                                                                                                                                                                                   
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
	CREATE PROCEDURE dbo.sp_alterdiagram
                                                                                                                                                                                                                        
	(
                                                                                                                                                                                                                                                           
		@diagramname 	sysname,
                                                                                                                                                                                                                                     
		@owner_id	int	= null,
                                                                                                                                                                                                                                      
		@version 	int,
                                                                                                                                                                                                                                             
		@definition 	varbinary(max)
                                                                                                                                                                                                                                
	)
                                                                                                                                                                                                                                                           
	WITH EXECUTE AS 'dbo'
                                                                                                                                                                                                                                       
	AS
                                                                                                                                                                                                                                                          
	BEGIN
                                                                                                                                                                                                                                                       
		set nocount on
                                                                                                                                                                                                                                             
	
                                                                                                                                                                                                                                                            
		declare @theId 			int
                                                                                                                                                                                                                                      
		declare @retval 		int
                                                                                                                                                                                                                                      
		declare @IsDbo 			int
                                                                                                                                                                                                                                      
		
                                                                                                                                                                                                                                                           
		declare @UIDFound 		int
                                                                                                                                                                                                                                    
		declare @DiagId			int
                                                                                                                                                                                                                                      
		declare @ShouldChangeUID	int
                                                                                                                                                                                                                               
	
                                                                                                                                                                                                                                                            
		if(@diagramname is null)
                                                                                                                                                                                                                                   
		begin
                                                                                                                                                                                                                                                      
			RAISERROR ('Invalid ARG', 16, 1)
                                                                                                                                                                                                                          
			return -1
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
		execute as caller;
                                                                                                                                                                                                                                         
		select @theId = DATABASE_PRINCIPAL_ID();	 
                                                                                                                                                                                                                 
		select @IsDbo = IS_MEMBER(N'db_owner'); 
                                                                                                                                                                                                                   
		if(@owner_id is null)
                                                                                                                                                                                                                                      
			select @owner_id = @theId;
                                                                                                                                                                                                                                
		revert;
                                                                                                                                                                                                                                                    
	
                                                                                                                                                                                                                                                            
		select @ShouldChangeUID = 0
                                                                                                                                                                                                                                
		select @DiagId = diagram_id, @UIDFound = principal_id from dbo.sysdiagrams where principal_id = @owner_id and name = @diagramname 
                                                                                                                         
		
                                                                                                                                                                                                                                                           
		if(@DiagId IS NULL or (@IsDbo = 0 and @theId <> @UIDFound))
                                                                                                                                                                                                
		begin
                                                                                                                                                                                                                                                      
			RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1);
                                                                                                                                                                               
			return -3
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
		if(@IsDbo <> 0)
                                                                                                                                                                                                                                            
		begin
                                                                                                                                                                                                                                                      
			if(@UIDFound is null or USER_NAME(@UIDFound) is null) -- invalid principal_id
                                                                                                                                                                             
			begin
                                                                                                                                                                                                                                                     
				select @ShouldChangeUID = 1 ;
                                                                                                                                                                                                                            
			end
                                                                                                                                                                                                                                                       
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
		-- update dds data			
                                                                                                                                                                                                                                      
		update dbo.sysdiagrams set definition = @definition where diagram_id = @DiagId ;
                                                                                                                                                                           

                                                                                                                                                                                                                                                             
		-- change owner
                                                                                                                                                                                                                                            
		if(@ShouldChangeUID = 1)
                                                                                                                                                                                                                                   
			update dbo.sysdiagrams set principal_id = @theId where diagram_id = @DiagId ;
                                                                                                                                                                             

                                                                                                                                                                                                                                                             
		-- update dds version
                                                                                                                                                                                                                                      
		if(@version is not null)
                                                                                                                                                                                                                                   
			update dbo.sysdiagrams set version = @version where diagram_id = @DiagId ;
                                                                                                                                                                                

                                                                                                                                                                                                                                                             
		return 0
                                                                                                                                                                                                                                                   
	END
                                                                                                                                                                                                                                                         
	                                                                                                                                                                                                                                                              
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- select * from reestr t0
                                                                                                                                                                                                                                   
CREATE procedure [dbo].[sp_checkRNum] @ContractNum varChar (90)
                                                                                                                                                                                              
as 
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
declare @RNumber int
                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
select @RNumber = max(t0.ReestrId)
                                                                                                                                                                                                                           
from CONTRACT t0 
                                                                                                                                                                                                                                            
	join reestr t1 on t0.ReestrId = t1.id
                                                                                                                                                                                                                       
where t0.ContractNum = @ContractNum
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
select t1.RNumber,t1.Name
                                                                                                                                                                                                                                    
from reestr t1 
                                                                                                                                                                                                                                              
where t1.id = @RNumber
                                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
select t0.ContractNum,t0.ReestrId,t1.RNumber,t1.Name
                                                                                                                                                                                                         
from CONTRACT t0 
                                                                                                                                                                                                                                            
	join reestr t1 on t0.ReestrId = t1.id and t0.ReestrId = @RNumber
                                                                                                                                                                                            
--		and t1.RNumber = @RNumber
                                                                                                                                                                                                                                
where t0.ContractNum like '%[_]%'
                                                                                                                                                                                                                            
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
create procedure sp_copyTmpl @id int, @newId int
                                                                                                                                                                                                             
as
                                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
declare @npp int = 1000
                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
insert into i_tmpl_str (idHead,npp,resName,xlsName,isPrint,attr,dataType,dataSize,isPos,strFormat,isActive,comm)
                                                                                                                                             
sELECT @newId,npp,resName,xlsName,isPrint,attr,dataType,dataSize,isPos,strFormat,isActive,comm 
                                                                                                                                                              
FROM i_tmpl_str
                                                                                                                                                                                                                                              
where idHead = @id and npp< @npp
                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
	CREATE PROCEDURE dbo.sp_creatediagram
                                                                                                                                                                                                                       
	(
                                                                                                                                                                                                                                                           
		@diagramname 	sysname,
                                                                                                                                                                                                                                     
		@owner_id		int	= null, 	
                                                                                                                                                                                                                                   
		@version 		int,
                                                                                                                                                                                                                                            
		@definition 	varbinary(max)
                                                                                                                                                                                                                                
	)
                                                                                                                                                                                                                                                           
	WITH EXECUTE AS 'dbo'
                                                                                                                                                                                                                                       
	AS
                                                                                                                                                                                                                                                          
	BEGIN
                                                                                                                                                                                                                                                       
		set nocount on
                                                                                                                                                                                                                                             
	
                                                                                                                                                                                                                                                            
		declare @theId int
                                                                                                                                                                                                                                         
		declare @retval int
                                                                                                                                                                                                                                        
		declare @IsDbo	int
                                                                                                                                                                                                                                         
		declare @userName sysname
                                                                                                                                                                                                                                  
		if(@version is null or @diagramname is null)
                                                                                                                                                                                                               
		begin
                                                                                                                                                                                                                                                      
			RAISERROR (N'E_INVALIDARG', 16, 1);
                                                                                                                                                                                                                       
			return -1
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
		execute as caller;
                                                                                                                                                                                                                                         
		select @theId = DATABASE_PRINCIPAL_ID(); 
                                                                                                                                                                                                                  
		select @IsDbo = IS_MEMBER(N'db_owner');
                                                                                                                                                                                                                    
		revert; 
                                                                                                                                                                                                                                                   
		
                                                                                                                                                                                                                                                           
		if @owner_id is null
                                                                                                                                                                                                                                       
		begin
                                                                                                                                                                                                                                                      
			select @owner_id = @theId;
                                                                                                                                                                                                                                
		end
                                                                                                                                                                                                                                                        
		else
                                                                                                                                                                                                                                                       
		begin
                                                                                                                                                                                                                                                      
			if @theId <> @owner_id
                                                                                                                                                                                                                                    
			begin
                                                                                                                                                                                                                                                     
				if @IsDbo = 0
                                                                                                                                                                                                                                            
				begin
                                                                                                                                                                                                                                                    
					RAISERROR (N'E_INVALIDARG', 16, 1);
                                                                                                                                                                                                                     
					return -1
                                                                                                                                                                                                                                               
				end
                                                                                                                                                                                                                                                      
				select @theId = @owner_id
                                                                                                                                                                                                                                
			end
                                                                                                                                                                                                                                                       
		end
                                                                                                                                                                                                                                                        
		-- next 2 line only for test, will be removed after define name unique
                                                                                                                                                                                     
		if EXISTS(select diagram_id from dbo.sysdiagrams where principal_id = @theId and name = @diagramname)
                                                                                                                                                      
		begin
                                                                                                                                                                                                                                                      
			RAISERROR ('The name is already used.', 16, 1);
                                                                                                                                                                                                           
			return -2
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
		insert into dbo.sysdiagrams(name, principal_id , version, definition)
                                                                                                                                                                                      
				VALUES(@diagramname, @theId, @version, @definition) ;
                                                                                                                                                                                                    
		
                                                                                                                                                                                                                                                           
		select @retval = @@IDENTITY 
                                                                                                                                                                                                                               
		return @retval
                                                                                                                                                                                                                                             
	END
                                                                                                                                                                                                                                                         
	                                                                                                                                                                                                                                                              
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
	CREATE PROCEDURE dbo.sp_dropdiagram
                                                                                                                                                                                                                         
	(
                                                                                                                                                                                                                                                           
		@diagramname 	sysname,
                                                                                                                                                                                                                                     
		@owner_id	int	= null
                                                                                                                                                                                                                                       
	)
                                                                                                                                                                                                                                                           
	WITH EXECUTE AS 'dbo'
                                                                                                                                                                                                                                       
	AS
                                                                                                                                                                                                                                                          
	BEGIN
                                                                                                                                                                                                                                                       
		set nocount on
                                                                                                                                                                                                                                             
		declare @theId 			int
                                                                                                                                                                                                                                      
		declare @IsDbo 			int
                                                                                                                                                                                                                                      
		
                                                                                                                                                                                                                                                           
		declare @UIDFound 		int
                                                                                                                                                                                                                                    
		declare @DiagId			int
                                                                                                                                                                                                                                      
	
                                                                                                                                                                                                                                                            
		if(@diagramname is null)
                                                                                                                                                                                                                                   
		begin
                                                                                                                                                                                                                                                      
			RAISERROR ('Invalid value', 16, 1);
                                                                                                                                                                                                                       
			return -1
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
		EXECUTE AS CALLER;
                                                                                                                                                                                                                                         
		select @theId = DATABASE_PRINCIPAL_ID();
                                                                                                                                                                                                                   
		select @IsDbo = IS_MEMBER(N'db_owner'); 
                                                                                                                                                                                                                   
		if(@owner_id is null)
                                                                                                                                                                                                                                      
			select @owner_id = @theId;
                                                                                                                                                                                                                                
		REVERT; 
                                                                                                                                                                                                                                                   
		
                                                                                                                                                                                                                                                           
		select @DiagId = diagram_id, @UIDFound = principal_id from dbo.sysdiagrams where principal_id = @owner_id and name = @diagramname 
                                                                                                                         
		if(@DiagId IS NULL or (@IsDbo = 0 and @UIDFound <> @theId))
                                                                                                                                                                                                
		begin
                                                                                                                                                                                                                                                      
			RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1)
                                                                                                                                                                                
			return -3
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
		delete from dbo.sysdiagrams where diagram_id = @DiagId;
                                                                                                                                                                                                    
	
                                                                                                                                                                                                                                                            
		return 0;
                                                                                                                                                                                                                                                  
	END
                                                                                                                                                                                                                                                         
	                                                                                                                                                                                                                                                              
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
	CREATE PROCEDURE dbo.sp_helpdiagramdefinition
                                                                                                                                                                                                               
	(
                                                                                                                                                                                                                                                           
		@diagramname 	sysname,
                                                                                                                                                                                                                                     
		@owner_id	int	= null 		
                                                                                                                                                                                                                                    
	)
                                                                                                                                                                                                                                                           
	WITH EXECUTE AS N'dbo'
                                                                                                                                                                                                                                      
	AS
                                                                                                                                                                                                                                                          
	BEGIN
                                                                                                                                                                                                                                                       
		set nocount on
                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
		declare @theId 		int
                                                                                                                                                                                                                                       
		declare @IsDbo 		int
                                                                                                                                                                                                                                       
		declare @DiagId		int
                                                                                                                                                                                                                                       
		declare @UIDFound	int
                                                                                                                                                                                                                                      
	
                                                                                                                                                                                                                                                            
		if(@diagramname is null)
                                                                                                                                                                                                                                   
		begin
                                                                                                                                                                                                                                                      
			RAISERROR (N'E_INVALIDARG', 16, 1);
                                                                                                                                                                                                                       
			return -1
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
		execute as caller;
                                                                                                                                                                                                                                         
		select @theId = DATABASE_PRINCIPAL_ID();
                                                                                                                                                                                                                   
		select @IsDbo = IS_MEMBER(N'db_owner');
                                                                                                                                                                                                                    
		if(@owner_id is null)
                                                                                                                                                                                                                                      
			select @owner_id = @theId;
                                                                                                                                                                                                                                
		revert; 
                                                                                                                                                                                                                                                   
	
                                                                                                                                                                                                                                                            
		select @DiagId = diagram_id, @UIDFound = principal_id from dbo.sysdiagrams where principal_id = @owner_id and name = @diagramname;
                                                                                                                         
		if(@DiagId IS NULL or (@IsDbo = 0 and @UIDFound <> @theId ))
                                                                                                                                                                                               
		begin
                                                                                                                                                                                                                                                      
			RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1);
                                                                                                                                                                               
			return -3
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
		select version, definition FROM dbo.sysdiagrams where diagram_id = @DiagId ; 
                                                                                                                                                                              
		return 0
                                                                                                                                                                                                                                                   
	END
                                                                                                                                                                                                                                                         
	                                                                                                                                                                                                                                                              
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
	CREATE PROCEDURE dbo.sp_helpdiagrams
                                                                                                                                                                                                                        
	(
                                                                                                                                                                                                                                                           
		@diagramname sysname = NULL,
                                                                                                                                                                                                                               
		@owner_id int = NULL
                                                                                                                                                                                                                                       
	)
                                                                                                                                                                                                                                                           
	WITH EXECUTE AS N'dbo'
                                                                                                                                                                                                                                      
	AS
                                                                                                                                                                                                                                                          
	BEGIN
                                                                                                                                                                                                                                                       
		DECLARE @user sysname
                                                                                                                                                                                                                                      
		DECLARE @dboLogin bit
                                                                                                                                                                                                                                      
		EXECUTE AS CALLER;
                                                                                                                                                                                                                                         
			SET @user = USER_NAME();
                                                                                                                                                                                                                                  
			SET @dboLogin = CONVERT(bit,IS_MEMBER('db_owner'));
                                                                                                                                                                                                       
		REVERT;
                                                                                                                                                                                                                                                    
		SELECT
                                                                                                                                                                                                                                                     
			[Database] = DB_NAME(),
                                                                                                                                                                                                                                   
			[Name] = name,
                                                                                                                                                                                                                                            
			[ID] = diagram_id,
                                                                                                                                                                                                                                        
			[Owner] = USER_NAME(principal_id),
                                                                                                                                                                                                                        
			[OwnerID] = principal_id
                                                                                                                                                                                                                                  
		FROM
                                                                                                                                                                                                                                                       
			sysdiagrams
                                                                                                                                                                                                                                               
		WHERE
                                                                                                                                                                                                                                                      
			(@dboLogin = 1 OR USER_NAME(principal_id) = @user) AND
                                                                                                                                                                                                    
			(@diagramname IS NULL OR name = @diagramname) AND
                                                                                                                                                                                                         
			(@owner_id IS NULL OR principal_id = @owner_id)
                                                                                                                                                                                                           
		ORDER BY
                                                                                                                                                                                                                                                   
			4, 5, 1
                                                                                                                                                                                                                                                   
	END
                                                                                                                                                                                                                                                         
	                                                                                                                                                                                                                                                              
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
CREATE procedure [dbo].[sp_mNiko] @IP varChar(15), @res varChar(250) out
                                                                                                                                                                                     
as 
                                                                                                                                                                                                                                                          
 
                                                                                                                                                                                                                                                            
 declare @ipBin binary(5)
                                                                                                                                                                                                                                    
	
                                                                                                                                                                                                                                                            
 set @ipBin = dbo.fnBinaryIPv4(@IP)
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
-- select * from inVal t0 where t0.Ip = @ip
                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
declare @newNum table(caseNum varchar(150),	caseID varchar(150))
                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
declare @date datetime = getDate();
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
insert into @newNum(caseNum,caseID)
                                                                                                                                                                                                                          
select distinct ContractNum,BusId
                                                                                                                                                                                                                            
from
                                                                                                                                                                                                                                                         
	(select row_id, t00.ContractNum , t00.BusId from inVal t00 where t00.Ip = @ipBin) t0
                                                                                                                                                                        
	left join multipleNiko t1 on 
                                                                                                                                                                                                                               
		-- t0.ContractNum  = t1.[caseNum] and 
                                                                                                                                                                                                                     
		t0.BusId = t1.[caseID]
                                                                                                                                                                                                                                     
where t1.caseID is null 
                                                                                                                                                                                                                                     
	and ContractNum is not null and BusId is not null
                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert multipleNiko(caseNum,caseID,DateCreate)
                                                                                                                                                                                                               
select caseNum,caseID,@date from @newNum
                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
select @res = 'update rows:' + cast(count(*) as varchar(10)) from @newNum
                                                                                                                                                                                    
--select 'update rows:' + cast(count(*) as varchar(10)) from @newNum
                                                                                                                                                                                         

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE procedure sp_mPUMBupd @ipStr varChar(15), @res varChar(250) out
                                                                                                                                                                                       
as 
                                                                                                                                                                                                                                                          
 
                                                                                                                                                                                                                                                            
 declare @ip binary(5)
                                                                                                                                                                                                                                       
	
                                                                                                                                                                                                                                                            
 set @ip = dbo.fnBinaryIPv4(@ipStr)
                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
-- select * from inVal t0 where t0.Ip = @ip
                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
declare @newNum table(caseNum varchar(150),	caseID varchar(150))
                                                                                                                                                                                             

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
insert into @newNum(caseNum,caseID)
                                                                                                                                                                                                                          
select distinct ContractNum,BusId
                                                                                                                                                                                                                            
from
                                                                                                                                                                                                                                                         
	(select row_id, t00.ContractNum , t00.BusId from inVal t00 where t00.Ip = @ip) t0
                                                                                                                                                                           
	left join multiplePUMB t1 on t0.ContractNum  = t1.[caseNum] and t0.BusId = t1.[caseID]
                                                                                                                                                                      
where t1.caseID is null 
                                                                                                                                                                                                                                     
	and ContractNum is not null and BusId is not null
                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
insert multiplePUMB(caseNum,caseID)
                                                                                                                                                                                                                          
select caseNum,caseID from @newNum
                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
select @res = 'update rows:' + cast(count(*) as varchar(10)) from @newNum
                                                                                                                                                                                    
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE procedure [dbo].[sp_regFunc] @fnName varChar(150), @tmpl int, @exm int = 0
                                                                                                                                                                            
as
                                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
-- @fnName varChar(150) = 'fn_getUkrSotzPay', @tmpl int = 40
                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
declare @tmplStr varChar(10) = ltrim(str(@tmpl))
                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
if (select count(fnName) from impFunc t0 where t0.fnName = @fnName) = 1
                                                                                                                                                                                      
	if (
                                                                                                                                                                                                                                                        
			select count(fnName) from impFunc t0 
                                                                                                                                                                                                                     
			where t0.fnName = @fnName and t0.tmpl like '%;'+@tmplStr+';%'
                                                                                                                                                                                             
		) = 0
                                                                                                                                                                                                                                                      
		begin
                                                                                                                                                                                                                                                      
			update impFunc set tmpl = ISNULL(tmpl,';') + @tmplStr + ';' where fnName = @fnName
                                                                                                                                                                        
			select 'has been updated record - '''+@tmplStr+''''
                                                                                                                                                                                                       
		end
                                                                                                                                                                                                                                                        
		else
                                                                                                                                                                                                                                                       
			select 'record already updated - '''+@tmplStr+''''
                                                                                                                                                                                                        
else 
                                                                                                                                                                                                                                                        
	insert into impFunc(fnName,tmpl) values(@fnName,';'+ @tmplStr +';')
                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
declare @fnExmName varChar(150) = null
                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
select @fnExmName = fnName from impFunc t0 where t0.tmpl like '%;'+ ltrim(str(@exm)) +';%'
                                                                                                                                                                   

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
if @fnExmName is not null
                                                                                                                                                                                                                                    
begin
                                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
	update impFunc 
                                                                                                                                                                                                                                             
	set 
                                                                                                                                                                                                                                                        
		tabFields = (select tabFields from impFunc  where fnName = @fnExmName),
                                                                                                                                                                                    
		inPar = (select inPar from impFunc  where fnName = @fnExmName),
                                                                                                                                                                                            
		outPar = (select outPar from impFunc  where fnName = @fnExmName)
                                                                                                                                                                                           
	where fnName = @fnName
                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
end
                                                                                                                                                                                                                                                          
else 
                                                                                                                                                                                                                                                        
	select 'tmpl ' + str(@exm) + ' not found'
                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
select * from impFunc t0 where t0.fnName = @fnName                                                                                                                                                                                                             
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
	CREATE PROCEDURE dbo.sp_renamediagram
                                                                                                                                                                                                                       
	(
                                                                                                                                                                                                                                                           
		@diagramname 		sysname,
                                                                                                                                                                                                                                    
		@owner_id		int	= null,
                                                                                                                                                                                                                                     
		@new_diagramname	sysname
                                                                                                                                                                                                                                   
	
                                                                                                                                                                                                                                                            
	)
                                                                                                                                                                                                                                                           
	WITH EXECUTE AS 'dbo'
                                                                                                                                                                                                                                       
	AS
                                                                                                                                                                                                                                                          
	BEGIN
                                                                                                                                                                                                                                                       
		set nocount on
                                                                                                                                                                                                                                             
		declare @theId 			int
                                                                                                                                                                                                                                      
		declare @IsDbo 			int
                                                                                                                                                                                                                                      
		
                                                                                                                                                                                                                                                           
		declare @UIDFound 		int
                                                                                                                                                                                                                                    
		declare @DiagId			int
                                                                                                                                                                                                                                      
		declare @DiagIdTarg		int
                                                                                                                                                                                                                                   
		declare @u_name			sysname
                                                                                                                                                                                                                                  
		if((@diagramname is null) or (@new_diagramname is null))
                                                                                                                                                                                                   
		begin
                                                                                                                                                                                                                                                      
			RAISERROR ('Invalid value', 16, 1);
                                                                                                                                                                                                                       
			return -1
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
		EXECUTE AS CALLER;
                                                                                                                                                                                                                                         
		select @theId = DATABASE_PRINCIPAL_ID();
                                                                                                                                                                                                                   
		select @IsDbo = IS_MEMBER(N'db_owner'); 
                                                                                                                                                                                                                   
		if(@owner_id is null)
                                                                                                                                                                                                                                      
			select @owner_id = @theId;
                                                                                                                                                                                                                                
		REVERT;
                                                                                                                                                                                                                                                    
	
                                                                                                                                                                                                                                                            
		select @u_name = USER_NAME(@owner_id)
                                                                                                                                                                                                                      
	
                                                                                                                                                                                                                                                            
		select @DiagId = diagram_id, @UIDFound = principal_id from dbo.sysdiagrams where principal_id = @owner_id and name = @diagramname 
                                                                                                                         
		if(@DiagId IS NULL or (@IsDbo = 0 and @UIDFound <> @theId))
                                                                                                                                                                                                
		begin
                                                                                                                                                                                                                                                      
			RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1)
                                                                                                                                                                                
			return -3
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
	
                                                                                                                                                                                                                                                            
		-- if((@u_name is not null) and (@new_diagramname = @diagramname))	-- nothing will change
                                                                                                                                                                  
		--	return 0;
                                                                                                                                                                                                                                               
	
                                                                                                                                                                                                                                                            
		if(@u_name is null)
                                                                                                                                                                                                                                        
			select @DiagIdTarg = diagram_id from dbo.sysdiagrams where principal_id = @theId and name = @new_diagramname
                                                                                                                                              
		else
                                                                                                                                                                                                                                                       
			select @DiagIdTarg = diagram_id from dbo.sysdiagrams where principal_id = @owner_id and name = @new_diagramname
                                                                                                                                           
	
                                                                                                                                                                                                                                                            
		if((@DiagIdTarg is not null) and  @DiagId <> @DiagIdTarg)
                                                                                                                                                                                                  
		begin
                                                                                                                                                                                                                                                      
			RAISERROR ('The name is already used.', 16, 1);
                                                                                                                                                                                                           
			return -2
                                                                                                                                                                                                                                                 
		end		
                                                                                                                                                                                                                                                      
	
                                                                                                                                                                                                                                                            
		if(@u_name is null)
                                                                                                                                                                                                                                        
			update dbo.sysdiagrams set [name] = @new_diagramname, principal_id = @theId where diagram_id = @DiagId
                                                                                                                                                    
		else
                                                                                                                                                                                                                                                       
			update dbo.sysdiagrams set [name] = @new_diagramname where diagram_id = @DiagId
                                                                                                                                                                           
		return 0
                                                                                                                                                                                                                                                   
	END
                                                                                                                                                                                                                                                         
	                                                                                                                                                                                                                                                              
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                                                                                                                                                                                                                                             
	CREATE PROCEDURE dbo.sp_upgraddiagrams
                                                                                                                                                                                                                      
	AS
                                                                                                                                                                                                                                                          
	BEGIN
                                                                                                                                                                                                                                                       
		IF OBJECT_ID(N'dbo.sysdiagrams') IS NOT NULL
                                                                                                                                                                                                               
			return 0;
                                                                                                                                                                                                                                                 
	
                                                                                                                                                                                                                                                            
		CREATE TABLE dbo.sysdiagrams
                                                                                                                                                                                                                               
		(
                                                                                                                                                                                                                                                          
			name sysname NOT NULL,
                                                                                                                                                                                                                                    
			principal_id int NOT NULL,	-- we may change it to varbinary(85)
                                                                                                                                                                                           
			diagram_id int PRIMARY KEY IDENTITY,
                                                                                                                                                                                                                      
			version int,
                                                                                                                                                                                                                                              
	
                                                                                                                                                                                                                                                            
			definition varbinary(max)
                                                                                                                                                                                                                                 
			CONSTRAINT UK_principal_name UNIQUE
                                                                                                                                                                                                                       
			(
                                                                                                                                                                                                                                                         
				principal_id,
                                                                                                                                                                                                                                            
				name
                                                                                                                                                                                                                                                     
			)
                                                                                                                                                                                                                                                         
		);
                                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
		/* Add this if we need to have some form of extended properties for diagrams */
                                                                                                                                                                            
		/*
                                                                                                                                                                                                                                                         
		IF OBJECT_ID(N'dbo.sysdiagram_properties') IS NULL
                                                                                                                                                                                                         
		BEGIN
                                                                                                                                                                                                                                                      
			CREATE TABLE dbo.sysdiagram_properties
                                                                                                                                                                                                                    
			(
                                                                                                                                                                                                                                                         
				diagram_id int,
                                                                                                                                                                                                                                          
				name sysname,
                                                                                                                                                                                                                                            
				value varbinary(max) NOT NULL
                                                                                                                                                                                                                            
			)
                                                                                                                                                                                                                                                         
		END
                                                                                                                                                                                                                                                        
		*/
                                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
		IF OBJECT_ID(N'dbo.dtproperties') IS NOT NULL
                                                                                                                                                                                                              
		begin
                                                                                                                                                                                                                                                      
			insert into dbo.sysdiagrams
                                                                                                                                                                                                                               
			(
                                                                                                                                                                                                                                                         
				[name],
                                                                                                                                                                                                                                                  
				[principal_id],
                                                                                                                                                                                                                                          
				[version],
                                                                                                                                                                                                                                               
				[definition]
                                                                                                                                                                                                                                             
			)
                                                                                                                                                                                                                                                         
			select	 
                                                                                                                                                                                                                                                  
				convert(sysname, dgnm.[uvalue]),
                                                                                                                                                                                                                         
				DATABASE_PRINCIPAL_ID(N'dbo'),			-- will change to the sid of sa
                                                                                                                                                                                         
				0,							-- zero for old format, dgdef.[version],
                                                                                                                                                                                                        
				dgdef.[lvalue]
                                                                                                                                                                                                                                           
			from dbo.[dtproperties] dgnm
                                                                                                                                                                                                                              
				inner join dbo.[dtproperties] dggd on dggd.[property] = 'DtgSchemaGUID' and dggd.[objectid] = dgnm.[objectid]	
                                                                                                                                           
				inner join dbo.[dtproperties] dgdef on dgdef.[property] = 'DtgSchemaDATA' and dgdef.[objectid] = dgnm.[objectid]
                                                                                                                                         
				
                                                                                                                                                                                                                                                         
			where dgnm.[property] = 'DtgSchemaNAME' and dggd.[uvalue] like N'_EA3E6268-D998-11CE-9454-00AA00A3F36E_' 
                                                                                                                                                 
			return 2;
                                                                                                                                                                                                                                                 
		end
                                                                                                                                                                                                                                                        
		return 1;
                                                                                                                                                                                                                                                  
	END
                                                                                                                                                                                                                                                         
	                                                                                                                                                                                                                                                              
