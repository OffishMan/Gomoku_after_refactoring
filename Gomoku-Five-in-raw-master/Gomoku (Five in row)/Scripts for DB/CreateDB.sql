USE [ScoreTable]
GO
/****** Object:  Table [dbo].[Scores]    Script Date: 28.10.2018 0:40:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Scores](
	[Nickname] [nvarchar](50) NOT NULL,
	[Score] [int] NOT NULL
) ON [PRIMARY]
GO
