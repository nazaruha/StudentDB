IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[dbo].[tblStudents]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TABLE [dbo].[tblStudents](
  [Id] [int] IDENTITY PRIMARY KEY NOT NULL,
  [FullName] [nvarchar](50) NOT NULL,
  [Phone] [nvarchar](20) NOT NULL,
  [GroupsId] [int] NOT NULL,

  CONSTRAINT [FK_tblStudents_tblGroups] FOREIGN KEY([GroupsId])
   REFERENCES [dbo].[tblGroups] ([Id]),
);'