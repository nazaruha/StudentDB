using DataGridStudents.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataGridStudents
{
    /// <summary>
    /// Interaction logic for CreateStudentWindow.xaml
    /// </summary>
    public partial class CreateStudentWindow : Window
    {
        private string StudentsTable { get; set; }
        private string GroupsTable { get; set; }
        private int StudentId { get; set; }
        private SqlCommand cmd { get; set; }
        public CreateStudentWindow(string StudentsTable, string GroupsTable, SqlCommand cmd)
        {
            InitializeComponent();

            this.StudentsTable = StudentsTable;
            this.GroupsTable = GroupsTable;
            this.cmd = cmd;
            InitializeGroupComboBox();
            Create_btn.Content = "Create";
        }

        public CreateStudentWindow(string StudentsTable, string GroupsTable, int StudentId, SqlCommand cmd)
        {
            InitializeComponent();
            this.StudentsTable = StudentsTable;
            this.GroupsTable = GroupsTable;
            this.StudentId = StudentId;
            this.cmd = cmd;
            InitializeGroupComboBox();
            Create_btn.Content = "Update";

            cmd.CommandText = "SELECT s.Id, s.FullName, s.Phone, g.Name as GroupName " +
                $"FROM {StudentsTable} AS s " +
                $"INNER JOIN {GroupsTable} AS g " +
                "ON s.GroupsId = g.Id " +
                $"WHERE s.Id = {StudentId}";
            var reader = cmd.ExecuteReader();
            reader.Read();
            FullName_txt.Text = reader["FullName"].ToString();
            Phone_txt.Text = reader["Phone"].ToString();
            Group_cbx.SelectedItem = reader["GroupName"].ToString();
        }

        private void InitializeGroupComboBox()
        {
            cmd.CommandText = "SELECT g.[Name] " +
                $"FROM {GroupsTable} AS g";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Group_cbx.Items.Add(reader["Name"].ToString());
                }
            }
        }

        private void Create_btn_Click(object sender, RoutedEventArgs e)
        {
            if (FullName_txt.BorderBrush == Brushes.Red || Phone_txt.BorderBrush == Brushes.Red || String.IsNullOrWhiteSpace(FullName_txt.Text) || String.IsNullOrWhiteSpace(Phone_txt.Text)) return;
            else if (Group_cbx.BorderBrush == Brushes.Red || Group_cbx.SelectedIndex == -1) return;

            int groupID = GetGroupId();

           if (!IsStudentExist(groupID))
           {
                MessageBox.Show("This Student is already in database", "DROP CREATING", MessageBoxButton.OK);
                return;
           }

           if (this.Title == "Student's Creating")
            {
                cmd.CommandText = $"INSERT INTO {StudentsTable}" +
                "(" +
                "FullName, Phone, GroupsId" +
                ") " +
                "VALUES (" +
                $"'{FullName_txt.Text}', '{Phone_txt.Text}', {groupID}" +
                ")";
                cmd.ExecuteNonQuery();

                MessageBox.Show("Student is successfully created!", "SUCCESS CREATE", MessageBoxButton.OK);
            }
            else
            {
                cmd.CommandText = $"UPDATE {StudentsTable} SET " +
                    $"FullName = '{FullName_txt.Text}', Phone = '{Phone_txt.Text}', GroupsId = {groupID} " +
                    $"WHERE Id = {StudentId}";
                cmd.ExecuteNonQuery();
                MessageBox.Show($"Student #{StudentId} is successfully updated", "SUCCESS UPDATE", MessageBoxButton.OK);
            }

        }

        private bool IsStudentExist(int groupID)
        {
            if (this.Title == "Student's Creating")
            {
                cmd.CommandText = $"SELECT * FROM {StudentsTable} " +
                $"WHERE FullName = '{FullName_txt.Text}' AND Phone = '{Phone_txt.Text}' AND GroupsId = {groupID}";

                var reader = cmd.ExecuteReader();
                reader.Read();
                try
                {
                    Student student = new Student() { FullName = reader["FullName"].ToString(), Phone = reader["Phone"].ToString(), GroupId = Int32.Parse(reader["GroupsId"].ToString()) };
                    return false;
                }
                catch
                {
                    return true;
                }
            }
            else
            {
                cmd.CommandText = $"SELECT s.Id FROM {StudentsTable} AS s " +
                    $"WHERE FullName = '{FullName_txt.Text}' AND Phone = '{Phone_txt.Text}' AND GroupsId = {groupID}";

                try
                {
                    var reader = cmd.ExecuteScalar().ToString();
                    int id = Int32.Parse(reader);
                    if (id == StudentId)
                        return true;
                    return false;
                }
                catch
                {
                    return true;
                }


            }
            
        }

        private int GetGroupId()
        {
            cmd.CommandText = "SELECT g.Id " +
                $"FROM {GroupsTable} AS g " +
                $"WHERE g.Name = '{Group_cbx.SelectedItem.ToString()}'";

            int groupID = Int32.Parse(cmd.ExecuteScalar().ToString());
            return groupID;
        }

        private void FullName_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(FullName_txt.Text)) FullName_txt.BorderBrush = Brushes.Red;
            Regex regex = new Regex(@"^[A-ZА-ЯІЇ][a-zа-яії]*\s[A-ZА-ЯІЇ][a-zа-яії]*$");
            if (!regex.IsMatch(FullName_txt.Text))
            {
                FullName_txt.BorderBrush = Brushes.Red;
            }
            else
            {
                FullName_txt.BorderBrush = Brushes.Gray;
                ErrorFullName_lb.Visibility = Visibility.Hidden;
            }
        }

        private void FullName_txt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FullName_txt.BorderBrush == Brushes.Red)
                ErrorFullName_lb.Visibility = Visibility.Visible;
        }

        private void Phone_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Phone_txt.Text)) Phone_txt.BorderBrush = Brushes.Red;
            Regex regex = new Regex(@"^\(\d{3}\)\s\d{3}-\d{2}-\d{2}$");
            if (!regex.IsMatch(Phone_txt.Text))
            {
                Phone_txt.BorderBrush = Brushes.Red;
            }
            else
            {
                Phone_txt.BorderBrush = Brushes.Gray;
                ErrorPhone_lb.Visibility = Visibility.Hidden;
            }
        }

        private void Phone_txt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Phone_txt.BorderBrush == Brushes.Red)
                ErrorPhone_lb.Visibility = Visibility.Visible;
        }

        private void WrapPanel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Group_cbx.SelectedIndex == -1)
            {
                Group_cbx.BorderBrush = Brushes.Red;
                ErrorGroup_lb.Visibility = Visibility.Visible;
            }
            else
            {
                Group_cbx.BorderBrush = Brushes.Gray;
                ErrorGroup_lb.Visibility = Visibility.Hidden;
            }
        }

        private void ClearWindow_img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FullName_txt.Clear();
            FullName_txt.BorderBrush = Brushes.Gray;
            ErrorFullName_lb.Visibility = Visibility.Hidden;

            Phone_txt.Clear();
            Phone_txt.BorderBrush = Brushes.Gray;
            ErrorPhone_lb.Visibility = Visibility.Hidden;

            Group_cbx.SelectedItem = null;
            Group_cbx.BorderBrush = Brushes.Gray;
            ErrorGroup_lb.Visibility = Visibility.Hidden;
        }
    }
}
