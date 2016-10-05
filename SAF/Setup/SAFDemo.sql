if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DocumentLogging]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[DocumentLogging]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Employees]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Employees]
GO

CREATE TABLE [dbo].[DocumentLogging] (
	[ID] [int] NOT NULL ,
	[Direction] [char] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Sender] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Content] [varchar] (8000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[TimeReceived] [datetime] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Employees] (
	[name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[ssn] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

