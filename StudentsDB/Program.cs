using Bogus;
using Bogus.DataSets;
using Microsoft.Extensions.Configuration;
using StudentsDB.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsDB
{
    class Program
    {
        static string database = "StudentsDB";
        static string dirSql = "SqlTables";
        static SqlConnection con;
        static SqlCommand cmd;
        static int StudentGenerateCount = 0;
        static int StudentsCountToFetch = 0;

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            string connectionDB = configuration.GetConnectionString("DefaultConnection");
            con = new SqlConnection(connectionDB);
            con.Open();
            if (!IsDBExists())
            {
                cmd.CommandText = $"CREATE DATABASE {database}";
                cmd.ExecuteNonQuery();
            }
            con = new SqlConnection(connectionDB + $"Initial Catalog={database}");
            con.Open();
            cmd = con.CreateCommand(); // creating command
            //GenerateTables();
            //GenerateGroups();
            //GenerateSubjects();
            StudentsCountToFetch = GetStudentsCount();
            GenerateStudents();
            GenerateStudentSubjects();
        }

        static bool IsDBExists()
        {
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT name FROM master.sys.databases";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"].ToString() == database)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static void GenerateTables()
        {
            string[] tables = { "tblGroups.sql", "tblStudents.sql", "tblSubjects.sql", "tblStudentSubjects.sql"  }; // tblSubjects.sql (incorrect syntax near int.)
            foreach (var table in tables)
            {
                ExecuteCommandFromFile(table);
            }
        }

        static void ExecuteCommandFromFile(string file)
        {
            string sql = ReadSqlFile(file);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        static string ReadSqlFile(string file)
        {
            string sql = File.ReadAllText($"{dirSql}\\{file}");
            return sql;
        }

        static void GenerateGroups(int count = 10)
        {
            string letters = "QqWwEeRrTtYyUuIiOoPpAaSsDdFfGgHhJjKkLlZzXxCcVvBbNnMm";
            string numbers = "1234567890";

            string[] groupNames = new string[count];
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                for (int l = 0; l < 2; l++)
                {
                    groupNames[i] += letters[rand.Next(0, letters.Length - 1)];
                }
                for (int n = 0; n < 3; n++)
                {
                    groupNames[i] += numbers[rand.Next(0, numbers.Length - 1)];
                }

                int groupCurrentIndex = Array.IndexOf(groupNames, groupNames[i]);
                if (groupCurrentIndex != i && groupCurrentIndex != -1)
                    i--;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");
            dt.Columns.Add("DateCreate");

            RandomDateTime date = new RandomDateTime();
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.NewRow();
                row["Id"] = 0;
                row["Name"] = groupNames[i];
                row["DateCreate"] = date.Next();
                dt.Rows.Add(row);
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
            {
                bulkCopy.DestinationTableName = "tblGroups";
                bulkCopy.WriteToServer(dt);
            }
        }

        static void GenerateSubjects()
        {
            string[] subjects = { "OS", "Diagrams", "C++ OP", "C++ OOP", "C#", "Win Forms (C#)", "WPF C#", "SQL Management Studio" };

            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");

            for (int i = 0; i < subjects.Length; i++)
            {
                DataRow row = dt.NewRow();
                row["Id"] = 0;
                row["Name"] = subjects[i];
                dt.Rows.Add(row);
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
            {
                bulkCopy.DestinationTableName = "tblSubjects";
                bulkCopy.WriteToServer(dt);
            }
        }

        static void GenerateStudents(int count = 100)
        {
            StudentGenerateCount = count;
            Faker<Student> faker = new Faker<Student>("uk")
                .RuleFor(s => s.FullName, (f, u) => f.Name.FullName())
                .RuleFor(s => s.Phone, (f, u) => f.Phone.PhoneNumberFormat());

            List<int> groupsId = GetGroupsId();
            List<Student> students = new List<Student>();
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                var student = faker.Generate();
                int r = rand.Next(0, groupsId.Count - 1);
                student.GroupsId = groupsId[r];
                students.Add(student);
            }

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(nameof(Student.Id)));
            dt.Columns.Add(new DataColumn(nameof(Student.FullName)));
            dt.Columns.Add(new DataColumn(nameof(Student.Phone)));
            dt.Columns.Add(new DataColumn(nameof(Student.GroupsId)));

            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.NewRow();
                row[nameof(Student.Id)] = students[i].Id;
                row[nameof(Student.FullName)] = students[i].FullName;
                row[nameof(Student.Phone)] = students[i].Phone;
                row[nameof(Student.GroupsId)] = students[i].GroupsId;
                dt.Rows.Add(row);
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
            {
                bulkCopy.DestinationTableName = "tblStudents";
                bulkCopy.WriteToServer(dt);
            }
        }

        static List<int> GetGroupsId()
        {
            cmd.CommandText = "SELECT Id " +
                "FROM tblGroups";

            List<int> groupsId = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    groupsId.Add(Int32.Parse(reader["Id"].ToString()));
                }
            }
            return groupsId;
        }

        static void GenerateStudentSubjects()
        {
            List<int> marks = new List<int>()
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
            };
            List<int> subjectsId = GetSubjectsId();
            List<int> studentsId = GetStudentsId();

            DataTable dt = new DataTable();
            dt.Columns.Add("StudentId");
            dt.Columns.Add("SubjectId");
            dt.Columns.Add("Mark");

            Random rand = new Random();
            for (int i = 0; i < studentsId.Count; i++)
            {
                for (int j = 0; j < subjectsId.Count; j++)
                {
                    DataRow row = dt.NewRow();
                    row["StudentId"] = studentsId[i];
                    row["SubjectId"] = subjectsId[j];
                    row["Mark"] = marks[rand.Next(0, marks.Count - 1)];
                    dt.Rows.Add(row);
                }
                
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
            {
                bulkCopy.DestinationTableName = "tblStudentSubjects";
                bulkCopy.WriteToServer(dt);
            }

        }

        static int GetStudentsCount()
        {
            cmd.CommandText = "SELECT COUNT(*) " +
                "FROM tblStudents";

            var reader = cmd.ExecuteReader();
            reader.Read();
            int count = Int32.Parse(reader[0].ToString());
            reader.Close();
            return count;
        }

        static List<int> GetSubjectsId()
        {
            cmd.CommandText = "SELECT Id " +
                "FROM tblSubjects";

            List<int> subjectsId = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    subjectsId.Add(Int32.Parse(reader["Id"].ToString()));
                }
            }
            return subjectsId;
        }

        static List<int> GetStudentsId()
        {
            cmd.CommandText = "SELECT Id " +
                "FROM tblStudents " +
                "ORDER BY Id " +
                $"OFFSET {StudentsCountToFetch} ROWS " +
                $"FETCH NEXT {StudentGenerateCount} ROWS ONLY";
                         
            List<int> studentsId = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    studentsId.Add(Int32.Parse(reader["Id"].ToString()));
                }
            }
            return studentsId;
        }


    }
}
