using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using DataGridStudents.Models;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace DataGridStudents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string database { get; set; } = "StudentsDB";
        private string tblStudents { get; set; } = "tblStudents";
        private string tblGroups { get; set; } = "tblGroups";
        private SqlConnection con { get; set; }
        private SqlCommand cmd { get; set; }
        private List<Student> students { get; set; } = new List<Student>();
        private int MaxStudentsInPage { get; set; } = 20;
        private int pages { get; set; }
        private int start { get; set; }

        private int currentPage { get; set; } = 1;

        public MainWindow()
        {
            InitializeComponent();
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            string connectionDB = configuration.GetConnectionString("DefaultConnection");
            con = new SqlConnection(connectionDB + $"Initial Catalog={database}");
            con.Open();
            cmd = con.CreateCommand(); // creating command

            GetPages();
            GetStudents();
        }

        private void GetStudents(int start = 0)
        {
            this.start = start;
            cmd.CommandText = "SELECT s.Id, s.FullName, s.Phone, g.[Name] AS [Group], g.DateCreate " +
                $"FROM {tblStudents} AS s " +
                $"LEFT JOIN {tblGroups} AS g " +
                "ON s.GroupsId = g.Id " +
                "ORDER BY s.Id " +
                $"OFFSET {start} ROWS " +
                $"FETCH NEXT {MaxStudentsInPage} ROWS ONLY";

            Stopwatch StopWatch = new Stopwatch();
            StopWatch.Start();
            students.Clear();
            dgStudents.ItemsSource = null;
            using (var reader = cmd.ExecuteReader())
            { 
                while (reader.Read())
                {
                    DateTime date = (DateTime)reader["DateCreate"];
                    Student student = new Student(Int32.Parse(reader["Id"].ToString()), reader["FullName"].ToString(), reader["Phone"].ToString(), reader["Group"].ToString(), date.ToShortDateString());
                    students.Add(student);
                }
            }
            dgStudents.ItemsSource = students;
            StopWatch.Stop();
            TimeSpan ts = StopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            TimeElapsed_txt.Text = $"Time elapsed: {elapsedTime}";

            PageOfPages_txt.Text = $"{currentPage} of {pages}";
        }

        private void GetPages()
        {
            cmd.CommandText = $"SELECT COUNT(*) FROM {tblStudents}";
            int StudentsCount = Int32.Parse(cmd.ExecuteScalar().ToString());
            pages = StudentsCount / MaxStudentsInPage;
            int extra = StudentsCount % MaxStudentsInPage;
            if (extra != 0)
            {
                pages++;
            }
        }

        private void Reset_img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetStudents(start);
        }

        private void Create_btn_Click(object sender, RoutedEventArgs e)
        {
            CreateStudentWindow create = new CreateStudentWindow(tblStudents, tblGroups, cmd);
            create.Title = "Student's Creating";
            create.ShowDialog();
            GetPages();
            GetStudents(start);
        }

        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem == null) return;
            int studentID = (dgStudents.SelectedItem as Student).Id;
            CreateStudentWindow update = new CreateStudentWindow(tblStudents, tblGroups, studentID, cmd);
            update.Title = $"Student's #{studentID} Updating";
            update.ShowDialog();
            GetPages();
            GetStudents(start);
        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem == null) return;
            cmd.CommandText = $"DELETE {tblStudents} " +
                $"WHERE Id = {(dgStudents.SelectedItem as Student).Id}";
            cmd.ExecuteNonQuery();
            GetPages();
            GetStudents(start);
        }

        private void PrevPage_img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentPage == 1) return;
            currentPage--;
            start -= MaxStudentsInPage;
            GetStudents(start);
        }

        private void NextPage_img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentPage == pages) return;
            currentPage++;
            start += MaxStudentsInPage;
            GetStudents(start);
        }

        private List<string> GetSubjects()
        {
            cmd.CommandText = "SELECT Name " +
                "FROM tblSubjects";

            List<string> subjects = new List<string>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    subjects.Add(reader["Name"].ToString());
                }
            }
            return subjects;
        }

        private List<int> GetStudentMarks()
        {
            cmd.CommandText = "SELECT Mark " +
                "FROM tblStudentSubjects " +
                $"WHERE StudentId = {(dgStudents.SelectedItem as Student).Id}";

            List<int> marks = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    marks.Add(Int32.Parse(reader["Mark"].ToString()));
                }
            }
            return marks;
        }

        private void GetStudentSubjectAndMarks()
        {
            cmd.CommandText = "SELECT s.[Name] AS [Subject], ss.Mark " +
                "FROM tblStudentSubjects AS ss " +
                "LEFT JOIN tblSubjects AS s " +
                "ON ss.SubjectId = s.Id " +
                $"WHERE ss.StudentId = {(dgStudents.SelectedItem as Student).Id}";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    (dgStudents.SelectedItem as Student).SubjectsAndMarks += reader["Subject"] + ": " + reader["Mark"] + "\n";
                }
            }
        }

        private void dgStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgStudents.SelectedItem == null) return;
            GetStudentSubjectAndMarks();

        }

    }
}