USE [master]
GO
IF EXISTS(SELECT * FROM sys.databases WHERE name='SchemaReaderTest')
	ALTER DATABASE [SchemaReaderTest]
	SET SINGLE_USER
	WITH ROLLBACK IMMEDIATE
GO
IF EXISTS(SELECT * FROM sys.databases WHERE name='SchemaReaderTest')
	DROP DATABASE [SchemaReaderTest]
go
/****** Object:  Database [SchemaReaderTest]    Script Date: 06/19/2012 15:33:27 ******/
CREATE DATABASE [SchemaReaderTest]
GO
ALTER DATABASE [SchemaReaderTest] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SchemaReaderTest].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
USE [SchemaReaderTest]
GO
/****** Object:  Table [dbo].[CandidateKey]    Script Date: 06/19/2012 15:33:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CandidateKey](
	[KeyCol1] [int] NULL,
	[KeyCol2] [varchar](5) NULL,
 CONSTRAINT [IX_CandidateKey] UNIQUE NONCLUSTERED 
(
	[KeyCol1] ASC,
	[KeyCol2] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ForeignKeyReference]    Script Date: 06/19/2012 15:33:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForeignKeyReference](
	[ForeignKey] [int] NOT NULL,
 CONSTRAINT [PK_ForeignKeyReference] PRIMARY KEY CLUSTERED 
(
	[ForeignKey] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ManyDataTypes]    Script Date: 06/19/2012 15:33:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ManyDataTypes](
	[Long] [bigint] NULL,
	[Short] [smallint] NULL,
	[Integer] [int] NULL,
	[Tiny] [tinyint] NULL,
	[UnicodeText] [nvarchar](50) NULL,
	[Varchar] [varchar](50) NULL,
	[FixedLength] [char](10) NULL,
	[UnicodeFixedLength] [nchar](10) NULL,
	[Boolean] [bit] NULL,
	[Binary] [binary](50) NULL,
	[Image] [image] NULL,
	[Timestamp] [timestamp] NULL,
	[Date] [date] NULL,
	[Datetime] [datetime] NULL,
	[Datetime2] [datetime2](7) NULL,
	[Time] [time](7) NULL,
	[Decimal] [decimal](18, 0) NULL,
	[Float] [float] NULL,
	[Money] [money] NULL,
	[Guid] [uniqueidentifier] NULL,
	[Real] [real] NULL,
	[Char] [char](10) NULL,
	[DatetimeOffset] [datetimeoffset](7) NULL,
	[Geography] [geography] NULL,
	[Geometry] [geometry] NULL,
	[Hierarchy] [hierarchyid] NULL,
	[Nchar] [nchar](10) NULL,
	[Ntext] [ntext] NULL,
	[Numeric] [numeric](18, 0) NULL,
	[Smalldatetime] [smalldatetime] NULL,
	[Smallmoney] [smallmoney] NULL,
	[SqlVariant] [sql_variant] NULL,
	[Text] [text] NULL,
	[Varbinary] [varbinary](50) NULL,
	[XML] [xml] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HasPrimaryKey]    Script Date: 06/19/2012 15:33:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HasPrimaryKey](
	[Id] [int] NOT NULL,
	[TextColumn] [nvarchar](50) NULL,
 CONSTRAINT [PK_HasPrimaryKey] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CandidateRef]    Script Date: 06/19/2012 15:33:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CandidateRef](
	[RefCol1] [int] NULL,
	[RefCol2] [varchar](5) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  ForeignKey [FK_HasPrimaryKey_ForeignKeyReference]    Script Date: 06/19/2012 15:33:28 ******/
ALTER TABLE [dbo].[ForeignKeyReference]  WITH CHECK ADD  CONSTRAINT [FK_HasPrimaryKey_ForeignKeyReference] FOREIGN KEY([ForeignKey])
REFERENCES [dbo].[HasPrimaryKey] ([Id])
GO
ALTER TABLE [dbo].[ForeignKeyReference] CHECK CONSTRAINT [FK_HasPrimaryKey_ForeignKeyReference]
GO
/****** Object:  ForeignKey [FK_CandidateRef_CandidateKey]    Script Date: 06/19/2012 15:33:28 ******/
ALTER TABLE [dbo].[CandidateRef]  WITH CHECK ADD  CONSTRAINT [FK_CandidateRef_CandidateKey] FOREIGN KEY([RefCol1], [RefCol2])
REFERENCES [dbo].[CandidateKey] ([KeyCol1], [KeyCol2])
GO
ALTER TABLE [dbo].[CandidateRef] CHECK CONSTRAINT [FK_CandidateRef_CandidateKey]
GO
CREATE TABLE [dbo].[MultipleUniqueKeys](
	[UQ1_1] [int] NOT NULL,
	[UQ1_2] [nvarchar](50) NOT NULL,
	[UQ2_1] [bit] NOT NULL,
	[UQ2_2] [varchar](50) NOT NULL,
	[UQ2_3] [nvarchar](10) NOT NULL,
	[UQ3] [nchar](10) NOT NULL,
 CONSTRAINT [UQ_1] UNIQUE NONCLUSTERED 
(
	[UQ1_1] ASC,
	[UQ1_2] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_2] UNIQUE NONCLUSTERED 
(
	[UQ2_1] ASC,
	[UQ2_2] ASC,
	[UQ2_3] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_3] UNIQUE NONCLUSTERED 
(
	[UQ3] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO