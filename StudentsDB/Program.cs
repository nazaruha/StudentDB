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
        static string StudentsTable = "tblStudents";
        static string GroupsTable = "tblGroups";
        static SqlConnection con;
        static SqlCommand cmd;

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
            GenerateSubjects();

            //cmd.CommandText = "SELECT g.Id " +
            //    $"FROM {GroupsTable} AS g";
            //List<int> Groups = new List<int>();
            //using (var reader = cmd.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        Groups.Add((int)reader["Id"]);
            //    }
            //}

            //Faker<Student> faker = new Faker<Student>("uk")
            //    .RuleFor(u => u.FullName, (f, u) => f.Name.FullName())
            //    .RuleFor(u => u.Phone, (f, u) => f.Phone.PhoneNumber());

            //Stopwatch StopWatch = new Stopwatch();
            //StopWatch.Start();
            //List<Student> students = new List<Student>();
            //Random rand = new Random();
            //for (int i = 0; i < 1000000; i++)
            //{
            //    var student = faker.Generate();

            //    int index = rand.Next(0, Groups.Count);

            //    student.GroupsId = Groups[index];

            //    students.Add(student);
            //}

            //DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn(nameof(Student.Id)));
            //dt.Columns.Add(new DataColumn(nameof(Student.FullName)));
            //dt.Columns.Add(new DataColumn(nameof(Student.Phone)));
            //dt.Columns.Add(new DataColumn(nameof(Student.GroupsId)));

            //for (int i = 0; i < 1000000; i++)
            //{
            //    int r = rand.Next(0, 100);
            //    var student = students[r];
            //    DataRow row = dt.NewRow();
            //    row[nameof(Student.Id)] = student.Id;
            //    row[nameof(Student.FullName)] = student.FullName;
            //    row[nameof(Student.Phone)] = student.Phone;
            //    row[nameof(Student.GroupsId)] = student.GroupsId;
            //    dt.Rows.Add(row);
            //}
            //using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
            //{
            //    bulkCopy.DestinationTableName = StudentsTable;
            //    bulkCopy.WriteToServer(dt);
            //}
            //StopWatch.Stop();
            //TimeSpan ts = StopWatch.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            //    ts.Hours, ts.Minutes, ts.Seconds,
            //    ts.Milliseconds / 10);
            //Console.WriteLine("RunTime " + elapsedTime);
            //Console.WriteLine("Items are successfully added\n");
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

        static void GenerateGroups()
        {
            int count = 10;

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

    }
}
