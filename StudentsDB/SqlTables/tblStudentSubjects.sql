IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[dbo].[tblStudentSubjects]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TABLE [dbo].[tblStudentSubjects](
  [StudentId] [int] NOT NULL,
  [SubjectId] [int] NOT NULL,
  [Mark] [int] NOT NULL,  

  CONSTRAINT [PK_tblStudentSubjects] PRIMARY KEY CLUSTERED
  (  [StudentId] ASC, [SubjectId] ASC ),

  CONSTRAINT [FK_tblStudentSubjects_tblStudents] FOREIGN KEY([StudentId])
   REFERENCES [dbo].[tblStudents] ([Id]),

  CONSTRAINT [FK_tblStudentSubjects_tblSubjects] FOREIGN KEY([SubjectId])
   REFERENCES [dbo].[tblSubjects] ([Id])
);'