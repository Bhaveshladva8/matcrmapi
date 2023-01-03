
INSERT INTO [dbo].[CustomTableColumn]
           ([Name],[ControlId],[MasterTableId],[IsDefault],[TenantId],[CreatedOn],[IsDeleted],[DeletedOn])

     VALUES

            --  Start Customer Table column

           ('Name',
           (select Id from CustomControl where Name = 'TextBox'),
		   (select Id from CustomTable where Name = 'Customer'),
           1,
           (select TenantId from Tenants where TenantName = 'testit'),
           GETDATE(),0,NULL),

		   ('Label',
           (select Id from CustomControl where Name = 'DropDown'),
		   (select Id from CustomTable where Name = 'Customer'),
           1,
           (select TenantId from Tenants where TenantName = 'testit'),
           GETDATE(),0,NULL),

		   ('Organization',
           (select Id from CustomControl where Name = 'DropDown'),
		   (select Id from CustomTable where Name = 'Customer'),
           1,
           (select TenantId from Tenants where TenantName = 'testit'),
           GETDATE(),0,NULL),

		   ('Email',
           (select Id from CustomControl where Name = 'TextBox'),
		   (select Id from CustomTable where Name = 'Customer'),
           1,
           (select TenantId from Tenants where TenantName = 'testit'),
           GETDATE(),0,NULL),

		('Phone',
           (select Id from CustomControl where Name = 'TextBox'),
		   (select Id from CustomTable where Name = 'Customer'),
           1,
           (select TenantId from Tenants where TenantName = 'testit'),
           GETDATE(),0,NULL),

	-- 	('ClosedDeals',
      --      (select Id from CustomControl where Name = 'TextBox'),
	-- 	   (select Id from CustomTable where Name = 'Customer'),
      --      1,
      --      (select TenantId from Tenants where TenantName = 'testit'),
      --      GETDATE(),0,NULL), 

	-- 	   ('DoneActivities',
      --      (select Id from CustomControl where Name = 'TextBox'),
	-- 	   (select Id from CustomTable where Name = 'Customer'),
      --      1,
      --      (select TenantId from Tenants where TenantName = 'testit'),
      --      GETDATE(),0,NULL),

	-- 	   ('UpdatedTime',
      --      (select Id from CustomControl where Name = 'TextBox'),
	-- 	   (select Id from CustomTable where Name = 'Customer'),
      --      1,
      --      (select TenantId from Tenants where TenantName = 'testit'),
      --      GETDATE(),0,NULL),

        --    End Customer table column

        --    Start Organization table column

           ('Name',
		   (select Id from CustomControl where Name = 'TextBox'),
		   (select Id from CustomTable where Name = 'Organization'),
		   1,
		   (select TenantId from Tenants where TenantName = 'testit'),
		   GETDATE(),0,NULL),

		   ('Label',
		   (select Id from CustomControl where Name = 'DropDown'),
		   (select Id from CustomTable where Name = 'Organization'),
		   1,
		   (select TenantId from Tenants where TenantName = 'testit'),
		   GETDATE(),0,NULL),

		   ('Address',
		   (select Id from CustomControl where Name = 'TextBox'),
		   (select Id from CustomTable where Name = 'Organization'),
		   1,
		   (select TenantId from Tenants where TenantName = 'testit'),
		   GETDATE(),0,NULL),

		   ('People',
		   (select Id from CustomControl where Name = 'TextBox'),
		   (select Id from CustomTable where Name = 'Organization'),
		   1,
		   (select TenantId from Tenants where TenantName = 'testit'),
		   GETDATE(),0,NULL),

		--    ('ClosedDeals',
		--    (select Id from CustomControl where Name = 'TextBox'),
		--    (select Id from CustomTable where Name = 'Organization'),
		--    1,
		--    (select TenantId from Tenants where TenantName = 'testit'),
		--    GETDATE(),0,NULL),   
		--    ('OpenDeals',
		--    (select Id from CustomControl where Name = 'TextBox'),
		--    (select Id from CustomTable where Name = 'Organization'),
		--    1,
		--    (select TenantId from Tenants where TenantName = 'testit'),
		--    GETDATE(),0,NULL),
        --    End organization table column

      

		 --  Start Lead Table column

           ('Title',
           (select Id from CustomControl where Name = 'TextBox'),
		   (select Id from CustomTable where Name = 'Lead'),
           1,
           (select TenantId from Tenants where TenantName = 'testit'),
           GETDATE(),0,NULL),

		   ('Label',
           (select Id from CustomControl where Name = 'DropDown'),
		   (select Id from CustomTable where Name = 'Lead'),
           1,
           (select TenantId from Tenants where TenantName = 'testit'),
           GETDATE(),0,NULL),

		   ('Organization',
           (select Id from CustomControl where Name = 'DropDown'),
		   (select Id from CustomTable where Name = 'Lead'),
           1,
           (select TenantId from Tenants where TenantName = 'testit'),
           GETDATE(),0,NULL),

		   ('Customer',
           (select Id from CustomControl where Name = 'DropDown'),
		   (select Id from CustomTable where Name = 'Lead'),
           1,
           (select TenantId from Tenants where TenantName = 'testit'),
           GETDATE(),0,NULL),

	-- 	   ('UpdatedTime',
      --      (select Id from CustomControl where Name = 'TextBox'),
	-- 	   (select Id from CustomTable where Name = 'Lead'),
      --      1,
      --      (select TenantId from Tenants where TenantName = 'testit'),
      --      GETDATE(),0,NULL)

        --    End Lead table column
GO

-- For MasterTable Lead table

      INSERT INTO [dbo].[CustomTable]
           ([Name]
           ,[CreatedOn]
           ,[IsDeleted]
           ,[DeletedOn])
     VALUES
           ('Lead'
           ,GETDATE()
           ,0
           ,NULL)
    GO
--   For custom module

INSERT INTO [dbo].[CustomModule]
           ([Name]
           ,[MasterTableId]
           ,[CreatedBy]
           ,[CreatedOn]
           ,[UpdatedBy]
           ,[UpdatedOn]
           ,[IsDeleted]
           ,[DeletedBy]
           ,[DeletedOn])
     VALUES
           ('Lead'
           ,(select Id from CustomTable where Name = 'Lead')
           ,NULL
           ,GETDATE()
           ,NULL
           ,NULL
           ,0
           ,NULL
           ,NULL)
      GO


-- For Update Customer Label and Organization labels color

-- Update CustomerLabel set color = 'green' where Name = 'CUSTOMER'
-- Update OrganizationLabel set color = 'green' where Name = 'CUSTOMER'

-- Update CustomerLabel set color = 'red' where Name = 'HOT LEAD'
-- Update OrganizationLabel set color = 'red' where Name = 'HOT LEAD'

-- Update CustomerLabel set color = '#f8cf07' where Name = 'WARM LEAD'
-- Update OrganizationLabel set color = '#f8cf07' where Name = 'WARM LEAD'

-- Update CustomerLabel set color = '#13b4ff' where Name = 'COLD LEAD'
-- Update OrganizationLabel set color = '#13b4ff' where Name = 'COLD LEAD'

-- Update CustomerLabel set color = '#ab3fdd' where Name = 'SUPPLIER'
-- Update OrganizationLabel set color = '#ab3fdd' where Name = 'SUPPLIER'


------------20211110 Insert column in to CustomTableColumn ------------------------

INSERT INTO [dbo].[CustomTableColumn]
           ([Name]
           ,[ControlId]
           ,[MasterTableId]
           ,[IsDefault]
           ,[TenantId]
           ,[CreatedOn]
           ,[IsDeleted]
           ,[DeletedOn]
           ,[CustomFieldId])
     VALUES
           ('FirstName'
           ,(select Id from CustomControl where Name = 'TextBox')
           ,(select Id from CustomTable where Name = 'Customer')
           ,1
           ,NULL
           ,GETDATE()
           ,0
           ,NULL
           ,NULL),
		   ('LastName'
           ,(select Id from CustomControl where Name = 'TextBox')
           ,(select Id from CustomTable where Name = 'Customer')
           ,1
           ,NULL
           ,GETDATE()
           ,0
           ,NULL
           ,NULL),
		   ('Salutation'
           ,(select Id from CustomControl where Name = 'DropDown')
           ,(select Id from CustomTable where Name = 'Customer')
           ,1
           ,NULL
           ,GETDATE()
           ,0
           ,NULL
           ,NULL)
GO
-----------------------------------------------------------------------

------------20211115 Change customer to person ------------------------

Update CustomModule set Name = 'Person' where Name = 'Customer'

Update CustomTable set Name = 'Person' where Name = 'Customer'

Update LabelCategory set Name = 'Person' where Name = 'Customer'

Update OneClappFormAction set Name = 'Person' where Name = 'Customer'

Update CustomTableColumn set Name = 'Person' where IsDefault = 1 and Name = 'Customer'

------------------------------------------------------------------------

-------------------------------------postGreSQL------------------------


INSERT INTO public."CustomTable"(
	 "Name", "CreatedOn", "IsDeleted", "DeletedOn")
	VALUES ('Project',NOW(), false, NULL),
             ('Task',NOW(), false, NULL)

INSERT INTO public."CustomModule"(
	 "Name", "MasterTableId", "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn", "IsDeleted", "DeletedBy", "DeletedOn")
	VALUES ( 'TimeTrack',  (select "Id" from public."CustomTable" where "Name" = 'Project'), null, NOW(), 
			null, null, false, null, null),
      ( 'Task',  (select "Id" from public."CustomTable" where "Name" = 'Task'), null, NOW(), 
		      null, null, false, null, null)
			
	
INSERT INTO public."CustomTableColumn"(
	 "Name", "ControlId", "MasterTableId", "IsDefault", "CustomFieldId", "TenantId", "CreatedOn", "IsDeleted", "DeletedOn")
	VALUES ( 'Name',
			 (Select "Id" From public."CustomControl" Where "Name" = 'TextBox'),
			  (select "Id" from public."CustomTable" where "Name" = 'Project'),
			  true,NULL,
			  (select "TenantId" from public."Tenants" where "TenantName" = 'testit'),
              NOW(), false, NULL),
			
			( 'Description',
			 (Select "Id" From public."CustomControl" Where "Name" = 'TextArea'),
			  (select "Id" from public."CustomTable" where "Name" = 'Project'),
			  true,NULL,
			  (select "TenantId" from public."Tenants" where "TenantName" = 'testit'),
              NOW(), false, NULL),
			 
			 ( 'Due Date',
			 (Select "Id" From public."CustomControl" Where "Name" = 'Date'),
			  (select "Id" from public."CustomTable" where "Name" = 'Project'),
			  true,NULL,
			  (select "TenantId" from public."Tenants" where "TenantName" = 'testit'),
              NOW(), false, NULL),
			  
			    ( 'Status',
			 (Select "Id" From public."CustomControl" Where "Name" = 'DropDown'),
			  (select "Id" from public."CustomTable" where "Name" = 'Project'),
			  true,NULL,
			  (select "TenantId" from public."Tenants" where "TenantName" = 'testit'),
              NOW(), false, NULL),
			  
			  ( 'Logo',
			 (Select "Id" From public."CustomControl" Where "Name" = 'TextBox'),
			  (select "Id" from public."CustomTable" where "Name" = 'Project'),
			  true,NULL,
			  (select "TenantId" from public."Tenants" where "TenantName" = 'testit'),
              NOW(), false, NULL),
			   
			  ( 'EstimatedTime',
			 (Select "Id" From public."CustomControl" Where "Name" = 'TextBox'),
			  (select "Id" from public."CustomTable" where "Name" = 'Project'),
			  true,NULL,
			  (select "TenantId" from public."Tenants" where "TenantName" = 'testit'),
              NOW(), false, NULL)
			 ;