using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridStudents.Models
{
    class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public object DateCreate { get; set; }
        public List<string> Subjects { get; set; }
        public List<int> Marks { get; set; }

        public string SubjectsAndMarks { get; set; }

        public Student(int Id = -1, string FullName = "None", string Phone = "None", string GroupName = "None", object DateCreate = default, List<string> subjects = null, List<int> marks = null)
        {
            this.Id = Id;
            this.FullName = FullName;
            this.Phone = Phone;
            this.GroupName = GroupName;
            this.DateCreate = DateCreate;
            Subjects = subjects;
            Marks = marks;
        }
    }
}
