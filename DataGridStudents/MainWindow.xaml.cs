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
        private string database { get; set; } = "users";
        private string StudentsTable { get; set; } = "tblStudents";
        private string GroupsTable { get; set; } = "tblGroups";
        private SqlConnection con { get; set; }
        private SqlCommand cmd { get; set; }
        private List<Student> students { get; set; } = new List<Student>();
        private int MaxStudentsInPage { get; set; } = 20;
        private int pages { get; set; }

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

        private void GetStudents(int start = 0, int end = 21)
        {
            cmd.CommandText = $"SELECT * " +
                "FROM " +
                "( " +
                "SELECT ROW_NUMBER() OVER(ORDER BY s.Id) AS row, " +
                "s.Id, s.FullName, s.Phone, g.Name AS GroupName " +
                $"FROM {StudentsTable} AS s " +
                $"INNER JOIN {GroupsTable} AS g " +
                "ON s.GroupsId = g.Id) AS dt " +
                $"WHERE row > {start} AND row < {end}";

            Stopwatch StopWatch = new Stopwatch();
            StopWatch.Start();
            students.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Student student = new Student(Int32.Parse(reader["Id"].ToString()), reader["FullName"].ToString(), reader["Phone"].ToString(), reader["GroupName"].ToString());
                    students.Add(student);
                }
            }
            dgStudents.ItemsSource = null;
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
            cmd.CommandText = $"SELECT COUNT(*) FROM {StudentsTable}";
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
            GetStudents((MaxStudentsInPage * currentPage) - MaxStudentsInPage, MaxStudentsInPage * currentPage + 1);
        }

        private void Create_btn_Click(object sender, RoutedEventArgs e)
        {
            CreateStudentWindow create = new CreateStudentWindow(StudentsTable, GroupsTable, cmd);
            create.Title = "Student's Creating";
            create.ShowDialog();
            GetPages();
            GetStudents((MaxStudentsInPage * currentPage) - MaxStudentsInPage, MaxStudentsInPage * currentPage + 1);
        }

        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem == null) return;
            int studentID = (dgStudents.SelectedItem as Student).Id;
            CreateStudentWindow update = new CreateStudentWindow(StudentsTable, GroupsTable, studentID, cmd);
            update.Title = $"Student's #{studentID} Updating";
            update.ShowDialog();
            GetPages();
            GetStudents((MaxStudentsInPage * currentPage) - MaxStudentsInPage, MaxStudentsInPage * currentPage + 1);
        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem == null) return;
            cmd.CommandText = $"DELETE {StudentsTable} " +
                $"WHERE Id = {(dgStudents.SelectedItem as Student).Id}";
            cmd.ExecuteNonQuery();
            GetPages();
            GetStudents((MaxStudentsInPage * currentPage) - MaxStudentsInPage, MaxStudentsInPage * currentPage + 1);
        }

        private void PrevPage_img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentPage == 1) return;
            currentPage--;
            GetStudents((MaxStudentsInPage * currentPage) - MaxStudentsInPage, MaxStudentsInPage * currentPage + 1);
        }

        private void NextPage_img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentPage == pages) return;
            currentPage++;
            GetStudents((MaxStudentsInPage * currentPage) - MaxStudentsInPage, MaxStudentsInPage * currentPage + 1);
        }
    }
}
