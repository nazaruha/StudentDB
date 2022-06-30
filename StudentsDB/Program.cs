﻿using Bogus;
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

        static void Main(string[] args)
        {
            string database = "users";
            string StudentsTable = "tblStudents";
            string GroupsTable = "tblGroups";

            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            string connectionDB = configuration.GetConnectionString("DefaultConnection");
            var con = new SqlConnection(connectionDB + $"Initial Catalog={database}");
            con.Open();
            var cmd = con.CreateCommand(); // creating command

            cmd.CommandText = "SELECT g.Id " +
                $"FROM {GroupsTable} AS g";
            List<int> Groups = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Groups.Add((int)reader["Id"]);
                }
            }

            Faker<Student> faker = new Faker<Student>("uk")
                .RuleFor(u => u.FullName, (f, u) => f.Name.FullName())
                .RuleFor(u => u.Phone, (f, u) => f.Phone.PhoneNumber());

            Stopwatch StopWatch = new Stopwatch();
            StopWatch.Start();
            List<Student> students = new List<Student>();
            Random rand = new Random();
            for (int i = 0; i < 1000000; i++)
            {
                var student = faker.Generate();

                int index = rand.Next(0, Groups.Count);

                student.GroupsId = Groups[index];

                students.Add(student);
            }

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(nameof(Student.Id)));
            dt.Columns.Add(new DataColumn(nameof(Student.FullName)));
            dt.Columns.Add(new DataColumn(nameof(Student.Phone)));
            dt.Columns.Add(new DataColumn(nameof(Student.GroupsId)));

            for (int i = 0; i < 1000000; i++)
            {
                int r = rand.Next(0, 100);
                var student = students[r];
                DataRow row = dt.NewRow();
                row[nameof(Student.Id)] = student.Id;
                row[nameof(Student.FullName)] = student.FullName;
                row[nameof(Student.Phone)] = student.Phone;
                row[nameof(Student.GroupsId)] = student.GroupsId;
                dt.Rows.Add(row);
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
            {
                bulkCopy.DestinationTableName = StudentsTable;
                bulkCopy.WriteToServer(dt);
            }
            StopWatch.Stop();
            TimeSpan ts = StopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            Console.WriteLine("Items are successfully added\n");
        }
    }
}
